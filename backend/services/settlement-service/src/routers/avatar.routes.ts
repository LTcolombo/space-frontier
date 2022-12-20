import express, { Request, Response } from "express";

import { getContract, mintAvatar, updateAvatar } from "../helpers/near-connection.helper";

export const avatarRoutes = express.Router();

avatarRoutes.get("/:id", async (req: Request, res: Response) => {
    try {
        var nearAddress = req.params.id;
        var contract = await getContract('avatar', ["nft_tokens_for_owner"], null);
        var data = await contract.nft_tokens_for_owner({ account_id: nearAddress });


        if (data.length > 0)
            res.status(200).send(data[data.length - 1].metadata);
        else {

            let cid = `bafkreie2tmhddv4fhf6gxyz5e5xoydilktvunfxkcy6ikmi2cknyczhorq.ipfs.nftstorage.link`;

            var metadata = {
                "title": 'Avatar',
                "description": `Avatar of ${nearAddress}`,
                "media": cid,
                "issued_at": new Date().toJSON(),
                "extra": JSON.stringify({
                    attributes: [{
                        trait_type: "karma",
                        value: 5
                    }]
                })
            };

            var mintResult = await mintAvatar(metadata, nearAddress);
            console.log(mintResult);
            res.status(200).send(metadata);
        }
    } catch (e: any) {
        console.error(e);
        res.status(500).send(e.message);
    }
});




avatarRoutes.post("/:id", async (req: Request, res: Response) => {
    try {


        var nearAddress = req.params.id;
        var contract = await getContract('avatar', ["nft_tokens_for_owner"], null);
        var data = await contract.nft_tokens_for_owner({ account_id: nearAddress });
        if (data.length < 1) {
            res.status(401).send("not found");
            return;
        }

        var updateResult = await updateAvatar(data[data.length-1].token_id, req.body);
        console.log(updateResult);
        res.status(200).send(updateResult);

    } catch (e: any) {
        console.error(e);
        res.status(500).send(e.message);
    }
});