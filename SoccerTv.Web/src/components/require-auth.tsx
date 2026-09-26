import { Navigate, Outlet } from 'react-router';
import { useAuth } from './auth-provider';

export function RequireAuth() {
    const { user, isLoading } = useAuth();

    if (isLoading) {
        return null;
    }

    if (!user) {
        return <Navigate to="/account/login" />;
    }

    return <Outlet />;
}
