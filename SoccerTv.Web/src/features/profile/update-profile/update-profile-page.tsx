import { Link, useNavigate } from 'react-router';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { Metadata } from '@/components/metadata';
import { QueryError } from '@/components/query-error';
import { useGetProfile, useUpdateProfile } from '../profile-queries';
import {
    UpdateProfileForm,
    type UpdateProfileFormValues,
} from './update-profile-form';
import { UpdateProfileLoading } from './update-profile-loading';
import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from '@/components/ui/card';

export function UpdateProfilePage() {
    const navigate = useNavigate();

    const {
        data: profile,
        isPending,
        isFetching,
        isSuccess,
        error,
    } = useGetProfile();

    const { mutate: updateProfile, isPending: isSaving } = useUpdateProfile();

    if (isPending) {
        return <UpdateProfileLoading />;
    }

    if (!isSuccess) {
        return <QueryError error={error} />;
    }

    function onSubmit(values: UpdateProfileFormValues) {
        updateProfile(values, {
            onSuccess: () => {
                toast.success('Your profile has been updated.');
                navigate('/profile');
            },
            onError: (error) => toast.error(error.message),
        });
    }

    return (
        <div className="mx-auto w-full max-w-2xl space-y-6">
            <Metadata title="Update Profile" />

            <Card>
                <CardHeader>
                    <CardTitle className="text-2xl">Update Profile</CardTitle>
                    <CardDescription>
                        Update your personal details below. Your email address
                        is used to login to your account.
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    <UpdateProfileForm
                        profile={profile}
                        onSubmit={onSubmit}
                        disabled={isFetching || isSaving}
                        loading={isSaving}
                    >
                        <Button asChild variant="outline">
                            <Link to="/profile">Cancel</Link>
                        </Button>
                    </UpdateProfileForm>
                </CardContent>
            </Card>
        </div>
    );
}
