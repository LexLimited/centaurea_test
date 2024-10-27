export const Index = () => {
    return <div>Home index</div>
}

// console.log("Home/Index");

// import { WeatherForecast, WeatherForecastService } from "@/openapi";
// import { DataGrid } from "@/openapi/models/DataGrid";
// import { DataGridService } from "@/openapi/services/DataGridService";
// import { DataGrid as UIDataGrid, DataGridBody, DataGridCell, DataGridHeader, DataGridHeaderCell, DataGridRow, Text, createTableColumn, TableColumnDefinition } from "@fluentui/react-components";
// import { useEffect, useState } from "react";

// const columns = (["date", "temperatureC", "summary"] as const)
//     .map(it => createTableColumn<WeatherForecast>({
//         columnId: it,
//         renderHeaderCell: () => <Text weight="semibold">{it}</Text>,
//         renderCell(item) {
//             return item[it]
//         },
//     }))

// const GRID_ID = 11;

// export const Index = () => {
//     // const [data, setData] = useState<WeatherForecast[]>([])

//     // useEffect(() => {
//     //     WeatherForecastService.getWeatherForecast().then(setData)
//     // }, [])

//     const [grid, setGrid] = useState<DataGrid | null>(null);

//     useEffect(() => {
//         DataGridService.getDataGrid(GRID_ID)
//             .then(setGrid)
//             .catch(err => console.error(`Request to data grid service failed: ${err}`));
//     }, []);

//     const uiColumns = [] as TableColumnDefinition<any>[];

//     // const uiColumns = grid.signature.map(sig => {
//     //     return createTableColumn<object>({
//     //         columnId: sig.name,
//     //         renderHeaderCell: () => <Text weight="semibold">{sig.name}</Text>,
//     //         renderCell(item) {
//     //             return <Text>Cell</Text>;
//     //         }
//     //     });
//     // });
    
//     if (grid == null) {
//         return <Text>Grid with id {GRID_ID} not found</Text>
//     }

//     return <UIDataGrid items={grid.rows} columns={uiColumns}>
//         <DataGridHeader>
//             <DataGridRow>
//                 {({ renderHeaderCell }) => <DataGridHeaderCell>{renderHeaderCell()}</DataGridHeaderCell>}
//             </DataGridRow>
//         </DataGridHeader>
//         <DataGridBody>
//             {({ item }) => <DataGridRow>
//                 {({ renderCell }) => <DataGridCell>{renderCell(item)}</DataGridCell>}
//             </DataGridRow>}
//         </DataGridBody>
//     </UIDataGrid>
// }