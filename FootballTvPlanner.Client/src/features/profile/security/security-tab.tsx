import { useAuth } from '@/components/auth-provider';
import { Metadata } from '@/components/metadata';
import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { socialProviders } from '@/lib/social-providers';
import { useGetLinkedAccounts } from '../profile-queries';
import { ChangePasswordForm } from './change-password-form';
import { LinkedAccounts } from './linked-accounts';
import { SetPasswordForm } from './set-password-form';
import { TwoFactorSettings } from './two-factor/two-factor-settings';

export function SecurityTab() {
    const { user } = useAuth();

    const { data, isPending, isError } = useGetLinkedAccounts();

    const hasPassword = !!user?.hasPassword;

    return (
        <>
            <Metadata title="Security" />
            <div className="space-y-6">
                <Card>
                    <CardHeader>
                        <CardTitle>
                            <h2>
                                {hasPassword ? 'Change Password' : 'Password'}
                            </h2>
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
                            Add an extra layer of security to your account using
                            an authenticator app
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
                        {isPending ? (
                            <div className="space-y-4">
                                {socialProviders.map((provider) => (
                                    <Skeleton
                                        key={provider.id}
                                        className="h-10 w-full"
                                    />
                                ))}
                            </div>
                        ) : isError ? (
                            <p className="text-sm text-destructive">
                                Failed to load linked accounts.
                            </p>
                        ) : (
                            <LinkedAccounts
                                providers={data.providers}
                                hasPassword={hasPassword}
                            />
                        )}
                    </CardContent>
                </Card>
            </div>
        </>
    );
}
