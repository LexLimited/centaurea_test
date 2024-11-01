import * as React from 'react';
import { useState, useEffect } from 'react';
import { DataGrid, GridColDef } from '@mui/x-data-grid';
import { Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField, Select, MenuItem, Typography } from '@mui/material';
import { Models } from '@/Models/DataGrid';
import { CentaureaApi } from '@/api/CentaureaApi';
import { deepClone } from '@mui/x-data-grid/internals';
import { useNavigate } from 'react-router-dom';
import { CreateFieldDialog } from './CreateFieldDialog';

type FieldDef = {
  idx: number,
  name: string,
  type: Models.DataGridValueType,
  options?: string,
};

export function Create() {
  const [gridName, setGridName] = useState<string>('');

  const [columns, setColumns] = useState<{ fieldDef: FieldDef, colDef: GridColDef }[]>([]);

  const [openDialog, setOpenDialog] = useState<boolean>(false);

  const [refOptions, setRefOptions] = useState<Models.Dto.DataGridDescriptor[]>([]);

  const [newField, setNewField] = useState<FieldDef>({
    idx: 0,
    name: '',
    type: '',
    options: undefined,
  });

  const navigate = useNavigate();

  useEffect(() => {
    if (newField.type === 'Ref') {
      CentaureaApi.getGridDescriptors()
        .then(res => setRefOptions(res.data))
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
    setNewField({ name: '', type: '', options: undefined });
    setOpenDialog(false);
  };

  type ValidationResult = {
    ok: boolean,
    message?: string,
  };

  const validateFieldDef = (fieldDef: FieldDef): ValidationResult => {
    if (fieldDef.name.trim().length == 0) {
      return {
        ok: false,
        message: 'Field name must not be empty or consist of whitespaces only'
      };
    }

    if (fieldDef.type as never == '') {
      return {
        ok: false,
        message: 'Select the type for this field',
      };
    }

    if (fieldDef.type == 'SingleSelect' || fieldDef.type == 'MultiSelect') {
      if (fieldDef.options!.some(segment => !segment.length)) {
        return { ok: false, message: 'Option names cannot be empty' };
      }
    }

    return { ok: true };
  }

  const handleAddColumn = () => {
    const validationResult = validateFieldDef(newField);
    if (!validationResult.ok) {
      alert(validationResult.message);
      return;
    }

    const fieldDef = deepClone(newField);

    const maxIdx = Math.max(...columns.map(c => c.colDef.fieldDef.idx));
    newField.idx = maxIdx + 1;

    const newColumn: GridColDef = {
      field: newField.idx,
      fieldDef,
      headerName: newField.name,
      sortable: false,
    };

    setColumns([...columns, { fieldDef: newField, colDef: newColumn }]);

    handleCloseDialog();
  };

  const removeColumnByIdx = (idx: number) => {
    console.log('Removing column:', idx);

    const index = columns.findIndex(c => c.colDef.fieldDef.idx == idx);
    if (index < 0) {
      console.log('Trying to remove a column that does not exist');
      return;
    }

    const newColumns = [...columns];
    newColumns.splice(index, 1);

    setColumns([...newColumns]);
  }

  const renderExtraSettings = () => {
    switch (newField.type) {
      case 'Regex':
        return (
          <TextField
            label="Enter Regex Pattern"
            fullWidth
            margin="dense"
            value={newField.options || ""}
            onChange={(e) => setNewField({ ...newField, options: e.target.value })}
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
              onChange={(e) => setNewField({ ...newField, options: e.target.value })}
            >
              {refOptions.map((option) => (
                <MenuItem key={option.id} value={option.id}>
                  {option.name} ({option.id})
                </MenuItem>
              ))}
            </Select>
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
              setNewField({
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

    if (!gridDto.name.trim().length) {
      alert('Grid name should not be empty or consist of whitespaces only');
      return;
    }

    CentaureaApi.createGrid(gridDto)
      .then(res => {
        if (!res || res.status != 200) {
          alert(`Failed to create grid: ${res?.data || ""}`);
        } else {
          navigate('/datagridlist');
        }
      })
      .catch(err => {
        alert(`Failed to create grid: ${err}`);
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
        required
        onChange={(e) => setGridName(e.target.value)}
      />
      <DataGrid
        columns={columns.map(c => c.colDef)}
        rows={[]}
        slots={{
          columnMenu: (props) => {
            console.log(props);
            const onDeleteClick = () => removeColumnByIdx(props.colDef.fieldDef.idx);
            const onRenameClick = () => {
              const newName = prompt('Enter new name:');
              const changedColumn = columns.find(c => c.colDef.fieldDef.idx == props.colDef.fieldDef.idx);

              if (newName && changedColumn) {
                console.log('Column is being changed to', newName);

                changedColumn!.fieldDef.name = newName;
                changedColumn.colDef.headerName = newName;
                setColumns([...columns]);
              }
            
              if (!changedColumn) {
                console.error('Changed column not found');
              }
            };

            return (
              <div style={{ display: 'flex', flexDirection: 'column' }}>
                <Button onClick={onDeleteClick}>Delete</Button>
                <Button onClick={onRenameClick}>Rename</Button>
              </div>
            );
          }
        }}
      />

      <Dialog open={openDialog} onClose={handleCloseDialog}>
        <DialogTitle>Add New Column</DialogTitle>
        <DialogContent>
          <TextField
            label="Column Name"
            fullWidth
            required
            margin="dense"
            value={newField.name}
            onChange={(e) => {
              setNewField({ ...newField, name: e.target.value })
            }}
          />
          <Select
            fullWidth
            margin="dense"
            value={newField.type}
            onChange={(e) => {
              setNewField({ ...newField, type: e.target.value })}
            }
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
