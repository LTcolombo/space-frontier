import ConfigModel, { BuildingConfig } from "./config.model";
import BuildingsModel from "./buildings.model";



export class CharacterData {
    public prefab: string;
    //skills etc
}

export default class CharactersModel {
    async getUserCharacters(id: string) {
        var characters = [];

        var buildings = await BuildingsModel.Instance.get(id);
        for (var i = 0; i < buildings.length; i++)
            characters.push("Characters/Character_Cyber_Male_01")

        return characters;
    }

    private static _instance: CharactersModel;
    static get Instance(): CharactersModel {
        if (CharactersModel._instance == null)
            CharactersModel._instance = new CharactersModel();
        return CharactersModel._instance;
    }
}