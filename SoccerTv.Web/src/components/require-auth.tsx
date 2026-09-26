import { Navigate, Outlet, useLocation } from 'react-router';
import { useAuth } from './auth-provider';

export function RequireAuth() {
    const location = useLocation();

    const { user, isLoading } = useAuth();

    if (isLoading) {
        return null;
    }

    if (!user) {
        // Return here after signing in
        return <Navigate to="/account/login" state={{ from: location }} />;
    }

    return <Outlet />;
}
