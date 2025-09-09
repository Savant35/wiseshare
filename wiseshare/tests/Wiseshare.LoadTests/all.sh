#!/usr/bin/env bash
set -euo pipefail

# disable k6's local REST API (so multiple instances don't conflict)
export K6_NO_SETUP=true

AUTH_SCRIPT="loadtest.js"
PROP_SCRIPT="loadtestproperty.js"
USER_SCRIPT="loadtestuser.js"

echo "🚀 Starting auth-flow load test..."
k6 run --address="" "$AUTH_SCRIPT" &   # <-- disable API server
echo "✅ Started auth-flow load test..."

sleep 5

echo "🚀 Starting property-flow load test..."
k6 run --address="" "$PROP_SCRIPT" &  
echo "✅ Started property-flow load test..."

sleep 5

echo "🚀 Starting user-flow load test..."
k6 run --address="" "$USER_SCRIPT" &  
echo "✅ Started user-flow load test..."

wait
echo "🏁 All load tests complete."
