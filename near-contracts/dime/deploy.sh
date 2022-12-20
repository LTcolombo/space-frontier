#!/bin/sh

./build.sh

if [ $? -ne 0 ]; then
  echo ">> Error building contract"
  exit 1
fi

echo ">> Deploying contract"

near deploy --accountId dime.frontiergame.testnet  --wasmFile target/wasm32-unknown-unknown/release/dime_frontier.wasm