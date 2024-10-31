import { RouteObject } from "react-router-dom";
import { Index } from "./Index";
import { Create } from "./Create";
import { Edit } from "./Edit";
import { ProtectedRoute } from "@/ProtectedRoute";

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
        element: (
            <ProtectedRoute requiredAuthority='user' reasonDenied="Requested page requires authentication">
                <Edit />
            </ProtectedRoute>
        ),
    },
    {
        path: "datagridcreate",
        element: (
            <ProtectedRoute requiredAuthority='privileged' reasonDenied="Requested page requires privileged access">
                <Create />
            </ProtectedRoute>
        ),
    }
] as RouteObject[]