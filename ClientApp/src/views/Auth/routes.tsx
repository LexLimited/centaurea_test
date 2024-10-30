
import { RouteObject } from "react-router-dom";
import { LogIn } from "./LogIn";
import { SignUp } from "./SignUp";

export const AuthRoutes = [
    {
        path: "auth",
        children: [
            {
                index: true,
                element: <LogIn />,
            },
            {
                path: "login",
                element: <LogIn />,
            },
            {
                path: "signup",
                element: <SignUp />,
            },
        ]
    },
] as RouteObject[]