import { Navigate, Outlet } from 'react-router';
import { useAuth } from './auth-provider';

export function RequireGuest() {
    const { user, isLoading } = useAuth();

    if (isLoading) {
        return null;
    }

    if (user) {
        return <Navigate to="/" />;
    }

    return <Outlet />;
}
