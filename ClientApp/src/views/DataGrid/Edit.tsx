import * as React from 'react';
import { DataGrid, GridColDef, GridCellEditStopParams } from '@mui/x-data-grid';
import { Button, Grid } from '@mui/material';
import { Models } from '@/Models/DataGrid';
import { CentaureaApi } from '@/api/CentaureaApi';

function ExpandingComponent({ children, style, ...props }: any) {
    return (
        <div style={{ background: 'white', padding: '1em', border: '1px solid #ccc', ...style }}>
            {children}
        </div>
    );
}

function FancyButton({
    children,
    style,
}: any) {
    return (
        <button style={{
            background: 'orange',
            borderRadius: '6px',
            border: 'none',
            padding: '6px',
            ...style,
        }}>
            {children}
        </button>
    );
};

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

function GridIdsList({
    gridDescriptors,
    onSelect,
}: {
    gridDescriptors: Models.Dto.DataGridDescriptor[],
    onSelect?: (gridId: number) => void,
}) {
    const [selectedId, setSelectedId] = React.useState<number | undefined>();

    if (!gridDescriptors.length) {
        return (
            <GridContainer>No available grid ids</GridContainer>
        )
    }

    const options = gridDescriptors.map(descriptor => (
        <Button
            key={descriptor.id}
            disabled={selectedId == descriptor.id}
            onClick={() => {
                setSelectedId(descriptor.id);
                onSelect?.(descriptor.id)
            }}
        >
            {JSON.stringify(descriptor.name)}
        </Button>
    ));

    return (
        <GridContainer>
            <div style={{ padding: 10 }}>{options}</div>
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

function GridView({
    rows, fields,
}: {
    rows: Models.Dto.DataGridDto['rows'],
    fields: Models.Dto.DataGridDto['signature']['fields'],
}) {
    if (!fields) {
        return <GridContainer>Grid has no fields</GridContainer>
    }

    return <GridContainer>Grid will be here</GridContainer>
}

export function Edit() {
    const [gridDescriptors, setGridDescriptors] = React.useState<Models.Dto.DataGridDescriptor[]>([]);

    const [exceptionDetails, setExceptionDetails] = React.useState<string>('');

    const [stage, setStage] = React.useState<Stage>(Stage.SelectingGrid);

    const [rows, setRows] = React.useState<Models.Dto.DataGridDto['rows']>([]);

    const [fields, setFields] = React.useState<Models.Dto.DataGridDto['signature']['fields']>([]);

    const [popUpShown, setPopUpShown] = React.useState<boolean>(false);

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
            setRows(gridDto.rows);
            setFields(gridDto.signature.fields);
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
            <GridIdsList
                gridDescriptors={gridDescriptors}
                onSelect={fetchGridDto}
            />
        )
    }

    if (stage == Stage.Exception) {
        return <ExceptionView details={exceptionDetails} />
    }

    if (stage == Stage.GridLoaded) {
        return <GridView rows={rows} fields={fields} />
    }

    // return (
    //     <GridContainer>
    //         <DataGrid
    //             onColumnHeaderClick={(props) => {

    //             }}
    //             slots={{
    //                 columnMenu: (props) => {
    //                     return (
    //                         <div>
    //                             <Button variant='contained' onClick={() => {
    //                                 const colIndex = columns.findIndex(col => col.field == props.colDef.field);
    //                                 let newColumns = [...columns];
    //                                 newColumns.splice(colIndex, 1);
    //                                 setColumns(newColumns);
    //                             }}>Remove</Button>
    //                             <Button onClick={() => {
    //                             }}>Rename</Button>
    //                         </div>
    //                     )
    //                 },
    //             }}
    //             rows={rows}
    //             columns={columns}
    //             onCellClick={(params) => {
    //                 if (params.field !== 'actions') {
    //                     params.api.startCellEditMode({ id: params.id, field: params.field });
    //                 }
    //             }}
    //             onCellEditStop={(params) => {
    //                 params.api.stopCellEditMode({ id: params.id, field: params.field });
    //             }}
    //             processRowUpdate={(updatedRow, _, params) => {
    //                 const rowIndex = rows.findIndex((row) => row.id === params.rowId);
    //                 const updatedRows = [...rows];
    //                 updatedRows[rowIndex] = updatedRow;
    //                 setRows(updatedRows);
    //             }}
    //         />
    //         <Button variant="contained" onClick={addColumn} style={{ margin: '10px 0' }}>
    //             Add New Column
    //         </Button>
    //         <Button onClick={() => setPopUpShown(!popUpShown)}>Click to render JSON</Button>
    //         {popUpShown ? (
    //             <ExpandingComponent style={{ top: 350, left: 10 }}>
    //                 <pre>{JSON.stringify({ rows, columns }, null, 2)}</pre>
    //             </ExpandingComponent>
    //         ) : null}
    //     </GridContainer>
    // );
}
