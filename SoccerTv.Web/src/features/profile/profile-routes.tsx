import { Route } from 'react-router';
import { RequireAuth } from '@/components/require-auth';
import { ProfilePage } from './profile/profile-page';
import { ChangeEmailPage } from './change-email/change-email-page';
import { ChangePasswordPage } from './change-password/change-password-page';
import { TwoFactorPage } from './two-factor/two-factor-page';

export const profileRoutes = (
    <Route path="profile" element={<RequireAuth />}>
        <Route index element={<ProfilePage />} />
        <Route path="change-email" element={<ChangeEmailPage />} />
        <Route path="change-password" element={<ChangePasswordPage />} />
        <Route path="two-factor" element={<TwoFactorPage />} />
    </Route>
);
