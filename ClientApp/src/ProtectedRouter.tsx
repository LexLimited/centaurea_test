import { useContext } from "react"
import { AuthContext } from "./context/AuthContext";
import { Navigate, useLocation } from "react-router-dom";

export const ProtectedRoute= ({ children }: any) => {
    const { authStatus } = useContext(AuthContext);
    const location = useLocation();

    if (!authStatus.isPrivileged) {
        const returnUrl = encodeURI(location.pathname);
        const redirectReason = encodeURIComponent('Requested page requires privileged access');
        
        return <Navigate to={`/auth?returnUrl=${returnUrl}&redirectReason=${redirectReason}`} replace />
    }

    return children;
}