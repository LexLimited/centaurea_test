import axios from 'axios';
import React from 'react';
import { Outlet } from 'react-router-dom';

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
    return <Outlet />
}