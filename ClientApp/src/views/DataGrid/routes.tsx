import { RouteObject } from "react-router-dom";
import { Index } from "./Index";
import { Create } from "./Create";

export const DataGridRoutes = [
    {
        path: "datagrid",
        children: [
            {
                index: true,
                element: <Create />,
            },
            {
                path: "create",
                element: <Create />
            },
        ]
    },
] as RouteObject[]