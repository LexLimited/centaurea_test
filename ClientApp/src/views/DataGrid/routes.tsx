import { RouteObject } from "react-router-dom";
import { Index } from "./Index";
import { Create } from "./Create";
import { Edit } from "./Edit";
import { ProtectedRoute } from "@/ProtectedRouter";

export const DataGridRoutes = [
    {
        path: "datagrid",
        element: <Index />,
        children: [
            {
                index: true,
                element: <Edit />,
            },
            {
                path: "create",
                element: <Create />
            },
            {
                path: "edit",
                element: <Edit />
            }
        ]
    },
    {
        path: "datagridedit",
        element: <Edit />,
    },
    {
        path: "datagridcreate",
        element: (
            <ProtectedRoute>
                <Create />
            </ProtectedRoute>
        ),
    }
] as RouteObject[]