import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { toast } from 'sonner';
import { FormOtpField } from '@/components/form/form-otp-field';
import { Button } from '@/components/ui/button';
import { FieldGroup } from '@/components/ui/field';
import { useEnableTwoFactor } from '../../profile-queries';

const schema = z.object({
    code: z.string().length(6, 'Code must be 6 digits'),
});

type TwoFactorVerifyFormValues = z.infer<typeof schema>;

interface TwoFactorVerifyProps {
    onSuccess: (recoveryCodes: string[]) => void;
    onCancel: () => void;
}

export function TwoFactorVerify({ onSuccess, onCancel }: TwoFactorVerifyProps) {
    const { mutate: enableTwoFactor, isPending } = useEnableTwoFactor();

    const form = useForm<TwoFactorVerifyFormValues>({
        resolver: zodResolver(schema),
        defaultValues: {
            code: '',
        },
        // HACK: prevent RHF from auto-focusing the first invalid field on submit, which breaks error state render
        shouldFocusError: false,
    });

    function onSubmit(values: TwoFactorVerifyFormValues) {
        enableTwoFactor(values, {
            onSuccess: (response) => {
                toast.success('Two-factor authentication enabled');
                onSuccess(response.recoveryCodes);
            },
            onError: (error) => toast.error(error.message),
        });
    }

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
                    <Button type="button" variant="outline" onClick={onCancel}>
                        Back
                    </Button>
                    <Button type="submit" disabled={isPending}>
                        {isPending ? 'Verifying...' : 'Verify & Enable'}
                    </Button>
                </div>
            </FieldGroup>
        </form>
    );
}
