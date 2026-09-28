import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { toast } from 'sonner';
import { FormInputField } from '@/components/form/form-input-field';
import { Button } from '@/components/ui/button';
import { FieldGroup } from '@/components/ui/field';
import { useUpdateProfile } from '../profile-queries';

const schema = z.object({
    name: z
        .string()
        .trim()
        .min(1, 'Name is required')
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
                toast.success('Profile updated');
                form.reset(values);
            },
            onError: (error) => toast.error(error.message),
        });
    }

    return (
        <form noValidate onSubmit={form.handleSubmit(onSubmit)}>
            <FieldGroup>
                <FormInputField
                    control={form.control}
                    name="name"
                    label="Name"
                    placeholder="John Smith"
                    maxLength={100}
                    autoComplete="name"
                    required
                />

                <div className="flex flex-col-reverse gap-2 sm:flex-row">
                    <Button type="submit" disabled={isPending}>
                        {isPending ? 'Saving...' : 'Save Changes'}
                    </Button>
                </div>
            </FieldGroup>
        </form>
    );
}
