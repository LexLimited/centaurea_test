import { RouteObject } from "react-router-dom";
import { Index } from "./Index";

export const HomeRoutes = [
    {
        path: "",
        children: [
            {
                index: true,
                element: <Index />
            },
        ]
    },
] as RouteObject[]