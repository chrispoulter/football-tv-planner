import { Route } from 'react-router';
import { RequireGuest } from '@/components/require-guest';
import { LoginPage } from './login/login-page';
import { RegisterPage } from './register/register-page';
import { ConfirmEmailPage } from './confirm-email/confirm-email-page';
import { ForgotPasswordPage } from './forgot-password/forgot-password-page';
import { ResetPasswordPage } from './reset-password/reset-password-page';
import { TwoFactorPage } from './two-factor/two-factor-page';

export const accountRoutes = (
    <Route path="account">
        <Route element={<RequireGuest />}>
            <Route path="login" element={<LoginPage />} />
            <Route path="two-factor" element={<TwoFactorPage />} />
            <Route path="register" element={<RegisterPage />} />
            <Route path="forgot-password" element={<ForgotPasswordPage />} />
            <Route path="reset-password" element={<ResetPasswordPage />} />
        </Route>

        {/* Also used while signed in to confirm a change of email */}
        <Route path="confirm-email" element={<ConfirmEmailPage />} />
    </Route>
);
