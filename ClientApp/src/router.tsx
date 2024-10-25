import { createBrowserRouter } from "react-router-dom";
import { HomeRoutes } from "./views/Home/routes";
import { Layout } from "./views/Layout";
import { SettingsRoutes } from "./views/Settings/routes";
import { DataGridRoutes } from "./views/DataGrid/routes";

export default createBrowserRouter([
    {
        path: "/",
        element: <Layout />,
        children: [
            ...HomeRoutes,
            ...SettingsRoutes,
            ...DataGridRoutes,
        ]
    }
], {
    basename: "/app"
})