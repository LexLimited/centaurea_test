import React, { useState } from 'react';
import { Button, TextField } from '@mui/material';

interface NewNameInputProps {
    onCommit?: (fieldName: string) => void;
    onFinished?: () => void;
}

export const NewNameInput: React.FC<NewNameInputProps> = ({ onCommit, onFinished }) => {
    const [fieldName, setFieldName] = useState<string>('');
    const [errorMessage, setErrorMessage] = useState<string>('');

    const handleFieldNameChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setFieldName(event.target.value);
    };

    return (
        <div style={{
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            marginBottom: 16,
        }}>
            <TextField
                label="New Name"
                value={fieldName}
                onChange={handleFieldNameChange}
                variant="outlined"
                style={{ flex: 1, marginRight: 8 }} // Use flex: 1 to take available space
                inputProps={{ style: { minWidth: '200px' } }} // Set a minimum width
           
            />
            <div style={{ padding: 10, display: 'flex', flexDirection: 'column' }}>
                <Button style={{ margin: 6 }} variant="contained" color="primary" onClick={() => {
                    onFinished?.();
                    onCommit?.(fieldName);
                }}>
                    Commit changes
                </Button>
                <Button style={{ margin: 6 }} variant="contained" color="primary" onClick={() => {
                    onFinished?.();
                }}>
                    Discard changes
                </Button>
            </div>
            {errorMessage && <div style={{ color: 'red', marginLeft: 8 }}>{errorMessage}</div>}
        </div>
    );
};
