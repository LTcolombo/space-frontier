mergeInto(LibraryManager.library, {
    SignIn: async function()
    {
        //configure minimal network settings and key storage
        const config = {
            networkId: "testnet",
            nodeUrl: "https://rpc.testnet.near.org",
            walletUrl: "https://wallet.testnet.near.org",
            helperUrl: "https://helper.testnet.near.org",
            explorerUrl: "https://explorer.testnet.near.org",
            deps: {
                keyStore: new nearApi.keyStores.BrowserLocalStorageKeyStore(),
            }
        };
        // open a connection to the NEAR platform
        window.near = await nearApi.connect(config);

        // ---------------------------------------------------------------------------
        // here you have access to `near-api-js` and a valid connection object `near`
        // ---------------------------------------------------------------------------

        const wallet = new nearApi.WalletConnection(window.near, 'beeswax');
        if (!wallet.isSignedIn())
            wallet.requestSignIn(
                "nickbalyanitsa.testnet", // contract requesting access
                "BeesWax"
            );
        else {

            SendMessage("Bootstrap", "HandleWalletId", wallet.getAccountId());
        }
    },
    Deposit: async function(val)
    {
        //configure minimal network settings and key storage
        const config = {
            networkId: "testnet",
            nodeUrl: "https://rpc.testnet.near.org",
            walletUrl: "https://wallet.testnet.near.org",
            helperUrl: "https://helper.testnet.near.org",
            explorerUrl: "https://explorer.testnet.near.org",
            deps: {
                keyStore: new nearApi.keyStores.BrowserLocalStorageKeyStore(),
            }
        };
        // open a connection to the NEAR platform
        window.near = await nearApi.connect(config);

        // ---------------------------------------------------------------------------
        // here you have access to `near-api-js` and a valid connection object `near`
        // ---------------------------------------------------------------------------

        const wallet = new nearApi.WalletConnection(window.near, 'beeswax');
        if (!wallet.isSignedIn())
            wallet.requestSignIn(
                "nickbalyanitsa.testnet", // contract requesting access
                "BeesWax"
            );
        else {
            wallet.account().sendMoney("nickbalyanitsa.testnet", val);
        }

    }
});