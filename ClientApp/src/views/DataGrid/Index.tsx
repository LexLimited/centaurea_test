import axios from 'axios';
import React from 'react';

class DataGridPayload {
    public id: number;
    public name: string;

    constructor() {
        this.id = 0;
        this.name = "defaul name";
    }

    getPayload() {
        const signature = {
            Fields: [],          
        };

        return {
            Id: this.id,
            Name: this.name,
            Signature: signature,
            Rows: [],
        };
    }
}

export function Index() {
    const [grid, setGrid] = React.useState<DataGridPayload>(new DataGridPayload());

    const postGrid = () => {
        axios.post('http://localhost:5468/api/datagrid/create', grid.getPayload())
            .then(() => console.log('Sent ok'))
            .catch(console.error);
    };

    return (
        <div>
            <label style={{ marginRight: 10 }}>name</label>
            <input placeholder='default name' onChange={e => {
                grid.name = e.target.value;
                setGrid(grid);
            }}></input>
            <button onClick={postGrid}>Create</button>
        </div>
    );
}