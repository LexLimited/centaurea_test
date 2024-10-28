import axios from 'axios';
import React, { useEffect, useState } from 'react';
import { Models } from '@/Models/DataGrid';

interface DataGridProps {
    grid: Models.Dto.DataGridDto;
}

async function fetchGrid(): Promise<Models.Dto.DataGridDto> {
    return (await axios.get<Models.Dto.DataGridDto>(`http://localhost:5468/api/datagrid/grid?gridId=${45}`)).data;
}

export const Create: React.FC<DataGridProps> = () => {
    const [grid, setGrid] = useState<Models.Dto.DataGridDto>();
    const [requestStatus, setRequestStatus] = useState<'pending' | 'fail' | 'ok'>('pending');

    useEffect(() => {
        (async () => {
            try {
                setGrid(await fetchGrid());
                setRequestStatus('ok');
            } catch {
                setRequestStatus('fail');
            }
        })();
    }, []);

    if (requestStatus == 'pending') {
        return <div><label>Please wait ...</label></div>
    } else if (requestStatus == 'fail') {
        return <div><label>Failed to fetch the grid</label></div>
    }

    return (
        <div>
            <h1>{grid.name}</h1>

            <h2>Signature</h2>
            <table>
                <thead>
                    <tr>
                        <th>Field Name</th>
                        <th>Type</th>
                        <th>Regex Pattern</th>
                    </tr>
                </thead>
                <tbody>
                    {grid.signature.fields.map((field, index) => (
                        <tr key={index}>
                            <td>{field.name}</td>
                            <td>{field.type}</td>
                            <td>{field.regexPattern || 'N/A'}</td>
                        </tr>
                    ))}
                </tbody>
            </table>

            <h2>Rows</h2>
            {grid.rows.length === 0 ? (
                <p>No rows available.</p>
            ) : (
                <table>
                    <thead>
                        <tr>
                            {grid.signature.fields.map((field, index) => (
                                <th key={index}>{field.name}</th>
                            ))}
                        </tr>
                    </thead>
                    <tbody>
                        {grid.rows.map((row, rowIndex) => (
                            <tr key={rowIndex}>
                                {row.items.map((item) => {
                                    // Display the value based on its type
                                    const value = item.stringValue || item.numericValue || item.optionId || 'N/A';
                                    return (
                                        <td key={item.fieldId}>
                                            {value}
                                        </td>
                                    );
                                })}
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
};
