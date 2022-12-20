import express, { Response } from "express";
import BuildingsModel, { BuildingPlacement } from "../models/buildings.model";
import ConfigModel from "../models/config.model";

export const characterRoutes = express.Router();

characterRoutes.get("/:id", async (req: { params: { id: string } }, res: Response) => {
    try {

        var result = [];

        result.push("Character_Cyber_Male_01")


        res.status(200).send(result);
    } catch (e: any) {
        console.error(e);
        res.status(500).send(e.message);
    }
});