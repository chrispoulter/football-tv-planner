import { Navigate, Outlet } from 'react-router';
import { useAuth } from './auth-provider';

export function RequireAuth() {
    const { user } = useAuth();

    if (!user) {
        return <Navigate to="/account/login" />;
    }

    return <Outlet />;
}
