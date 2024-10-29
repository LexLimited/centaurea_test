import { Models } from '@/Models/DataGrid';
import axios from 'axios';

export namespace CentaureaApi {

    export const API_ENDPOINT = "https://localhost:8080/api";
    export const DATA_GRID_API_ENDPOINT = `${API_ENDPOINT}/datagrid`;

    export async function getGridDescriptors() {
        return await axios.get<Models.Dto.DataGridDescriptor[]>(`${DATA_GRID_API_ENDPOINT}`);
    }

    export async function getGrid(gridId: number) {
        return await axios.get<Models.Dto.DataGridDto>(`${DATA_GRID_API_ENDPOINT}/grid?gridId=${gridId}`);
    }

}