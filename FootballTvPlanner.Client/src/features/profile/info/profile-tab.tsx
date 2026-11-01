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
import { VerifyEmailForm } from './verify-email-form';

export function ProfileTab() {
    const { user } = useAuth();

    return (
        <>
            <Metadata title="Profile" />
            <div className="space-y-6">
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
                        <UpdateProfileForm />
                    </CardContent>
                </Card>

                {user && !user.isEmailConfirmed && (
                    <Card>
                        <CardHeader>
                            <CardTitle>
                                <h2>Verify Email Address</h2>
                            </CardTitle>
                            <CardDescription>
                                Verify your email address so you can recover
                                your account if you forget your password
                            </CardDescription>
                        </CardHeader>
                        <CardContent>
                            <VerifyEmailForm />
                        </CardContent>
                    </Card>
                )}

                <Card>
                    <CardHeader>
                        <CardTitle>
                            <h2>Update Email Address</h2>
                        </CardTitle>
                        <CardDescription>
                            Update your email address — a verification link will
                            be sent to confirm the change
                        </CardDescription>
                    </CardHeader>
                    <CardContent>
                        <UpdateEmailForm />
                    </CardContent>
                </Card>

                <Card>
                    <CardHeader>
                        <CardTitle>
                            <h2>Your Data</h2>
                        </CardTitle>
                        <CardDescription>
                            Download a copy of your personal data, including
                            your bookmarked fixtures
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
        </>
    );
}
