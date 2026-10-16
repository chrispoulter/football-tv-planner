import { Download } from 'lucide-react';
import { useAuth } from '@/components/auth-provider';
import { Metadata } from '@/components/metadata';
import { Button } from '@/components/ui/button';
import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from '@/components/ui/card';
import { personalDataUrl } from '../profile-queries';
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
                    <CardTitle>
                        <h2>Personal Information</h2>
                    </CardTitle>
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
                    <CardTitle>
                        <h2>Email Address</h2>
                    </CardTitle>
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

            <Card>
                <CardHeader>
                    <CardTitle>
                        <h2>Your Data</h2>
                    </CardTitle>
                    <CardDescription>
                        Download a copy of your personal data, including your
                        bookmarked fixtures
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    <Button
                        asChild
                        variant="outline"
                        className="w-full sm:w-auto"
                    >
                        <a href={personalDataUrl} download>
                            <Download />
                            Download your data
                        </a>
                    </Button>
                </CardContent>
            </Card>
        </div>
    );
}
