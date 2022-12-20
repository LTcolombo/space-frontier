import express, { Request, Response } from "express";
import { createAccount, getAccountState, getContract, getCustodialAccountIDFromWalletId, sendDime } from "../helpers/near-connection.helper";

export const ftRoutes = express.Router();

async function getBalanceFor(custodialId: string, id: string) {
    var state = null;
    try {
        state = await getAccountState(custodialId)
    }
    catch (e) {
        console.error(e);
    }

    console.log("state", state);

    if (!state) {
        var balance = 10; //initial balance
        var accountCreationTransactions = await createAccount(custodialId, id);
        console.log(`${custodialId} creation transactions:`, accountCreationTransactions);

        await sendDime(balance, `${process.env.DIME_ACCOUNT_PREFIX}.${process.env.DOMAIN}.${process.env.ROOT_ACCOUNT}`, custodialId);
        return balance;
    }
    else
        return parseInt(await (await getContract("dime", ["ft_balance_of"], null)).ft_balance_of({ account_id: custodialId }));
}

ftRoutes.get("/:id", async (req: Request, res: Response) => {
    try {
        var custodialId = getCustodialAccountIDFromWalletId(req.params.id)
        res.json(await getBalanceFor(custodialId, req.params.id)).status(200);
    } catch (e: any) {
        console.error(e);
        res.status(500).send(e.message);
    }
});


ftRoutes.post("/:id", async (req: Request, res: Response) => {
    try {
        var custodialId = getCustodialAccountIDFromWalletId(req.params.id)
        let balance = await getBalanceFor(custodialId, req.params.id);

        if (req.body.value < 0 && balance + req.body.value > 0)
            sendDime(-req.body.value, custodialId, `${process.env.DIME_ACCOUNT_PREFIX}.${process.env.DOMAIN}.${process.env.ROOT_ACCOUNT}`);
        else
            sendDime(req.body.value, `${process.env.DIME_ACCOUNT_PREFIX}.${process.env.DOMAIN}.${process.env.ROOT_ACCOUNT}`, custodialId);


        res.json(balance + req.body.value).status(200);
    } catch (e: any) {
        console.error(e);
        res.status(500).send(e.message);
    }
});

