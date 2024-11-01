import { CentaureaApi } from "@/api/CentaureaApi";
import { Models } from "@/Models/DataGrid";
import {
    Button,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    MenuItem,
    Select,
    TextField
} from "@mui/material";
import React from "react";

type ValidationResult = {
    ok: boolean,
    message?: string,
};

const validateFieldDef = (fieldDef: Models.Dto.DataGridFieldSignatureDto): ValidationResult => {
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

export function CreateFieldDialog({
    open,
    onSubmit,
    onCancel,
    onAction,
}: {
    open: boolean,
    onSubmit?: (field: Models.Dto.DataGridFieldSignatureDto) => void,
    onCancel?: () => void,
    onAction?: () => void,
}) {
    const [field, setField] = React.useState<Models.Dto.DataGridFieldSignatureDto>({
        id: 0,
        name: '',
        order: 0,
        type: '' as never,
    });

    const [refOptions, setRefOptions] = React.useState<Models.Dto.DataGridDescriptor[]>([]);

    React.useEffect(() => {
        if (field.type === 'Ref') {
            CentaureaApi.getGridDescriptors()
                .then(res => setRefOptions(res.data))
                .catch(err => console.error('[Error] fetching ref options:', err));
        }
    }, [field.type]);

    const renderExtraSettings = () => {
        switch (field.type) {
            case 'Regex':
                return (
                    <TextField
                        label="Enter Regex Pattern"
                        fullWidth
                        margin="dense"
                        value={field.regexPattern || ""}
                        onChange={(e) => setField({ ...field, regexPattern: e.target.value })}
                    />
                );
            case 'Ref':
                return (
                    <>
                        <Select
                            label="Select Reference ID"
                            fullWidth
                            margin="dense"
                            value={field.referencedGridId || ""}
                            onChange={(e) => setField({ ...field, referencedGridId: Number.parseInt(e.target.value) })}
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
                        value={field.options || ""}
                        onChange={(e) => {
                            const newField = {
                                ...field,
                                options: e.target.value.split(',').map((val) => val.trim()),
                            };
                            console.log('new field value:', newField);
                            setField(newField);
                        }}
                    />
                );
            default:
                return null;
        }
    };

    return (
        <Dialog open={open}>
            <DialogTitle>Add New Column</DialogTitle>
            <DialogContent>
                <TextField
                    label="Column Name"
                    fullWidth
                    required
                    margin="dense"
                    value={field.name}
                    onChange={(e) => {
                        setField({ ...field, name: e.target.value })
                    }}
                />
                <Select
                    fullWidth
                    margin="dense"
                    value={field.type}
                    onChange={(e) => {
                        setField({ ...field, type: e.target.value })
                    }
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
                <Button onClick={() => {
                    onCancel?.();
                    onAction?.();
                }}
                >
                    Cancel
                </Button>
                <Button onClick={() => {
                    const validationResult = validateFieldDef(field);
                    if (!validationResult.ok) {
                        alert(validationResult.message);
                        return;
                    }

                    onSubmit?.(field);
                    onAction?.();
                }} variant="contained">
                    Add Column
                </Button>
            </DialogActions>
        </Dialog>
    );
}