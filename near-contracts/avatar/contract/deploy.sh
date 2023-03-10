#!/bin/sh

./build.sh

if [ $? -ne 0 ]; then
  echo ">> Error building contract"
  exit 1
fi

echo ">> Deploying contract"

near deploy --accountId avatar.frontiergame.testnet  --wasmFile build/avatar.wasm