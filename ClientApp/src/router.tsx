import { createBrowserRouter } from "react-router-dom";
import { HomeRoutes } from "./views/Home/routes";
import { Layout } from "./views/Layout";
import { SettingsRoutes } from "./views/Settings/routes";
import { DataGridRoutes } from "./views/DataGrid/routes";
import { AuthRoutes } from "./views/Auth/routes";

export default createBrowserRouter([
    {
        path: "/",
        element: <Layout />,
        children: [
            ...DataGridRoutes,
            ...AuthRoutes,
            ...HomeRoutes,
            ...SettingsRoutes,
        ]
    }
], {
    basename: "/app"
})