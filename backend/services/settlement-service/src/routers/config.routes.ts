import express, { Request, Response } from "express";
import ConfigModel from "../models/config.model";

export const configRoutes = express.Router();

configRoutes.get("/build", async (req: Request, res: Response) => {
    try {
        res.status(200).send(ConfigModel.Instance.buildConfig);
    } catch (e: any) {
        console.error(e);
        res.status(500).send(e.message);
    }
});