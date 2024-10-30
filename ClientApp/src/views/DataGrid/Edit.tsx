import * as React from 'react';
import { DataGrid, GridColDef, GridCellEditStopParams, GridValidRowModel, GridColumnMenuProps, ColumnMenuPropsOverrides, useGridApiRef, useGridApiContext, GridCellParams } from '@mui/x-data-grid';
import { Button, Grid } from '@mui/material';
import { Models } from '@/Models/DataGrid';
import { CentaureaApi } from '@/api/CentaureaApi';
import { NewFieldNameInput } from './NewFieldNameInput';
import { UserPermissionSelector } from './UserPermissionSelector';

function GridContainer({ children, style }: any) {
    return (
        <div style={{ height: 400, width: '100%', ...style }}>
            {children}
        </div>
    )
}

enum Stage {
    SelectingGrid,
    GridLoaded,
    Exception,
};

function GridsList({
    gridDescriptors,
    onSelect,
}: {
    gridDescriptors: Models.Dto.DataGridDescriptor[],
    onSelect?: (gridId: number) => void,
}) {
    const [selectedId, setSelectedId] = React.useState<number | undefined>();

    if (!gridDescriptors.length) {
        return (
            <GridContainer>No grids found</GridContainer>
        )
    }

    const options = gridDescriptors.map(descriptor => (
        <CButton
            key={descriptor.id}
            disabled={selectedId == descriptor.id}
            onClick={() => {
                setSelectedId(descriptor.id);
                onSelect?.(descriptor.id)
            }}
            style={{ width: 175 }}
        >
            {JSON.stringify(descriptor.name)}
        </CButton>
    ));

    return (
        <GridContainer>
            <div style={{ padding: 10, width: 100, display: 'flex', flexDirection: 'column' }}>{options}</div>
        </GridContainer>
    )
}

function ExceptionView({ details }: { details: string }) {
    return (
        <GridContainer>
            <p>Something went wrong</p>
            <p>Details: '{details}'</p>
        </GridContainer>
    )
}

const CButton = ({children, style, ...props}: any) => {
    return <Button style={{padding: 10, margin: 5, ...style}} {...props}>{children}</Button>
}

interface CentaureaGridRowModel extends GridValidRowModel {
    id: number,
}

function extractValueDtoValue(dtoValue?: Models.Dto.DataGridValueDto) {
    if (dtoValue == undefined) {
        throw "extractValueDtoValue given an undefined value";
    }

    switch (dtoValue!.type) {
        case 'Numeric': return dtoValue.numericValue;
        case 'String': return dtoValue.stringValue;
        case 'Email': return dtoValue.stringValue;
        case 'Regex': return dtoValue.stringValue;
        case 'Ref': return dtoValue.intValue;
        case 'SingleSelect': return dtoValue.intValue;
        case 'MultiSelect': return dtoValue.intListValue;
    };
}

function setValueDtoValue(dtoValue: Models.Dto.DataGridValueDto, value: string) {
    switch (dtoValue!.type) {
        case 'Numeric': dtoValue.numericValue = Number.parseFloat(value);
            break;
        case 'String': dtoValue.stringValue = value;
            break;
        case 'Email': dtoValue.stringValue = value;
            break;
        case 'Regex': dtoValue.stringValue = value;
            break;
        case 'Ref': dtoValue.intValue = Number.parseInt(value);
            break;
        case 'SingleSelect': dtoValue.intValue = Number.parseInt(value);
            break;
        case 'MultiSelect':
            let optArray = value.split(',').map(Number.parseInt);
            dtoValue.intListValue = optArray;
            console.log('optArray:', optArray);
            break;
    };
}

type ValidateValueDtoResult = {
    ok: boolean,
    message?: string,
};

function validateValueDto(dtoValue: Models.Dto.DataGridValueDto, stringifiedValue?: string): ValidateValueDtoResult {
    const resultOk = (message?: string) =>  {
        return { ok: true, message };
    }

    const resultErr = (message?: string) => {
        return { ok: false, message };
    }

    const validateNumeric = () => {
        return Number.isNaN(stringifiedValue)
            ? resultErr(`${stringifiedValue} is not a valid number`)
            : resultOk();
    };

    const validateEmail = () => {
        const EMAIL_REGEX_STRING = /^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/g;

        return EMAIL_REGEX_STRING.test(stringifiedValue!)
            ? resultOk() : resultErr(`${stringifiedValue} is not a valid email`);
    };

    const validateInt = () => {
        return Number.isInteger(stringifiedValue)
            ? resultOk() : resultErr(`${stringifiedValue} is not an integer`);
    };

    if (typeof(stringifiedValue) !== 'string') {
        return resultErr(`expected string as stringified value, got ${typeof(stringifiedValue)}`);
    }

    switch (dtoValue.type) {
        case 'Numeric': return validateNumeric()
        case 'String': return resultOk();
        case 'Email': return validateEmail();
        // TODO! Implement this case
        case 'Regex': throw "NotImplemented: regex validated is not yet supported";
        case 'Ref': return validateInt();
        case 'SingleSelect': return validateInt();
        // TODO! Implement this case
        case 'MultiSelect': throw "NotImplemented: setValueDtoValue for type 'MultiSelect' is not yet supported";
    }
}

function ErrorPanel({
    message
}: {
    message?: string
}) {
    return (
        <div style={{ padding: 10 }}>
            Errors:
            <div style={{ padding: 6, marginLeft: 6 }}>
                { message || 'No errors' }
            </div>
        </div>
    );
}

function createValueDto(fieldId: number, type: Models.DataGridValueType, stringifiedValue: string) {
    const newValueDto: Models.Dto.DataGridValueDto = {
        fieldId,
        type,
    };

    setValueDtoValue(newValueDto, stringifiedValue);
    return newValueDto;
}

function GridView({
    gridDto,
    
    // TODO! Get rid of these
    gridName,
    dtoFields,
    dtoRows,
}: {
    gridDto: Models.Dto.DataGridDto,
    gridName?: string,
    dtoFields: Models.Dto.DataGridDto['signature']['fields'],
    dtoRows: Models.Dto.DataGridDto['rows'],
}) {
    const apiRef = useGridApiRef();

    const [errorMessage, setErrorMessage] = React.useState<string>();

    const [rows, setRows] = React.useState<CentaureaGridRowModel[]>(dtoRows.map(mapDtoRowToGridRow));

    const [columns, setColumns] = React.useState<GridColDef<CentaureaGridRowModel>[]>(dtoFields.map(mapDtoFieldsToGridColumn));

    const editedRowSnapshot = React.useRef<CentaureaGridRowModel>();

    const editedColumnId = React.useRef<number>();

    const FIELD_IDS = dtoFields.map(dtoField => dtoField.id);

    function deleteGrid() {
        // TODO! Stop reloading the window
        CentaureaApi.deleteGrid(gridDto.id!)
            .then(() => window.location.reload());
    }

    function mapDtoFieldsToGridColumn(dtoField: Models.Dto.DataGridDto['signature']['fields'][0]): GridColDef<CentaureaGridRowModel> {
        return {
            field: String(dtoField.id),
            editable: true,
            headerName: dtoField.name,
        };
    }

    function calculateMaxRowIndex(): number | undefined {
        let ret = -Infinity;
        for (let dtoRow of dtoRows)
        {
            const maxIndex = !!dtoRow.items.length ? Math.max(...dtoRow.items.map(item => item.rowIndex!)) : -Infinity;
            ret = Math.max(ret, maxIndex);
        }

        return Number.isFinite(ret) ? ret : undefined;
    }

    function mapDtoRowToGridRow(dtoRow: Models.Dto.DataGridRowDto, idx: number): CentaureaGridRowModel {
        // Id is the row index
        // If the row is virtual (doesn't exist in db), the index is undefined
        // Otherwise it has an item and any item's rowIndex is the index
        let id = dtoRow.items[0]?.rowIndex;

        // If the index is undefined, increment max id
        if (id == undefined) {
            // If the max row index is undefined, there are no values is the grid, default to 0
            id = calculateMaxRowIndex() || 0;
        }

        const ret: CentaureaGridRowModel = { id };

        for (let dtoField of dtoFields) {
            const fieldId = dtoField.id;

            const dtoValue = dtoRow.items.find(item => item.fieldId == fieldId);

            // If the value is undefined (not in db), leave it blank
            if (!!dtoValue) {
                ret[fieldId] = extractValueDtoValue(dtoValue);
            }
        }

        return ret;
    }

    function renderColumnMenu(props?: GridColumnMenuProps & ColumnMenuPropsOverrides) {
        const [renamingField, setRenamingField] = React.useState<boolean>(false);

        const FIELD_ID = Number.parseInt(props!.colDef.field);

        const onDeleteClick = () => {
            console.log('OnDeleteField clicked: props: ', props);

            // TODO! Stop reloading the window
            CentaureaApi.deleteField(FIELD_ID)
                .then(() => window.location.reload());
        };

        const onRenameClick = () => {
            setRenamingField(true);
        };

        if (renamingField) {
            return (
                <NewFieldNameInput
                    onCommit={async (newName) => {
                        // TODO! Stop reloading the window
                        CentaureaApi.renameField(FIELD_ID, newName)
                            .then(() => window.location.reload());
                    }}
                    onFinished={() => {
                        setRenamingField(false);
                    }}
                />
            )
        }

        return (
            <div style={{ display: 'flex', flexDirection: 'column' }}>
                <CButton onClick={onDeleteClick}>Delete</CButton>
                <CButton onClick={onRenameClick}>Rename</CButton>
            </div>
        )
    }

    if (!columns) {
        return <GridContainer>Grid has no fields</GridContainer>
    }

    const actionColumn: any = {
        field: 'actions',
        headerName: 'Column actions',
        width: 175,
        sortable: false,
        filterable: false,
        renderCell: (params: GridCellParams) => {
            return (
                <CButton
                    style={{ background: 'rgba(255, 0, 0, 0.25)' }}
                    onClick={() => {
                        console.log('params:', params);
                        CentaureaApi.deleteRow(gridDto.id!, Number.parseInt(`${params.id}`))
                            .then(() => window.location.reload());
                    }}
                >
                    Delete row
                </CButton>
            )
        }
    };

    return (
        <GridContainer>
            { gridName ? <h1>Grid name: {gridName}</h1> : null }
            <DataGrid
                onColumnHeaderClick={(props) => {

                }}
                slots={{
                    columnMenu: renderColumnMenu,
                }}
                rows={rows}
                columns={[...columns, actionColumn]}
                onCellClick={(params) => {
                    editedColumnId.current = Number.parseInt(params.colDef.field);
                }}
                onCellEditStop={(params) => {
                    // params.api.stopCellEditMode({ id: params.id, field: params.field });
                    console.log('onCellEditStop params:', params);
                }}
                onCellEditStart={(params) => {
                    editedRowSnapshot.current = params.row;
                }}

                // TODO! Fix this async error
                processRowUpdate={async (updatedRow, _, params) => {
                    // Send update to the backend and change the fieldId to the actual value
                    // rowId is an index
                    
                    const handleParams: CentaureaApi.CentaureaApiPutValueHandleExtraParams = {
                        fieldId: editedColumnId.current!,
                        rowIndex: updatedRow.id,
                    }

                    const valueRowDto = dtoRows.find(row => {
                        console.log('[Looking for value row dto] Looking in row', row);

                        return row.items.some(
                            item => item.fieldId == handleParams.fieldId
                                    && item.rowIndex == handleParams.rowIndex
                        );
                    });

                    let valueDto = valueRowDto?.items.find(
                        item => item.fieldId == handleParams.fieldId
                                && item.rowIndex == handleParams.rowIndex
                    );

                    const stringifiedValue = updatedRow[handleParams.fieldId!];

                    // If value dto does not exist, create a new one
                    if (valueDto == undefined) {
                        const newValueType = dtoFields.find(field => field.id == handleParams.fieldId)?.type;
                        if (!newValueType) {
                            throw `Failed to find for the value in field ${handleParams.fieldId}`;
                        }

                        valueDto = createValueDto(
                            handleParams.fieldId,
                            newValueType!,
                            stringifiedValue,
                        );

                        try {
                            console.log('Dto value:', valueDto);
                            const resValue = (await CentaureaApi.postValue(valueDto, handleParams))?.data;

                            return {
                                ...editedRowSnapshot.current!,
                                [handleParams.fieldId!]: extractValueDtoValue(resValue)
                            };
                        } catch (e) {
                            console.error('Api request failed:', e);
                        }
                    }

                    const validationResult = validateValueDto(valueDto);
                    if (!validationResult.ok) {
                        setErrorMessage(validationResult.message);
                    }

                    setValueDtoValue(valueDto, stringifiedValue);

                    // Update the row cell according to the data that came from the backend
                    // And return it
                    try {
                        console.log('Dto value:', valueDto);
                        const resValue = (await CentaureaApi.putValue(valueDto, handleParams))?.data;

                        return {
                            ...editedRowSnapshot.current!,
                            [handleParams.fieldId!]: extractValueDtoValue(resValue)
                        };
                    } catch (e) {
                        console.error('Api request failed:', e);
                    }

                    // This will return whatever the user put in
                    return editedRowSnapshot.current!;
                }}
                onProcessRowUpdateError={err => {
                    console.error('Row update process failed:', err);
                }}
            />
            <CButton onClick={() => console.log('TODO! Add new column')}>
                Add New Column
            </CButton>
            <CButton onClick={() => { setRows([...rows, { id: (calculateMaxRowIndex() || 0) + 1 }]) }}>
                Add New Row
            </CButton>
            <CButton
                style={{ backgroundColor: 'rgba(255, 0, 0, 0.25)' }}
                onClick={deleteGrid}
            >
                Delete grid
            </CButton>
            <ErrorPanel message={errorMessage}/>
            <UserPermissionSelector gridId={gridDto.id!}/>
        </GridContainer>
    );
}

export function Edit() {
    const [gridDescriptors, setGridDescriptors] = React.useState<Models.Dto.DataGridDescriptor[]>([]);

    const [exceptionDetails, setExceptionDetails] = React.useState<string>('');

    const [stage, setStage] = React.useState<Stage>(Stage.SelectingGrid);

    const [gridDto, setGridDto] = React.useState<Models.Dto.DataGridDto>();

    const [dtoRows, setDtoRows] = React.useState<Models.Dto.DataGridDto['rows']>([]);

    const [dtoFields, setDtoFields] = React.useState<Models.Dto.DataGridDto['signature']['fields']>([]);

    function setExceptionStage(e: any) {
        setExceptionDetails(`${e}`);
        setStage(Stage.Exception);
    }

    async function fetchGridDescriptors() {
        try {
            setGridDescriptors((await CentaureaApi.getGridDescriptors()).data);
            setStage(Stage.SelectingGrid);
        } catch (e) {
            console.error(`Failed to fetch grid descriptors: ${e}`);
            setExceptionStage(e);
        }
    }

    async function fetchGridDto(gridId: number) {
        console.log(`Fetching grid dto for gridId ${gridId}`);

        try {
            const gridDto = (await CentaureaApi.getGrid(gridId)).data;
            setGridDto(gridDto);
            // TODO! Get rid of these
            setDtoRows(gridDto.rows);
            setDtoFields(gridDto.signature.fields);
            setStage(Stage.GridLoaded);
        } catch (e) {
            console.error(`Failed to fetch grid dto: ${e}`);
            setExceptionStage(e);
        }
    }

    React.useEffect(() => {
        fetchGridDescriptors();
    }, []);

    if (stage == Stage.SelectingGrid) {
        return (
            <GridsList
                gridDescriptors={gridDescriptors}
                onSelect={fetchGridDto}
            />
        )
    }

    if (stage == Stage.Exception) {
        return <ExceptionView details={exceptionDetails} />
    }

    if (stage == Stage.GridLoaded) {
        return (
            <GridView
                gridDto={gridDto!}
                dtoRows={dtoRows}
                dtoFields={dtoFields}
            />
        )
    }

}
