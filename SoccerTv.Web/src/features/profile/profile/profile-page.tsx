import { Link } from 'react-router';
import { useAuth } from '@/components/auth-provider';
import { Button } from '@/components/ui/button';
import {
    Card,
    CardContent,
    CardDescription,
    CardFooter,
    CardHeader,
    CardTitle,
} from '@/components/ui/card';
import { Metadata } from '@/components/metadata';
import { useGetTwoFactor } from '../profile-queries';
import { DeleteAccountButton } from './delete-account-button';

export function ProfilePage() {
    const { user } = useAuth();

    const { data: twoFactor } = useGetTwoFactor();

    return (
        <div className="mx-auto w-full max-w-2xl space-y-6">
            <Metadata title="My Account" />

            <div>
                <h1 className="text-2xl font-bold tracking-tight">
                    My Account
                </h1>
                <p className="text-sm text-muted-foreground">
                    Manage your account settings
                </p>
            </div>

            <Card>
                <CardHeader>
                    <CardTitle>Login Details</CardTitle>
                    <CardDescription>
                        Choose a strong password and don&apos;t reuse it for
                        other accounts. For security reasons, change your
                        password on a regular basis.
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    <dl className="space-y-1">
                        <dt className="text-sm font-medium">Email Address</dt>
                        <dd className="truncate text-sm text-muted-foreground">
                            {user?.email}
                        </dd>
                    </dl>
                </CardContent>
                <CardFooter className="flex flex-col gap-2 sm:flex-row">
                    <Button asChild className="w-full sm:w-auto">
                        <Link to="/profile/change-email">Change Email</Link>
                    </Button>
                    <Button asChild className="w-full sm:w-auto">
                        <Link to="/profile/change-password">
                            Change Password
                        </Link>
                    </Button>
                </CardFooter>
            </Card>

            <Card>
                <CardHeader>
                    <CardTitle>Two-Factor Authentication</CardTitle>
                    <CardDescription>
                        {twoFactor?.isTwoFactorEnabled
                            ? 'Enabled. A code from your authenticator app is required when you log in with your password.'
                            : 'Not enabled. Add an extra layer of security to your account.'}
                    </CardDescription>
                </CardHeader>
                <CardFooter>
                    <Button asChild className="w-full sm:w-auto">
                        <Link to="/profile/two-factor">
                            {twoFactor?.isTwoFactorEnabled
                                ? 'Manage'
                                : 'Set Up'}
                        </Link>
                    </Button>
                </CardFooter>
            </Card>

            <Card>
                <CardHeader>
                    <CardTitle>Danger Zone</CardTitle>
                    <CardDescription>
                        Once you delete your account all of your data and
                        settings will be removed. Please be certain.
                    </CardDescription>
                </CardHeader>
                <CardFooter>
                    <DeleteAccountButton className="w-full sm:w-auto" />
                </CardFooter>
            </Card>
        </div>
    );
}
