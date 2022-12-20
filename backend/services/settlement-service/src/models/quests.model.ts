import fs from 'fs';

export class Quest {
    id: string;
    initiator: string;
}

export default class QuestsModel {

    data = new Map<string, Array<Quest>>();
    private constructor() { }


    async get(id: string): Promise<Array<Quest>> {

        //case one = in memory
        if (this.data[id]){
            console.log(this.data[id]);
            return this.data[id];
        }


        //case 2 - exists in file
        var fileData = await this.load(id);
        if (fileData != null) {
            this.data[id] = fileData;
            console.log(fileData);
            return fileData;
        }

        //case 3 - new user

        var newData = [
            {
                id: "Builder",
                initiator: "Character_Cyber_Male_01"
            },
        
            {
                id: "Intro",
                initiator: "Character_Junky_Female_01"
            }];

        this.data[id] = newData;
        await this.save(id, newData);

        console.log(newData);
        return newData;
    }

    async complete(id: string, questId: string) {
        var userData = await this.get(id);

        for (var i = userData.length; i >= 0; i--){
            if (userData[i].id == questId){
                userData.splice(i, 1);
                break;
            }
        }

        await this.save(id, userData);

        return userData;
    }

    async save(id: string, value: Array<Quest>): Promise<void> {
        if (!fs.existsSync("./data"))
            fs.mkdirSync("./data");

        var path = `./data/quest_${id}`;
        await fs.promises.writeFile(path, JSON.stringify(value, null, 2));
    }

    async load(id: string): Promise<Array<Quest>> {
        var path = `./data/quest_${id}`;
        return fs.existsSync(path) ? JSON.parse(await fs.promises.readFile(path, "utf-8")) : null;
    }

    private static _instance: QuestsModel;
    static get Instance(): QuestsModel {
        if (QuestsModel._instance == null)
            QuestsModel._instance = new QuestsModel();
        return QuestsModel._instance;
    }
}