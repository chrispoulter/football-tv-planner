import { Navigate, Outlet, useLocation } from 'react-router';
import { useAuth } from './auth-provider';
import { PageLoading } from './page-loading';

export function RequireAuth() {
    const location = useLocation();

    const { user, isLoading } = useAuth();

    if (isLoading) {
        return <PageLoading />;
    }

    if (!user) {
        // Return here after signing in
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    return <Outlet />;
}
