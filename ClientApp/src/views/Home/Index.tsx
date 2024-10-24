import { WeatherForecast, WeatherForecastService } from "@/openapi";
import { DataGrid, DataGridBody, DataGridCell, DataGridHeader, DataGridHeaderCell, DataGridRow, Text, createTableColumn } from "@fluentui/react-components";
import { useEffect, useState } from "react";

const columns = (["date", "temperatureC", "summary"] as const)
    .map(it => createTableColumn<WeatherForecast>({
        columnId: it,
        renderHeaderCell: () => <Text weight="semibold">{it}</Text>,
        renderCell(item) {
            return item[it]
        },
    }))

export const Index = () => {
    const [data, setData] = useState<WeatherForecast[]>([])

    useEffect(() => {
        WeatherForecastService.getWeatherForecast().then(setData)
    }, [])

    return <DataGrid items={data} columns={columns}>
        <DataGridHeader>
            <DataGridRow>
                {({ renderHeaderCell }) => <DataGridHeaderCell>{renderHeaderCell()}</DataGridHeaderCell>}
            </DataGridRow>
        </DataGridHeader>
        <DataGridBody>
            {({ item }) => <DataGridRow>
                {({ renderCell }) => <DataGridCell>{renderCell(item)}</DataGridCell>}
            </DataGridRow>}
        </DataGridBody>
    </DataGrid>
}