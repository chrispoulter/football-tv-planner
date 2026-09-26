import { Controller, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { REGEXP_ONLY_DIGITS } from 'input-otp';
import { LoadingButton } from '@/components/loading-button';
import { Button } from '@/components/ui/button';
import {
    Field,
    FieldError,
    FieldGroup,
    FieldLabel,
} from '@/components/ui/field';
import {
    InputOTP,
    InputOTPGroup,
    InputOTPSlot,
} from '@/components/ui/input-otp';

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
                <Controller
                    name="code"
                    control={form.control}
                    render={({ field, fieldState }) => (
                        <Field data-invalid={fieldState.invalid}>
                            <FieldLabel htmlFor={field.name}>
                                Enter the 6-digit code from your app
                            </FieldLabel>
                            <InputOTP
                                id={field.name}
                                maxLength={6}
                                pattern={REGEXP_ONLY_DIGITS}
                                autoComplete="one-time-code"
                                autoFocus
                                disabled={loading}
                                aria-invalid={fieldState.invalid}
                                {...field}
                            >
                                <InputOTPGroup>
                                    <InputOTPSlot index={0} />
                                    <InputOTPSlot index={1} />
                                    <InputOTPSlot index={2} />
                                    <InputOTPSlot index={3} />
                                    <InputOTPSlot index={4} />
                                    <InputOTPSlot index={5} />
                                </InputOTPGroup>
                            </InputOTP>
                            {fieldState.invalid && (
                                <FieldError errors={[fieldState.error]} />
                            )}
                        </Field>
                    )}
                />

                <div className="flex flex-col-reverse gap-2 sm:flex-row">
                    <Button
                        type="button"
                        variant="outline"
                        onClick={onBack}
                        disabled={loading}
                    >
                        Back
                    </Button>
                    <LoadingButton
                        type="submit"
                        loading={loading}
                        loadingText="Verifying..."
                    >
                        Verify &amp; Enable
                    </LoadingButton>
                </div>
            </FieldGroup>
        </form>
    );
}
