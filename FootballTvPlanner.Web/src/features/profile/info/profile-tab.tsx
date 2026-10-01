import { useAuth } from '@/components/auth-provider';
import { Metadata } from '@/components/metadata';
import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from '@/components/ui/card';
import { UpdateEmailForm } from './update-email-form';
import { UpdateProfileForm } from './update-profile-form';

export function ProfileTab() {
    const { user } = useAuth();

    // RequireAuth only renders this once the user has loaded
    if (!user) {
        return null;
    }

    return (
        <div className="space-y-6">
            <Metadata title="Profile" />

            <Card>
                <CardHeader>
                    <CardTitle>Personal Information</CardTitle>
                    <CardDescription>
                        Update your personal details
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    <UpdateProfileForm name={user.name} />
                </CardContent>
            </Card>

            <Card>
                <CardHeader>
                    <CardTitle>Email Address</CardTitle>
                    <CardDescription>
                        Update your email address — a verification link will be
                        sent to confirm the change
                    </CardDescription>
                </CardHeader>
                <CardContent className="space-y-4">
                    <p className="text-sm text-muted-foreground">
                        Current email:{' '}
                        <span className="font-medium break-all text-foreground">
                            {user.email}
                        </span>
                    </p>

                    <UpdateEmailForm />
                </CardContent>
            </Card>
        </div>
    );
}
