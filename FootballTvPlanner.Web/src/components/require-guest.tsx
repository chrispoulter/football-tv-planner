import { Navigate, Outlet, useLocation } from 'react-router';
import { returnUrl } from '@/lib/return-url';
import { useAuth } from './auth-provider';
import { PageLoading } from './page-loading';

export function RequireGuest() {
    const location = useLocation();

    const { user, isLoading } = useAuth();

    if (isLoading) {
        return <PageLoading />;
    }

    if (user) {
        // Also where the login page goes once signed in, so the two agree
        return <Navigate to={returnUrl(location.state)} replace />;
    }

    return <Outlet />;
}
