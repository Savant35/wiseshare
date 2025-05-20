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
    let msg = res.body;
    try {
      const j = JSON.parse(res.body);
      msg = j.message || j.Message || msg;
      const errs = j.errors || j.Errors;
      if (Array.isArray(errs) && errs.length) {
        msg += ' | ' + errs.join('; ');
      }
    } catch {}
    msg = String(msg);
    console.log(`❌ [VU=${__VU} ITER=${__ITER}] ${endpoint} ⇒ ${res.status} — ${msg}`);
    errorCounter.add(1, { endpoint, message: msg });
  }
}

export default function () {
  // generate a fresh property payload
  const id = uuidv4();
  const name = `LoadProperty-${id}`;
  const address = `${id} Testing Blvd`;
  const location = 'LoadCity';
  const originalValue = Math.floor(Math.random() * 1e6) + 10000;
  const description = 'Property created for load test';

  // 1) CREATE
  let res = http.post(
    `${BASE}/api/property/create`,
    JSON.stringify({
      Name: name,
      Address: address,
      Location: location,
      OriginalValue: originalValue,
      Description: description,
    }),
    { headers: { 'Content-Type': 'application/json' } }
  );
  logAndCountError(res, 'create');
  check(res, { 'create': r => r.status === 200 });

  // parse out the new propertyId (some serializers use camelCase)
  let propertyId;
  try {
    const b = JSON.parse(res.body);
    propertyId = b.propertyId || b.PropertyId;
  } catch {
    return; // abort if create failed
  }

  // 2) GET BY ID
  res = http.get(`${BASE}/api/property/search/id/${propertyId}`);
  logAndCountError(res, 'getById');
  check(res, { 'getById': r => r.status === 200 });

  // 3) GET BY ADDRESS
  res = http.get(
    `${BASE}/api/property/search/address/${encodeURIComponent(address)}`
  );
  logAndCountError(res, 'getByAddress');
  check(res, { 'getByAddress': r => r.status === 200 });

  // 4) GET BY NAME
  res = http.get(
    `${BASE}/api/property/search/name/${encodeURIComponent(name)}`
  );
  logAndCountError(res, 'getByName');
  check(res, { 'getByName': r => r.status === 200 });

  // 5) GET BY LOCATION
  res = http.get(
    `${BASE}/api/property/search/location/${encodeURIComponent(location)}`
  );
  logAndCountError(res, 'getByLocation');
  check(res, { 'getByLocation': r => r.status === 200 });

  // GET ALL (commented out if too slow)
  // res = http.get(`${BASE}/api/property/all`);
  // logAndCountError(res, 'getAll');
  // check(res, { 'getAll': r => r.status === 200 });

  // 6) UPDATE
  const newDescription = description + ' (updated)';
  const newAvailableShares = 10000;
  res = http.put(
    `${BASE}/api/property/UpdateProperty${propertyId}`,
    JSON.stringify({
      Name: name,
      Description: newDescription,
      AvailableShares: newAvailableShares,
    }),
    { headers: { 'Content-Type': 'application/json' } }
  );
  logAndCountError(res, 'update');
  check(res, { 'update': r => r.status === 200 });

  sleep(Math.random() * 0.5);
}

export function handleSummary(data) {
  return {
    'propertysummary.html': htmlReport(data, {
      title: 'Property Endpoints Load Test',
    }),
  };
}
