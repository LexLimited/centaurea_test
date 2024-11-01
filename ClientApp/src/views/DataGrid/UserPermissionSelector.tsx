import React, { useEffect, useState } from 'react';
import {
    Checkbox,
    ListItemText,
    MenuItem,
    Select,
    FormControl,
    InputLabel,
} from '@mui/material';
import { Button } from '@fluentui/react-components';
import { CentaureaApi } from '@/api/CentaureaApi';

export const UserPermissionSelector = ({ gridId }: { gridId: number }) => {
    const [options, setOptions] = React.useState<string[]>([]);

    const [selectedOptions, setSelectedOptions] = useState<string[]>([]);

    React.useEffect(() => {
        const fetchAndSetOptions = async () => {
            const options = (await CentaureaApi.getUsers()).data.map(user => user.userName); 
            const allowed = (await CentaureaApi.getGridPermissions(gridId)).data.map(user => user.userName); 

            setOptions(options);            
            setSelectedOptions(allowed);
        };

        fetchAndSetOptions();
    }, []);

    const handleToggle = (value) => {
        setSelectedOptions((prev) =>
            prev.includes(value)
                ? prev.filter((option) => option !== value)
                : [...prev, value]
        );
    };

    const handleSelectAll = (event) => {
        if (event.target.checked) {
            setSelectedOptions(options);
        } else {
            setSelectedOptions([]);
        }
    };

    return (
        <FormControl fullWidth>
            <InputLabel style={{ background: 'white' }} id="checkbox-dropdown-label">Select users with access to this table</InputLabel>
            <Select
                labelId="checkbox-dropdown-label"
                multiple
                value={selectedOptions}
                renderValue={(selected) => selected.join(', ')}
                MenuProps={{
                    PaperProps: {
                        style: {
                            maxHeight: 200,
                            width: 250,
                        },
                    },
                }}
            >
                <MenuItem>
                    <Checkbox
                        checked={selectedOptions.length === options.length}
                        onChange={handleSelectAll}
                    />
                    <ListItemText primary="Select All" />
                </MenuItem>
                {options.map((option) => (
                    <MenuItem key={option} onClick={() => handleToggle(option)}>
                        <Checkbox
                            checked={selectedOptions.includes(option)}
                        />
                        <ListItemText primary={option} />
                    </MenuItem>
                ))}
            </Select>
            <Button
                style={{ height: 55 }}
                onClick={() => {
                    CentaureaApi.setGridPermissions(gridId, selectedOptions);
                }}
            >
                Submit
            </Button>
        </FormControl>
    );
};
