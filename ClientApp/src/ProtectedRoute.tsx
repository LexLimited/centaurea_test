import { useContext } from "react"
import { AuthContext } from "./context/AuthContext";
import { Navigate, useLocation } from "react-router-dom";

export const ProtectedRoute= ({
    children,
    reasonDenied,
    requiredAuthority,
}: {
    children: any,
    reasonDenied?: string,
    requiredAuthority: 'privileged' | 'user' | 'none',
}) => {
    const { authStatus, loading } = useContext(AuthContext);
    const location = useLocation();

    const returnUrl = encodeURI(location.pathname);
    const reasonDeniedDefault = encodeURIComponent(`You don't have access to the requested page`);

    if (loading) {
        return <label>loading ...</label>;
    }

    if (requiredAuthority == 'privileged' && !authStatus.isPrivileged) {
        return <Navigate to={`/auth?returnUrl=${returnUrl}&redirectReason=${reasonDenied || reasonDeniedDefault}`} replace />
    }

    // TODO! Suspicious check
    if (requiredAuthority == 'user' && !authStatus.username.length) {
        return <Navigate to={`/auth?returnUrl=${returnUrl}&redirectReason=${reasonDenied || reasonDeniedDefault}`} replace />
    }

    return children;
}