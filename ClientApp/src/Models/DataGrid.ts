export namespace Models {
    export type DataGridValueType = "Numeric" | "String" | "Email" | "Regex" | "Ref" | "SingleSelect" | "MultiSelect";

    export namespace Dto {

        export type DataGridDescriptor = {
            name: string,
            id: number,
        };

        export type DataGridFieldSignatureDto = {
            name: string,
            type: DataGridValueType,
            regexPattern?: string,
            referencedGridId?: number,
            optionId?: number,
        };

        export type DataGridSignatureDto = {
            fields: DataGridFieldSignatureDto[],
        };

        export type DataGridRowItemDto = {
            id: number,
            fieldId: number,
            rowIndex: number,
            stringValue?: string,
            numericValue?: number,
            referencedFieldId?: number,
            optionId?: number,
            optionIds?: number[],
        }
        // & (
        //     | { stringValue: string }
        //     | { numericValue: number }
        //     | { referencedFieldId: number }
        //     | { optionId: number }
        //     | { optionIds: number[] }
        // );

        export type DataGridRowDto = {
            items: DataGridRowItemDto[],
        };

        export type DataGridDto = {
            name: string,
            id?: number,
            signature: DataGridSignatureDto,
            rows: DataGridRowDto[],
        };

    } // Dto

} // Models