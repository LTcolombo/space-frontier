import express, { Request, Response } from "express";
import QuestsModel from "../models/quests.model";

export const questsRoutes = express.Router();

questsRoutes.get("/:id", async (req: Request, res: Response) => {
    try {
        res.status(200).send(await QuestsModel.Instance.get(req.params.id));
    } catch (e: any) {
        console.error(e);
        res.status(500).send(e.message);
    }
});



questsRoutes.post("/:id", async (req: Request, res: Response) => {
    try {
        res.status(200).send(QuestsModel.Instance.complete(req.params.id, req.body.id));
    } catch (e: any) {
        console.error(e);
        res.status(500).send(e.message);
    }
});
