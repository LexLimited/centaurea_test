import { Models } from '@/Models/DataGrid';
import axios from 'axios';

export namespace CentaureaApi {

    export const API_ENDPOINT = "https://localhost:8080/api";
    export const DATA_GRID_API_ENDPOINT = `${API_ENDPOINT}/datagrid`;

    const axiosInstance = axios.create({
        baseURL: DATA_GRID_API_ENDPOINT,
        timeout: 3000,
    });

    axiosInstance.interceptors.request.use(config => {
        console.log('[Info] Axios request:', config);
        return config;
    });

    axiosInstance.interceptors.response.use(
        res => {
            console.log('[Info] Axios response:', res);
            return res;
        },
        err => {
            console.error('[Error] Axios response:'), err;
        }
    );

    export async function getGridDescriptors() {
        return await axiosInstance.get<Models.Dto.DataGridDescriptor[]>('');
    }

    export async function getGrid(gridId: number) {
        return await axiosInstance.get<Models.Dto.DataGridDto>(`grid?gridId=${gridId}`);
    }

    export type CentaureaApiPutValueHandleExtraParams = {
        fieldId?: number,
        rowIndex?: number,
    };

    export async function putValue(dto: Models.Dto.DataGridValueDto, params: CentaureaApiPutValueHandleExtraParams) {
        dto.fieldId = params.fieldId || dto.fieldId;
        dto.rowIndex = params.rowIndex || dto.rowIndex;

        return await axiosInstance.put<Models.Dto.DataGridValueDto>(`/field/${dto.fieldId}/value`, dto);
    }

    export async function deleteGrid(gridId: number) {
        return await axiosInstance.delete<number>(`/grid/${gridId}`);
    }

    export async function renameField(fieldId: number, newName: string) {
        return await axiosInstance.put<string>(
            `/field/${fieldId}/rename`,
            `"${newName}"`,
            {
                headers: {
                    'Content-Type': 'application/json'
                }
            }
        );
    }

    export async function renameGrid(gridId: number, newName: string) {
        return axiosInstance.put(`/grid/${gridId}/rename?newName=${newName}`);
    }

    export async function deleteField(fieldId: number) {
        return await axiosInstance.delete<number>(`/field/${fieldId}`);
    }

    export async function deleteRow(gridId: number, rowIndex: number) {
        return await axiosInstance.delete<number[]>(`/row/${gridId}?rowIndex=${rowIndex}`);
    }

}