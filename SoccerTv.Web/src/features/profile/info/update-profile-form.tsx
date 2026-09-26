import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { toast } from 'sonner';
import { TextField } from '@/components/form/text-field';
import { LoadingButton } from '@/components/loading-button';
import { FieldGroup } from '@/components/ui/field';
import { useUpdateProfile } from '../profile-queries';

const schema = z.object({
    name: z
        .string({ message: 'Name must be a valid string' })
        .trim()
        .min(1, 'Name is a required field')
        .max(100, 'Name must be no more than 100 characters'),
});

type UpdateProfileFormValues = z.infer<typeof schema>;

interface UpdateProfileFormProps {
    name?: string;
}

export function UpdateProfileForm({ name }: UpdateProfileFormProps) {
    const { mutate: updateProfile, isPending } = useUpdateProfile();

    const form = useForm<UpdateProfileFormValues>({
        resolver: zodResolver(schema),
        defaultValues: {
            name: name ?? '',
        },
    });

    function onSubmit(values: UpdateProfileFormValues) {
        updateProfile(values, {
            onSuccess: () => {
                toast.success('Your profile has been updated.');
                form.reset(values);
            },
            onError: (error) => toast.error(error.message),
        });
    }

    return (
        <form noValidate onSubmit={form.handleSubmit(onSubmit)}>
            <FieldGroup>
                <TextField
                    control={form.control}
                    name="name"
                    label="Name"
                    placeholder="John Smith"
                    maxLength={100}
                    autoComplete="name"
                    required
                    disabled={isPending}
                />

                <div className="flex flex-col-reverse gap-2 sm:flex-row">
                    <LoadingButton type="submit" loading={isPending}>
                        Save Changes
                    </LoadingButton>
                </div>
            </FieldGroup>
        </form>
    );
}
