import { BuildingType } from "./buildings.model";

export class BuildingConfig {
    public type: BuildingType;
    public width: number;
    public height: number;
    public prefab: string;
}

export default class ConfigModel {

    buildConfig: {
        width: number,
        height: number,
        buildings: Array<BuildingConfig>
    } = {
        width: 20,
        height: 18,

        buildings: [
            {
                type: BuildingType.Generator,
                width: 4,
                height: 4,
                prefab: "Buildings/Generator"
            },

            {
                type: BuildingType.Distributor,
                width: 2,
                height: 2,
                prefab: "Buildings/Distributor"
            }
        ]
    }

    private constructor() { }

    public  getBuildingConfig(type: BuildingType): BuildingConfig {
        return this.buildConfig.buildings.find(b => b.type == type);
    }

    private static _instance: ConfigModel;
    static get Instance(): ConfigModel {
        if (ConfigModel._instance == null)
            ConfigModel._instance = new ConfigModel();
        return ConfigModel._instance;
    }
}