import { Models } from '@/Models/DataGrid';
import axios from 'axios';

export namespace CentaureaApi {

    export const BASE_ENDPOINT = "https://localhost:8080";
    export const API_ENDPOINT = `${BASE_ENDPOINT}/api`;
    export const AUTH_ENDPOINT = `${BASE_ENDPOINT}/auth`
    export const DATA_GRID_API_ENDPOINT = `${API_ENDPOINT}/datagrid`;

    const authInstance = axios.create({
        baseURL: `${AUTH_ENDPOINT}`,
        timeout: 3000,
    });

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

    authInstance.interceptors.request.use(config => {
        console.log('[Info] Axios request:', config);
        return config;
    });

    authInstance.interceptors.response.use(
        res => {
            console.log('[Info] Axios response:', res);
            return res;
        },
        err => {
            console.error('[Error] Axios response:'), err;
        }
    );
    export async function getGridDescriptors() {
        return axiosInstance.get<Models.Dto.DataGridDescriptor[]>('');
    }

    export async function getGrid(gridId: number) {
        return axiosInstance.get<Models.Dto.DataGridDto>(`grid?gridId=${gridId}`);
    }

    export async function createGrid(gridDto: Models.Dto.DataGridDto) {
        return axiosInstance.post(`grid`, gridDto);
    }

    export type CentaureaApiPutValueHandleExtraParams = {
        fieldId: number,
        rowIndex: number,
    };

    export async function putValue(dto: Models.Dto.DataGridValueDto, params: CentaureaApiPutValueHandleExtraParams) {
        dto.fieldId = params.fieldId || dto.fieldId;
        dto.rowIndex = params.rowIndex || dto.rowIndex;

        return axiosInstance.put<Models.Dto.DataGridValueDto>(`/value?fieldId=${dto.fieldId}`, dto);
    }

    export async function postValue(dto: Models.Dto.DataGridValueDto, params: CentaureaApiPutValueHandleExtraParams) {
        dto.fieldId = params.fieldId || dto.fieldId;
        dto.rowIndex = params.rowIndex || dto.rowIndex;

        return axiosInstance.post<Models.Dto.DataGridValueDto>(`/value?fieldId=${dto.fieldId}`, dto);
    }

    export async function deleteGrid(gridId: number) {
        return axiosInstance.delete<number>(`/grid/${gridId}`);
    }

    export async function renameField(fieldId: number, newName: string) {
        return axiosInstance.put<string>(`/field/${fieldId}/rename?newName=${newName}`);
    }

    export async function deleteField(fieldId: number) {
        return axiosInstance.delete<number>(`/field/${fieldId}`);
    }

    export async function deleteRow(gridId: number, rowIndex: number) {
        return axiosInstance.delete<number[]>(`/row/${gridId}?rowIndex=${rowIndex}`);
    }

    export async function logIn(logInModel: { username: string, password: string }) {
        return authInstance.post(`/login`, logInModel);
    }

    export async function getUsers() {
        return authInstance.get<Models.ApplicationUser[]>(`/users`);
    }

    export async function setGridPermissions(gridId: number, allowedUser: string[]) {
        return axiosInstance.post(`/grid/${gridId}/permissions`, allowedUser);
    }

}