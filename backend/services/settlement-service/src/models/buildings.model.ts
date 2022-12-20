import ConfigModel, { BuildingConfig } from "./config.model";
import fs from 'fs';

export enum BuildingType {
    Generator,
    Distributor
}

export class BuildingPlacement {
    public type: BuildingType;
    public x: number;
    public y: number;
}

export default class BuildingsModel {

    data = new Map<string, Array<BuildingPlacement>>();
    private constructor() { }


    async get(id: string): Promise<Array<BuildingPlacement>> {

        //case one = in memory
        if (this.data[id])
            return this.data[id];


        //case 2 - exists in file
        var fileData = await this.load(id);
        if (fileData != null) {
            this.data[id] = fileData;
            return fileData;
        }

        //case 3 - new user

        var newData = [{
            type: BuildingType.Generator,
            x: 6,
            y: 1
        }];

        this.data[id] = newData;
        await this.save(id, newData);

        return newData;
    }

    async add(id: string, value: BuildingPlacement) {
        var userData = await this.get(id);
        userData.push(value);
        this.save(id, userData);
    }

    async save(id: string, value: Array<BuildingPlacement>): Promise<void> {
        if (!fs.existsSync("./data"))
            fs.mkdirSync("./data");

        var path = `./data/${id}`;
        await fs.promises.writeFile(path, JSON.stringify(value, null, 2));
    }

    async load(id: string): Promise<Array<BuildingPlacement>> {
        var path = `./data/${id}`;
        return fs.existsSync(path) ? JSON.parse(await fs.promises.readFile(path, "utf-8")) : null;
    }

    public static getCellsData(data: Array<BuildingPlacement>) {

        var result = [];

        for (var i = 0; i < ConfigModel.Instance.buildConfig.width; i++) {
            result.push([]);
            for (var j = 0; j < ConfigModel.Instance.buildConfig.height; j++)
                result[i].push(0);
        }

        for (var building of data) {
            var config = ConfigModel.Instance.getBuildingConfig(building.type);

            for (var i = building.x - config.width / 2; i < building.x + config.width / 2; i++)
                for (var j = building.y - config.height / 2; j < building.y + config.height / 2; j++)
                    result[i | 0][j | 0] = 1;
        }

        return result;
    }

    public static checkIfFits(data: Array<BuildingPlacement>, building: BuildingPlacement, config: BuildingConfig): boolean {
        var cells = BuildingsModel.getCellsData(data);
        for (var i = building.x - config.width / 2; i < building.x + config.width / 2; i++)
            for (var j = building.y - config.height / 2; j < building.y + config.height / 2; j++)
                if (cells[i | 0][j | 0] != 0)
                    return false;

        return true;
    }

    private static _instance: BuildingsModel;
    static get Instance(): BuildingsModel {
        if (BuildingsModel._instance == null)
            BuildingsModel._instance = new BuildingsModel();
        return BuildingsModel._instance;
    }
}