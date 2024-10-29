import * as React from 'react';
import { DataGrid, GridColDef, GridCellEditStopParams } from '@mui/x-data-grid';
import { Button } from '@mui/material';

function PopUpWindows({ children, style, ...props }: any) {
  return (
    <div style={{ position: 'absolute', background: 'white', padding: '1em', border: '1px solid #ccc', ...style }}>
      {children}
    </div>
  );
}

export function Create() {
  const [rows, setRows] = React.useState([
    { id: 1, col1: 'Hello', col2: 'World' },
    { id: 2, col1: 'DataGridPro', col2: 'is Awesome' },
    { id: 3, col1: 'MUI', col2: 'is Amazing' },
  ]);

  const [columns, setColumns] = React.useState<GridColDef[]>([
    { field: 'col1', headerName: 'Column 1', width: 150, editable: true },
    { field: 'col2', headerName: 'Column 2', width: 150, editable: true },
  ]);

  const [popUpShown, setPopUpShown] = React.useState<boolean>(false);

  // Function to remove a column
  const removeColumn = (field: string) => {
    setColumns((prevColumns) => prevColumns.filter((col) => col.field !== field));
  };

  // Function to change a column's name
  const changeColumnName = (field: string, newHeaderName: string) => {
    setColumns((prevColumns) =>
      prevColumns.map((col) =>
        col.field === field ? { ...col, headerName: newHeaderName } : col
      )
    );
  };

  // Function to add a new column on the far right
  const addColumn = () => {
    const newField = `col${columns.length + 1}`;
    const newColumn: GridColDef = {
      field: newField,
      headerName: `New Column ${columns.length + 1}`,
      width: 150,
      editable: true,
    };
    setColumns((prevColumns) => [...prevColumns, newColumn]);

    // Add the new column with empty data for each row
    setRows((prevRows) =>
      prevRows.map((row) => ({ ...row, [newField]: '' }))
    );
  };

  const FancyButtonPair = ({
    callbackLeft,
    callbackRight,
  }: {
    callbackLeft?: () => any,
    callbackRight?: () => any,
  }) => {
    return (
      <div>
        <Button
          variant="contained"
          color="secondary"
          onClick={callbackLeft}
          style={{ marginRight: 5 }}
        >
          Remove
        </Button>
        <Button
          variant="contained"
          color="primary"
          onClick={callbackRight}
        >
          Rename
        </Button>
      </div>
    )
  };

  return (
    <div style={{ height: 400, width: '100%' }}>
      <DataGrid
        slots={{
          columnMenu: (props) => {
            return (
              <div>
                <button onClick={() => {
                  const colIndex = columns.findIndex(col => col.field == props.colDef.field);
                  let newColumns = [...columns];
                  newColumns.splice(colIndex, 1);
                  setColumns(newColumns);
                }}>Remove</button>
                <button onClick={() => {
                }}>Rename</button>
              </div>
            )
          },
        }}
        rows={rows}
        columns={[
          ...columns,
          {
            field: 'actions',
            headerName: 'Actions',
            width: 150,
            renderCell: (params) => FancyButtonPair({
              callbackLeft: () => removeColumn(params.field),
              callbackRight: () => {
                const newHeaderName = prompt('Enter new column name:', params.colDef.headerName);
                if (newHeaderName) changeColumnName(params.field, newHeaderName);
              }
            }),
          },
        ]}
        onCellClick={(params) => {
          if (params.field !== 'actions') {
            params.api.startCellEditMode({ id: params.id, field: params.field });
          }
        }}
        onCellEditStop={(params) => {
          params.api.stopCellEditMode({ id: params.id, field: params.field });
        }}
        processRowUpdate={(updatedRow, _, params) => {
          const rowIndex = rows.findIndex((row) => row.id === params.rowId);
          const updatedRows = [...rows];
          updatedRows[rowIndex] = updatedRow;
          setRows(updatedRows);
        }}
      />
      <Button variant="contained" onClick={addColumn} style={{ margin: '10px 0' }}>
        Add New Column
      </Button>
      <Button onClick={() => setPopUpShown(!popUpShown)}>Click to render JSON</Button>
      {popUpShown ? (
        <PopUpWindows style={{ top: 350, left: 10 }}>
          <pre>{JSON.stringify({ rows, columns }, null, 2)}</pre>
        </PopUpWindows>
      ) : null}
    </div>
  );
}
