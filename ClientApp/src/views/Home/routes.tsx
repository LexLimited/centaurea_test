import { RouteObject } from "react-router-dom";
import { Index } from "./Index";
import { AuthRoutes } from "../Auth/routes";

export const HomeRoutes = [
    {
        path: "",
        children: [
            {
                index: true,
                element: <Index />
            }
        ]
    },
] as RouteObject[]