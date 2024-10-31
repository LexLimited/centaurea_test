import { Models } from "@/Models/DataGrid";
import { CentaureaApi } from "@/api/CentaureaApi";
import { AxiosResponse } from "axios";
import React, { useContext, createContext } from "react";

export type AuthContextValue = {
    logIn: (logInData: CentaureaApi.LogInModel, onLogIn?: () => void) => Promise<AxiosResponse<CentaureaApi.LogInResult, any>>,
    logOut: (onLogOut?: () => any) => any,
    authStatus: CentaureaApi.WhoAmIResult,
};

export const AuthContext = createContext<AuthContextValue>({
    logIn: (() => {}) as never,
    logOut: (() => {}) as never,
    authStatus: {
        username: '',
        roles: [],
        isPrivileged: false,
    },
});

export const AuthProvider: React.FC<any> = ({ children }) => {
    const [authStatus, setAuthStatus] = React.useState<CentaureaApi.WhoAmIResult>({
        username: '',
        roles: [],
        isPrivileged: false,
    });

    const onAuthStateChangeAction = React.useRef<(() => void) | undefined>(undefined);

    React.useEffect(() => {
        CentaureaApi.whoAmI().then(res => setAuthStatus(res.data));
    }, []);

    React.useEffect(() => {
        onAuthStateChangeAction?.current?.();
        onAuthStateChangeAction.current = undefined;
    }, [authStatus]);

    const logIn = async (logInModel: CentaureaApi.LogInModel, onLogIn?: () => void): Promise<AxiosResponse<CentaureaApi.LogInResult, any>> => {
        if (onAuthStateChangeAction) {
            onAuthStateChangeAction.current = onLogIn;
        }

        const res = await CentaureaApi.logIn(logInModel);

        if (!res.data) {
            throw new Error(res.data);
        }

        CentaureaApi.whoAmI().then(res => setAuthStatus(res.data));

        return res;
    };

    const logOut = async (onLogOut?: () => void) => {
        if (onAuthStateChangeAction) {
            onAuthStateChangeAction.current = onLogOut;
        }

        await CentaureaApi.logOut();
        CentaureaApi.whoAmI().then(res => setAuthStatus(res.data));
    };

    return <AuthContext.Provider value={{
        logIn,
        logOut,
        authStatus,
    }}>{children}</AuthContext.Provider>
}

export const useAuth = () => {
    return useContext(AuthContext);
}