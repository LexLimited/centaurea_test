import { Models } from "@/Models/DataGrid";
import { CentaureaApi } from "@/api/CentaureaApi";
import { AxiosResponse } from "axios";
import React, { useContext, createContext } from "react";

export type AuthContextValue = {
    logIn: (logInData: CentaureaApi.LogInModel, onLogIn?: () => void) => Promise<AxiosResponse<CentaureaApi.LogInResult, any>>,
    authStatus: CentaureaApi.WhoAmIResult,
};

export const AuthContext = createContext<AuthContextValue>({
    logIn: (() => {}) as never,
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

    const onLogInAction = React.useRef<(() => void) | undefined>(undefined);

    React.useEffect(() => {
        CentaureaApi.whoAmI().then(res => setAuthStatus(res.data));
    }, []);

    React.useEffect(() => {
        onLogInAction?.current?.();
        onLogInAction.current = undefined;
    }, [authStatus]);

    const logIn = async (logInModel: CentaureaApi.LogInModel, onLogIn?: () => void): Promise<AxiosResponse<CentaureaApi.LogInResult, any>> => {
        if (onLogInAction) {
            onLogInAction.current = onLogIn;
        }

        const res = await CentaureaApi.logIn(logInModel);

        if (!res.data) {
            throw new Error(res.data);
        }

        CentaureaApi.whoAmI().then(res => setAuthStatus(res.data));

        return res;
    };

    return <AuthContext.Provider value={{
        logIn,
        authStatus,
    }}>{children}</AuthContext.Provider>
}

export const useAuth = () => {
    return useContext(AuthContext);
}