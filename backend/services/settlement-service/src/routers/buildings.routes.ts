import express, { Response } from "express";
import BuildingsModel, { BuildingPlacement } from "../models/buildings.model";
import CharactersModel from "../models/characters.model";
import ConfigModel from "../models/config.model";

export const buildingsRoutes = express.Router();

buildingsRoutes.get("/:id", async (req: { params: { id: string } }, res: Response) => {
    try {
        var buildings = await BuildingsModel.Instance.get(req.params.id);
        console.log(buildings);


        res.status(200).send({
            cells: BuildingsModel.getCellsData(buildings),
            buildings,
            characters: await CharactersModel.Instance.getUserCharacters(req.params.id)
        });
    } catch (e: any) {
        console.error(e);
        res.status(500).send(e.message);
    }
});


buildingsRoutes.post("/:id", async (req: { params: { id: string }, body: BuildingPlacement }, res: Response) => {
    try {
        var buildings = await BuildingsModel.Instance.get(req.params.id);

        if (!BuildingsModel.checkIfFits(buildings, req.body, ConfigModel.Instance.getBuildingConfig(req.body.type))) {
            res.status(500).send(false);
            return;
        }

        await BuildingsModel.Instance.add(req.params.id, req.body);

        res.status(200).send({
            cells: BuildingsModel.getCellsData(buildings),
            buildings,
            characters: await CharactersModel.Instance.getUserCharacters(req.params.id)
        });
    } catch (e: any) {
        console.error(e);
        res.status(500).send(e.message);
    }
});