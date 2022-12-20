

export default class CurrencyModel {

    value = {};


    private constructor() { }

    public get(id: string): number {
        if (!this.value[id])
            this.value[id] = 100;
        return this.value[id];
    }

    public set(id: string, value : number) {
        this.value[id] = value;
    }

    private static _instance: CurrencyModel;
    static get Instance(): CurrencyModel {
        if (CurrencyModel._instance == null)
            CurrencyModel._instance = new CurrencyModel();
        return CurrencyModel._instance;
    }
}