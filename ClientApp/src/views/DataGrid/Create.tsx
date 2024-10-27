import axios from 'axios';
import React, { useState } from "react";
import { Models } from '@/Models/DataGrid';

type IFieldPropsAssociatedValue = {
    regexPattern?: string,
    referencedGridId?: number,
    optionTableId?: number,
}

interface IFieldProps extends IFieldPropsAssociatedValue, Omit<React.ComponentProps<'div'>, 'onChange'> {
    name: string,
    type: Models.DataGridValueType,
    onChange?: (_: IFieldPropsAssociatedValue) => void,
}

const Field: React.FC<IFieldProps> = ({
    name,
    type,
    regexPattern,
    referencedGridId,
    optionTableId,
    onChange,
    ...props
}) => {
    return (
        <div style={{ padding: 10, background: 'rgba(0,0,0,0.1)', border: 'solid black 1px' }} {...props}>
            <label>Type: {type}</label><br/>
            <label>Name: {name}</label><br/>
            {
                regexPattern ? (
                    <div>
                        <label>Associated regex pattern: {regexPattern}</label><br/>
                    </div>
                ) : null
            }
            {
                referencedGridId ? (
                    <div>
                        <label>Associated regex pattern: {referencedGridId}</label><br/>
                    </div>
                ) : null
            }
            {
                optionTableId ? (
                    <div>
                        <label>Associated regex pattern: {optionTableId}</label><br/>
                    </div>
                ) : null
            }
        </div>
    );
}

interface IFieldsProps {
    fields: Models.DataGridDto['signature']['fields'],
}

const Fields: React.FC<IFieldsProps> = ({ fields }) => {
    if (fields.length == 0) {
        return <div><label>Table has no fields</label></div>
    }

    return (
        <div>{fields.map(Field)}</div>
    );
}

function createSampleDataGrid(): Models.DataGridDto {
    return {
        name: "Sample grid",
        id: 0,
        signature: {
            fields: [
                {
                    name: "numeric field name",
                    type: 'Numeric',
                },
                {
                    name: "string field name",
                    type: 'String',
                },
                {
                    name: "regex field name",
                    type: 'Regex',
                    regexPattern: '[a-zA-Z_]'
                },
            ]
        },
        rows: [],
    };
}

export function Create() {
    const [grid, setGrid] = useState<Models.DataGridDto>(createSampleDataGrid());

    const sendGrid = () => {
        console.log('Will create the following grid:', grid);

        axios.post("http://localhost:5468/api/datagrid/create", grid)
            .then(() => console.log("Post ok"))
            .catch((err) => console.error(`Post failed:`, err));
    };

    return (
        <div style={{ display: "flex", flexDirection: "column", padding: "20px" }}>
            <label>Create a new grid</label>
            <input
                placeholder="Grid name ..."
                value={grid.name}
                onChange={(e) => setGrid({ ...grid, name: e.target.value })}
            />
{/* 
            <div style={{ marginTop: "10px" }}>
                <label>Field Type:</label>
                <select value={fieldType} onChange={(e) => setFieldType(e.target.value as Models.DataGridValueType)}>
                    <option value="Numeric">Numeric</option>
                    <option value="String">String</option>
                    <option value="Email">Email</option>
                    <option value="Regex">Regex</option>
                    <option value="Ref">Ref</option>
                    <option value="SingleChoice">Single Choice</option>
                    <option value="MultipleChoice">Multiple Choice</option>
                </select>
            </div>

            <button onClick={addField} style={{ marginTop: "10px" }}>Add Field</button> */}

            <Fields fields={grid.signature.fields} />

            <button onClick={sendGrid} style={{ marginTop: "20px" }}>Create Grid</button>
        </div>
    );
}
