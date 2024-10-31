import * as React from 'react';
import { useState, useEffect } from 'react';
import { DataGrid, GridColDef } from '@mui/x-data-grid';
import { Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField, Select, MenuItem, Typography } from '@mui/material';
import { Models } from '@/Models/DataGrid';
import { CentaureaApi } from '@/api/CentaureaApi';
import { deepClone } from '@mui/x-data-grid/internals';

export function Create() {
  const [gridName, setGridName] = useState<string>('');

  const [columns, setColumns] = useState<{ fieldDef: any, colDef: GridColDef }[]>([]);
  
  const [openDialog, setOpenDialog] = useState<boolean>(false);
  
  const [refOptions, setRefOptions] = useState<number[]>([]);
  
  const [newField, setNewColumn] = useState<{
    name: string,
    type: Models.DataGridValueType,
    options: any,
  }>({
    name: '',
    type: '',
    options: null,
  });

  useEffect(() => {
    if (newField.type === 'Ref') {
      CentaureaApi.getGridDescriptors()
        .then(res => setRefOptions(res.data.map(descriptor => descriptor.id)))
        .catch(err => console.error('[Error] fetching ref options:', err));
    }
  }, [newField.type]);

  function createDataGridSignatureFieldDtos(): Models.Dto.DataGridFieldSignatureDto[] {
    return columns.map((column, index) => {
      const { name, type, options } = column.fieldDef;
      const dto: Models.Dto.DataGridFieldSignatureDto = {
        name: name,
        type: type,
        order: index,
      };

      switch (type) {
        case 'Regex':
          dto.regexPattern = options;
          break;
        case 'Ref':
          dto.referencedGridId = options;
          break;
        case 'SingleSelect':
        case 'MultiSelect':
          dto.options = options;
          break;
        default:
          break;
      }
      return dto;
    });
  }

  function createDataGridDto(): Models.Dto.DataGridDto {
    return {
      name: gridName,
      rows: [],
      signature: {
        fields: createDataGridSignatureFieldDtos(),
      }
    };
  }

  const handleAddColumnClick = () => setOpenDialog(true);

  const handleCloseDialog = () => {
    setNewColumn({ name: '', type: '', options: null });
    setOpenDialog(false);
  };

  const handleAddColumn = () => {
    const fieldDef = deepClone(newField);

    const newColumn: GridColDef = {
      field: fieldDef.name,
      fieldDef: fieldDef,
      headerName: newField.name,
    };

    setColumns([...columns, { fieldDef: newField, colDef: newColumn }]);
    handleCloseDialog();
  };

  const renderExtraSettings = () => {
    switch (newField.type) {
      case 'Regex':
        return (
          <TextField
            label="Enter Regex Pattern"
            fullWidth
            margin="dense"
            value={newField.options || ""}
            onChange={(e) => setNewColumn({ ...newField, options: e.target.value })}
          />
        );
      case 'Ref':
        return (
          <>
            <Select
              label="Select Reference ID"
              fullWidth
              margin="dense"
              value={newField.options || ""}
              onChange={(e) => setNewColumn({ ...newField, options: e.target.value })}
            >
              {refOptions.map((option) => (
                <MenuItem key={option} value={option}>
                  {option}
                </MenuItem>
              ))}
            </Select>
            {newField.options && (
              <Typography variant="body2" color="textSecondary">
                Selected Reference ID: {newField.options}
              </Typography>
            )}
          </>
        );
      case 'SingleSelect':
      case 'MultiSelect':
        return (
          <TextField
            label="Options (comma-separated)"
            fullWidth
            margin="dense"
            value={newField.options || ""}
            onChange={(e) =>
              setNewColumn({
                ...newField,
                options: e.target.value.split(',').map((val) => val.trim()),
              })
            }
          />
        );
      default:
        return null;
    }
  };

  async function handleCreateClick() {
    const gridDto = createDataGridDto();

    CentaureaApi.createGrid(gridDto)
      .then(console.log)
      .catch(err => {
        console.error('[Error] creation failed:', err);
        console.log('[Info] the following json failed:', JSON.stringify(gridDto));
      });
  }

  return (
    <div style={{ height: 400, width: '100%' }}>
      <Button variant="contained" onClick={handleAddColumnClick}>Add Column</Button>
      <Button variant="contained" onClick={handleCreateClick}>Create</Button>
      <TextField
        label="Grid Name"
        fullWidth
        margin="dense"
        value={gridName}
        onChange={(e) => setGridName(e.target.value)}
      />
      <DataGrid
        columns={columns.map(c => c.colDef)}
        rows={[]}
        slots={{
          columnMenu: (props) => {
            const fieldDef = props.colDef.fieldDef;

            if (fieldDef.type == 'Ref') {
              return (
                <div>
                  <label>ref. grid id</label><br/>
                  <label>{fielddef.options}</label>
                </div>
              );
            }

            if (fieldDef.type == 'SingleSelect') {
              return (
                <div>
                  <label>Option ids:</label><br/>
                  {
                    fieldDef.options.map()
                  }
                </div>
              );
            }

            return (
              <div>Value: {fieldDef.options}</div>
            )
          }
        }}
      />

      <Dialog open={openDialog} onClose={handleCloseDialog}>
        <DialogTitle>Add New Column</DialogTitle>
        <DialogContent>
          <TextField
            label="Column Name"
            fullWidth
            margin="dense"
            value={newField.name}
            onChange={(e) => setNewColumn({ ...newField, name: e.target.value })}
          />
          <Select
            fullWidth
            margin="dense"
            value={newField.type}
            onChange={(e) => setNewColumn({ ...newField, type: e.target.value })}
            displayEmpty
          >
            <MenuItem value="" disabled>Select Type</MenuItem>
            <MenuItem value="String">String</MenuItem>
            <MenuItem value="Numeric">Number</MenuItem>
            <MenuItem value="Email">Email</MenuItem>
            <MenuItem value="Regex">Regex</MenuItem>
            <MenuItem value="Ref">Ref</MenuItem>
            <MenuItem value="SingleSelect">Single Select</MenuItem>
            <MenuItem value="MultiSelect">Multi Select</MenuItem>
          </Select>
          {renderExtraSettings()}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Cancel</Button>
          <Button onClick={handleAddColumn} variant="contained">Add Column</Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}
