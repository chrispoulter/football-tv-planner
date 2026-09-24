import { Link } from 'react-router';
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
import { QueryError } from '@/components/query-error';
import { toDisplay } from '@/lib/dates';
import { toReminderLabel } from '@/lib/reminders';
import { useGetProfile } from '../profile-queries';
import { DeleteAccountButton } from './delete-account-button';
import { ProfileLoading } from './profile-loading';

export function ProfilePage() {
    const {
        data: profile,
        isPending,
        isFetching,
        isSuccess,
        error,
    } = useGetProfile();

    if (isPending) {
        return <ProfileLoading />;
    }

    if (!isSuccess) {
        return <QueryError error={error} />;
    }

    const details = [
        { label: 'Email Address', value: profile.emailAddress },
        { label: 'Name', value: `${profile.firstName} ${profile.lastName}` },
        { label: 'Date Of Birth', value: toDisplay(profile.dateOfBirth) },
        {
            label: 'Reminder Before Kick-off',
            value: toReminderLabel(profile.reminderMinutesBefore ?? 0),
        },
    ];

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
                    <CardTitle>Personal Details</CardTitle>
                </CardHeader>
                <CardContent>
                    <dl className="grid gap-4 sm:grid-cols-2">
                        {details.map(({ label, value }) => (
                            <div key={label} className="space-y-1">
                                <dt className="text-sm font-medium">{label}</dt>
                                <dd className="truncate text-sm text-muted-foreground">
                                    {value}
                                </dd>
                            </div>
                        ))}
                    </dl>
                </CardContent>
                <CardFooter>
                    <Button asChild className="w-full sm:w-auto">
                        <Link to="/profile/update-profile">Update Profile</Link>
                    </Button>
                </CardFooter>
            </Card>

            <Card>
                <CardHeader>
                    <CardTitle>Login Details</CardTitle>
                    <CardDescription>
                        Choose a strong password and don&apos;t reuse it for
                        other accounts. For security reasons, change your
                        password on a regular basis.
                    </CardDescription>
                </CardHeader>
                <CardFooter>
                    <Button asChild className="w-full sm:w-auto">
                        <Link to="/profile/change-password">
                            Change Password
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
                    <DeleteAccountButton
                        disabled={isFetching}
                        className="w-full sm:w-auto"
                    />
                </CardFooter>
            </Card>
        </div>
    );
}
