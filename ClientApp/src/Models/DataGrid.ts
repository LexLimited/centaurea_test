export namespace Models {
    export type DataGridValueType =
        | "Numeric"
        | "String"
        | "Email"
        | "Regex"
        | "Ref"
        | "SingleSelect"
        | "MultiSelect";

    export namespace Dto {

        export type DataGridDescriptor = {
            name: string,
            id: number,
        };

        export type DataGridFieldSignatureDto = {
            id: number,
            name: string,
            type: DataGridValueType,
            order: number,
            regexPattern?: string,
            referencedGridId?: number,
            options?: string[],
        };

        export type DataGridSignatureDto = {
            fields: DataGridFieldSignatureDto[],
        };

        export type DataGridValueDto = {
            id?: number,
            fieldId: number,
            rowIndex?: number,
            type: DataGridValueType,

            stringValue?: string,
            numericValue?: number,
            intValue?: number;
            intListValue?: number[],
        };

        export type DataGridRowDto = {
            items: DataGridValueDto[],
        };

        export type DataGridDto = {
            name: string,
            id?: number,
            signature: DataGridSignatureDto,
            rows: DataGridRowDto[],
        };

    } // Dto

    export type ApplicationUser = {
        userName: string,
        userId: string,
    };

    export type GridPermission = {
        gridId: number,
        userName: string,
    };

} // Models