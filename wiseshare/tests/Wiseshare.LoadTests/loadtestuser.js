import http from 'k6/http';
import { check, sleep } from 'k6';
import { uuidv4 } from 'https://jslib.k6.io/k6-utils/1.1.0/index.js';
import { Counter } from 'k6/metrics';
import { htmlReport } from 'https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js';

export let options = {
    vus: 100,
    duration: '150s',
    thresholds: {
        http_req_duration: ['p(95)<500'],
        http_req_failed: ['rate<0.01'],
    },
};

const BASE = __ENV.BASE_URL || 'https://localhost:7154';
const JSON_HEADERS = { headers: { 'Content-Type': 'application/json' } };

export let register200 = new Counter('register_200');
export let register400 = new Counter('register_400');

export let getByEmail200 = new Counter('getByEmail_200');
export let getByEmail404 = new Counter('getByEmail_404');

export let getByPhone200 = new Counter('getByPhone_200');
export let getByPhone404 = new Counter('getByPhone_404');

export let getById200 = new Counter('getById_200');
export let getById400 = new Counter('getById_400');

export let getAll200 = new Counter('getAllUsers_200');
export let getAll400 = new Counter('getAllUsers_400');

export let update200 = new Counter('updateUser_200');
export let update400 = new Counter('updateUser_400');

function think() {
    sleep(Math.random() * 0.5 + 0.5);
}

export default function () {
    // ■ create a fresh user via /auth/register
    const id = uuidv4();
    const email = `load+${id}@example.com`;
    const phone = id.slice(0, 10);
    const pw = 'P@ssw0rd123!';
    const secQ = 'pet?';
    const secA = 'cat';

    let res = http.post(
        `${BASE}/auth/register`,
        JSON.stringify({
            FirstName: 'Load',
            LastName: 'Tester',
            Email: email,
            Phone: phone,
            Password: pw,
            SecurityQuestion: secQ,
            SecurityAnswer: secA,
        }),
        JSON_HEADERS
    );
    if (res.status === 200) register200.add(1);
    else if (res.status === 400) register400.add(1);
    check(res, { 'register ⇒ 200': r => r.status === 200 });

    think();

    // ■ GET by email
    res = http.get(
        `${BASE}/api/users/search/email/${encodeURIComponent(email)}`  // GET /api/users/search/email/{email} :contentReference[oaicite:2]{index=2}&#8203;:contentReference[oaicite:3]{index=3}
    );
    const userObj = res.status === 200 ? res.json() : null;
    const userId = userObj?.id || userObj?.Id || '';
    if (res.status === 200) getByEmail200.add(1);
    else getByEmail404.add(1);
    check(res, {
        'getByEmail ⇒ 200': r => r.status === 200,
        'getByEmail ⇒ has id': r => !!userId,
    });

    think();

    // ■ GET by phone
    res = http.get(
        `${BASE}/api/users/search/phone/${encodeURIComponent(phone)}`
    );
    if (res.status === 200) getByPhone200.add(1);
    else getByPhone404.add(1);
    check(res, { 'getByPhone ⇒ 200': r => r.status === 200 });

    think();

    // ■ GET by id
    res = http.get(
        `${BASE}/api/users/search/id/${userId}`
    );
    if (res.status === 200) getById200.add(1);
    else getById400.add(1);
    check(res, { 'getById ⇒ 200': r => r.status === 200 });

    think();

    // ■ GET all users
    res = http.get(`${BASE}/api/users/search/All`);
    if (res.status === 200) getAll200.add(1);
    else getAll400.add(1);
    check(res, { 'getAll ⇒ 200': r => r.status === 200 });

    think();

    // ■ UPDATE user
    const newEmail = `upd+${id}@example.com`;
    res = http.put(
        `${BASE}/api/users/UpdateUser/${userId}`,
        JSON.stringify({
            Email: newEmail,
            Phone: phone,
            Password: pw,
            SecurityQuestion: secQ,
            SecurityAnswer: secA,
        }),
        JSON_HEADERS
    );
    if (res.status === 200) update200.add(1);
    else update400.add(1);
    check(res, { 'updateUser ⇒ 200': r => r.status === 200 });

    think();
}

export function handleSummary(data) {
    return {
        'usersreport.html': htmlReport(data, {
            title: 'Users Endpoints Load Test'
        }),
    };
}
