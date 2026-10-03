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
                        <h2>{hasPassword ? 'Change Password' : 'Password'}</h2>
                    </CardTitle>
                    <CardDescription>
                        {hasPassword
                            ? 'Update your password'
                            : 'Add a password to your account'}
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    {hasPassword ? (
                        <ChangePasswordForm key="change" />
                    ) : (
                        <SetPasswordForm key="set" />
                    )}
                </CardContent>
            </Card>

            <Card>
                <CardHeader>
                    <CardTitle>
                        <h2>Two-Factor Authentication</h2>
                    </CardTitle>
                    <CardDescription>
                        Add an extra layer of security to your account using an
                        authenticator app
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    <TwoFactorSettings />
                </CardContent>
            </Card>

            <Card>
                <CardHeader>
                    <CardTitle>
                        <h2>Linked Accounts</h2>
                    </CardTitle>
                    <CardDescription>
                        Connect your account to a third-party provider for
                        passwordless sign-in
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    <LinkedAccounts hasPassword={hasPassword} />
                </CardContent>
            </Card>
        </div>
    );
}
