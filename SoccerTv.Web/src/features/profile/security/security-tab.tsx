import { useAuth } from '@/components/auth-provider';
import { Metadata } from '@/components/metadata';
import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from '@/components/ui/card';
import { ChangePasswordForm } from './change-password-form';
import { LinkedAccounts } from './linked-accounts';
import { SetPasswordForm } from './set-password-form';
import { TwoFactorSettings } from './two-factor/two-factor-settings';

export function SecurityTab() {
    const { user } = useAuth();

    // RequireAuth only renders this once the user has loaded
    if (!user) {
        return null;
    }

    const hasPassword = !!user.hasPassword;

    return (
        <div className="space-y-6">
            <Metadata title="Security" />

            <Card>
                <CardHeader>
                    <CardTitle>
                        {hasPassword ? 'Change Password' : 'Password'}
                    </CardTitle>
                    <CardDescription>
                        {hasPassword
                            ? "Choose a strong password and don't reuse it for other accounts"
                            : 'Add a password to your account'}
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    {/* Keyed so the form resets once a password has been set */}
                    {hasPassword ? (
                        <ChangePasswordForm key="change" />
                    ) : (
                        <SetPasswordForm key="set" />
                    )}
                </CardContent>
            </Card>

            <Card>
                <CardHeader>
                    <CardTitle>Two-Factor Authentication</CardTitle>
                    <CardDescription>
                        Add an extra layer of security by requiring a code from
                        an authenticator app when you sign in with your password
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    <TwoFactorSettings />
                </CardContent>
            </Card>

            <Card>
                <CardHeader>
                    <CardTitle>Linked Accounts</CardTitle>
                    <CardDescription>
                        Connect a third-party account to sign in without a
                        password
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    <LinkedAccounts hasPassword={hasPassword} />
                </CardContent>
            </Card>
        </div>
    );
}
