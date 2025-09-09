import http from 'k6/http';
import { uuidv4 } from 'https://jslib.k6.io/k6-utils/1.1.0/index.js';
import { check, sleep } from 'k6';
import { Counter } from 'k6/metrics';
import { htmlReport } from 'https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js';

export let options = {
  vus: 100,
  duration: '150s',
  thresholds: {
    http_req_duration: ['p(95)<500'],
    http_req_failed:   ['rate<0.01'],
  },
};

const BASE = __ENV.BASE_URL || 'https://localhost:7154';

// one counter for all errors, tagged by endpoint and message
export let errorCounter = new Counter('errors_total');

function logAndCountError(res, endpoint) {
  if (res.status !== 200) {
    // extract a friendly message (fall back to raw body)
    let msg = res.body;
    try {
      const j = JSON.parse(res.body);
      msg = j.message || j.Message || msg;
      const errs = j.errors || j.Errors;
      if (Array.isArray(errs) && errs.length) {
        msg += ' | ' + errs.join('; ');
      }
    } catch {
      // leave msg as-is (could be null or empty)
    }
    // ensure it's a string so tags are valid
    msg = String(msg);

    // immediate console log
    console.log(`❌ [VU=${__VU} ITER=${__ITER}] ${endpoint} ⇒ ${res.status} — ${msg}`);

    // increment counter with string tags only
    errorCounter.add(1, { endpoint, message: msg });
  }
}

export default function () {
  const id    = uuidv4();
  const email = `load+${id}@example.com`;
  const phone = uuidv4().slice(0, 10);
  const pw    = 'P@ssw0rd123!';
  const newPw = `New${uuidv4().slice(0,6)}!`;

  // 1) REGISTER
  let res = http.post(`${BASE}/auth/register`,
    JSON.stringify({
      FirstName:        'Load',
      LastName:         'Tester',
      Email:            email,
      Phone:            phone,
      Password:         pw,
      SecurityQuestion: 'fav?',
      SecurityAnswer:   'none',
    }),
    { headers: { 'Content-Type': 'application/json' } }
  );
  logAndCountError(res, 'register');
  check(res, { 'register': r => r.status === 200 });

  // 2) LOGIN
  res = http.post(`${BASE}/auth/login`,
    JSON.stringify({ Email: email, Password: pw }),
    { headers: { 'Content-Type': 'application/json' } }
  );
  logAndCountError(res, 'login');
  check(res, { 'login': r => r.status === 200 });

  // 3) SECURITY QUESTION
  res = http.get(`${BASE}/auth/security-question?email=${encodeURIComponent(email)}`);
  logAndCountError(res, 'security-question');
  check(res, { 'security-question': r => r.status === 200 });

  // 4) RESET PASSWORD
  res = http.post(`${BASE}/auth/reset-password`,
    JSON.stringify({
      Email:          email,
      SecurityAnswer: 'none',
      NewPassword:    newPw,
    }),
    { headers: { 'Content-Type': 'application/json' } }
  );
  logAndCountError(res, 'reset-password');
  check(res, { 'reset-password': r => r.status === 200 });

  // 5) LOGIN AFTER RESET
  res = http.post(`${BASE}/auth/login`,
    JSON.stringify({ Email: email, Password: newPw }),
    { headers: { 'Content-Type': 'application/json' } }
  );
  logAndCountError(res, 'login-after-reset');
  check(res, { 'login-after-reset': r => r.status === 200 });

  sleep(Math.random() * 0.5);
}

export function handleSummary(data) {
  return {
    'authsummary.html': htmlReport(data, { title: 'Auth Flow Load Test' }),
  };
}
