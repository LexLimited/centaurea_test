export namespace Models {
    export type DataGridValueType = "Numeric" | "String" | "Email" | "Regex" | "Ref" | "SingleSelect" | "MultiSelect";

    export type DataGridFieldSignatureDto = {
        name: string,
        type: DataGridValueType,
        regexPattern?: string,
        referencedGridId?: number,
        optionTableId?: number,
    };

    export type DataGridSignatureDto = {
        fields: DataGridFieldSignatureDto[],
    };
    
    export type DataGridDto = {
        name: string,
        id?: number,
        signature: DataGridSignatureDto,
        rows: any[],
    };
}