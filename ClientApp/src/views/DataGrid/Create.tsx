import * as React from 'react';
import { useState } from 'react';
import { DataGrid, GridColDef } from '@mui/x-data-grid';
import { Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField, Select, MenuItem } from '@mui/material';
import { Models } from '@/Models/DataGrid';
import { CentaureaApi } from '@/api/CentaureaApi';

export function Create() {
  const [gridName, setGridName] = useState<string>('');

  const [columns, setColumns] = useState<{ fieldDef: any, colDef: GridColDef }[]>([]); // Stores column definitions

  const [openDialog, setOpenDialog] = useState<boolean>(false); // Manages dialog visibility
  
  const [refOptions, setRefOptions] = useState<number[]>([]);

  const [newField, setNewColumn] = useState<{
    name: string,
    type: Models.DataGridValueType,
    options: any,
  }>({
    name: '',
    type: '',
    options: null,
  }); // Manages new column details

  React.useEffect(() => {
    if (newField.type == 'Ref') {
      CentaureaApi.getGridDescriptors()
        .then(res => setRefOptions(res.data.map(descriptor => descriptor.id)));
    }
  }, [newField.type]);

  // Function to map column definitions to DataGrid field signature DTOs
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
          dto.regexPattern = options; // Store the regex pattern
          break;
        case 'Ref':
          dto.referencedGridId = options; // Store the reference grid ID
          break;
        case 'SingleSelect':
        case 'MultiSelect':
          dto.options = options; // Store options as an array of strings
          break;
        default:
          break; // No additional fields for basic types like string, number, email
      }
      return dto;
    });
  }

  // Creates DataGrid DTO for preview
  function createDataGridDto(): Models.Dto.DataGridDto {
    return {
      name: gridName,
      rows: [], // No rows
      signature: {
        fields: createDataGridSignatureFieldDtos(),
      }
    };
  }

  // Open the dialog to add a new column
  const handleAddColumnClick = () => setOpenDialog(true);

  // Close the dialog
  const handleCloseDialog = () => {
    setNewColumn({ name: '', type: '', options: null });
    setOpenDialog(false);
  };

  // Handle form submission to add column
  const handleAddColumn = () => {
    console.log('Adding a field:', newField);
    const newColumn: GridColDef = {
      field: newField.name,
      headerName: newField.name,
    };

    setColumns([...columns, { fieldDef: newField, colDef: newColumn }]);
    handleCloseDialog();
  };

  // Render settings based on column type
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

  // Render grid DTO preview in console
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
        label="Grid Name" // New field for grid name
        fullWidth
        margin="dense"
        value={gridName}
        onChange={(e) => setGridName(e.target.value)} // Update grid name state
      />
      <DataGrid
        columns={columns.map(c => c.colDef)}
        rows={[]} // No rows
        autoHeight
      />

      {/* Dialog for adding new column */}
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
