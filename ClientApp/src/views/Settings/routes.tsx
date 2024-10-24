import { RouteObject } from "react-router-dom";
import { Index } from "./Index";

export const SettingsRoutes = [
    {
        path: "settings",
        children: [
            {
                index: true,
                element: <Index />
            }
        ]
    }
] as RouteObject[]