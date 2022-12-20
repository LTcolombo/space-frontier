
import { keyStores, KeyPair, connect, Contract } from "near-api-js";
import { randomBytes } from 'crypto';
import BN from 'bn.js';

function getAccountPrivateKey(accountPrefix: string) {


    if (accountPrefix.indexOf(process.env.DIME_ACCOUNT_PREFIX) >= 0)
        accountPrefix = process.env.DIME_ACCOUNT_PREFIX;

    if (accountPrefix.indexOf(process.env.AVATAR_ACCOUNT_PREFIX) >= 0)
        accountPrefix = process.env.AVATAR_ACCOUNT_PREFIX;

    var envKey = "KEY_" + accountPrefix.replaceAll(".", "_").toUpperCase();
    console.log(envKey);
    if (process.env[envKey])
        return KeyPair.fromString(process.env[envKey]);

    console.error("Keypair not found for", accountPrefix);
    return null;
}

export async function updateAvatar(id: string, {key, value}: any){
    return await callFunc(
        `${process.env.AVATAR_ACCOUNT_PREFIX}.${process.env.DOMAIN}.${process.env.ROOT_ACCOUNT}`,
        `${process.env.AVATAR_ACCOUNT_PREFIX}.${process.env.DOMAIN}.${process.env.ROOT_ACCOUNT}`,
        "nft_append_numeric_attribute",
        {
            "token_id": id,
            "attribute": key,
            value
        },
        "30000000000000000000000",
        "300000000000000"
    );
}

export async function mintAvatar(metadata, receiverAddress) {
    return await callFunc(
        `${process.env.AVATAR_ACCOUNT_PREFIX}.${process.env.DOMAIN}.${process.env.ROOT_ACCOUNT}`,
        `${process.env.AVATAR_ACCOUNT_PREFIX}.${process.env.DOMAIN}.${process.env.ROOT_ACCOUNT}`,
        "nft_mint",
        {
            "token_id": randomBytes(12).toString('hex'),
            "receiver_id": receiverAddress,
            "metadata": metadata
        },
        "30000000000000000000000",
        "300000000000000"
    );

}

export async function getContract(accountPrefix: string, viewMethods: string[], changeMethods: string[] | undefined): Promise<any> {

    if (!changeMethods)
        changeMethods = [];

    const ACCOUNT_ID = `${accountPrefix}.${process.env.DOMAIN}.${process.env.ROOT_ACCOUNT}`;

    const keyStore = new keyStores.InMemoryKeyStore();
    keyStore.setKey(process.env.NETWORK_ID, ACCOUNT_ID, getAccountPrivateKey(accountPrefix));

    const near = await connect({
        networkId: process.env.NETWORK_ID,
        keyStore,
        nodeUrl: (changeMethods.length == 0) ?
            `https://${process.env.ARCHIVAL_RPC_API_SUBDOMAIN}.${process.env.NETWORK_ID}.near.org/` :
            `https://${process.env.RPC_API_SUBDOMAIN}.${process.env.NETWORK_ID}.near.org/`,
        headers: { 'x-api-key': process.env.API_KEY },
    });

    const account = await near.account(ACCOUNT_ID);

    return new Contract(
        account, // the account object that is connecting
        ACCOUNT_ID,
        {
            viewMethods, // view methods do not change state but usually return a value
            changeMethods // change methods modify state
        }
    );
}

export async function createAccount(custodialId, userId) {
    const dimeMainAccountId = `${process.env.DIME_ACCOUNT_PREFIX}.${process.env.DOMAIN}.${process.env.ROOT_ACCOUNT}`;

    const memkeyStore = new keyStores.InMemoryKeyStore();
    const dimeKeypair = getAccountPrivateKey(process.env.DIME_ACCOUNT_PREFIX);
    memkeyStore.setKey(process.env.NETWORK_ID, custodialId, dimeKeypair);
    memkeyStore.setKey(process.env.NETWORK_ID, dimeMainAccountId, dimeKeypair);

    var near = await connect({
        networkId: process.env.NETWORK_ID,
        keyStore: memkeyStore,
        nodeUrl: `https://${process.env.RPC_API_SUBDOMAIN}.${process.env.NETWORK_ID}.near.org/`,
        headers: { 'x-api-key': process.env.NEAR_RPC_API_KEY },
    });

    var txs = [];
    //Create account
    try {
        const masterAccount = await near.account(`${process.env.DIME_ACCOUNT_PREFIX}.${process.env.DOMAIN}.${process.env.ROOT_ACCOUNT}`);
        const response = await masterAccount.createAccount(custodialId, dimeKeypair.getPublicKey(), new BN('500000000000000000000000'));
        txs.push(response.transaction.hash);
        console.log(`Account ${custodialId} for network "${process.env.NETWORK_ID}" was created.`);

    } catch (error) {
        if (error.type === 'RetriesExceeded') {
            console.warn('Received a timeout when creating account, please run:');
            console.warn('to confirm creation. Keyfile for this account has been saved.');
        } else {
            throw error;
        }
    }

    txs.push(await depositStorage(userId, "150000000000000000000000"));
    txs.push(await depositStorage(custodialId, "150000000000000000000000"));
    return txs;
}


async function depositStorage(receiver, amount) {

    const ftAccount = `${process.env.DIME_ACCOUNT_PREFIX}.${process.env.DOMAIN}.${process.env.ROOT_ACCOUNT}`;
    return await callFunc(
        ftAccount,
        ftAccount,
        "storage_deposit",
        { "account_id": receiver },
        amount);
}



export function getCustodialAccountIDFromWalletId(accountId) {

    const dimeMainAccountId = `${process.env.DIME_ACCOUNT_PREFIX}.${process.env.DOMAIN}.${process.env.ROOT_ACCOUNT}`;

    if (accountId.indexOf(dimeMainAccountId) >= 0)
        return accountId;

    var parts = accountId.split('.');

    if (parts.length > 1)
        parts.splice(parts.length - 1, 1);

    parts.push(dimeMainAccountId);
    return parts.join('.');
}

export async function getAccountState(id) {
    const keyStore = new keyStores.InMemoryKeyStore();
    const dimeKeypair = getAccountPrivateKey(`${process.env.DIME_ACCOUNT_PREFIX}`);
    keyStore.setKey(process.env.NETWORK_ID, id, dimeKeypair);

    var near = await connect({
        networkId: process.env.NETWORK_ID,
        keyStore,
        nodeUrl: `https://${process.env.RPC_API_SUBDOMAIN}.${process.env.NETWORK_ID}.near.org/`,
        headers: { 'x-api-key': process.env.NEAR_RPC_API_KEY },
    });

    const account = await near.account(id);
    return await account.state();
}

export async function sendDime(amount, sender, receiver) {

    var senderWallet = getCustodialAccountIDFromWalletId(sender);
    var receiverWallet = getCustodialAccountIDFromWalletId(receiver);

    console.warn("sending [custodial]", amount, senderWallet, "->", receiverWallet);

    return await callFunc(
        senderWallet,
        `${process.env.DIME_ACCOUNT_PREFIX}.${process.env.DOMAIN}.${process.env.ROOT_ACCOUNT}`,
        "ft_transfer",
        {
            receiver_id: receiverWallet,
            amount: amount.toString()
        },
        "1");
};

const callFunc = async (signerId, contractId, methodName, args, attachedDeposit, gas = new BN('300000000000000')) => {
    const keyStore = new keyStores.InMemoryKeyStore();
    await keyStore.setKey(process.env.NETWORK_ID, signerId, getAccountPrivateKey(signerId));

    const rpcConfig = {
        networkId: process.env.NETWORK_ID,
        keyStore,
        nodeUrl: `https://${process.env.RPC_API_SUBDOMAIN}.${process.env.NETWORK_ID}.near.org/`,
        headers: { 'x-api-key': process.env.NEAR_RPC_API_KEY },
    };

    const near = await connect(rpcConfig);
    const account = await near.account(signerId);
    const response = await account.functionCall({ contractId, methodName, args, gas, attachedDeposit });

    console.log(response);

    return response?.transaction?.hash;
}