import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { FormOtpField } from '@/components/form/form-otp-field';
import { Button } from '@/components/ui/button';
import { FieldGroup } from '@/components/ui/field';

const schema = z.object({
    code: z.string().length(6, 'Code must be 6 digits'),
});

export type TwoFactorVerifyFormValues = z.infer<typeof schema>;

interface TwoFactorVerifyFormProps {
    loading?: boolean;
    onSubmit: (values: TwoFactorVerifyFormValues) => void;
    onBack: () => void;
}

export function TwoFactorVerifyForm({
    loading,
    onSubmit,
    onBack,
}: TwoFactorVerifyFormProps) {
    const form = useForm<TwoFactorVerifyFormValues>({
        resolver: zodResolver(schema),
        defaultValues: {
            code: '',
        },
        // Focusing the invalid code input on submit stops its error showing
        shouldFocusError: false,
    });

    return (
        <form noValidate onSubmit={form.handleSubmit(onSubmit)}>
            <FieldGroup>
                <FormOtpField
                    control={form.control}
                    name="code"
                    label="Enter the 6-digit code from your app"
                    autoFocus
                />

                <div className="flex flex-col-reverse gap-2 sm:flex-row">
                    <Button type="button" variant="outline" onClick={onBack}>
                        Back
                    </Button>
                    <Button type="submit" disabled={loading}>
                        {loading ? 'Verifying...' : 'Verify & Enable'}
                    </Button>
                </div>
            </FieldGroup>
        </form>
    );
}
