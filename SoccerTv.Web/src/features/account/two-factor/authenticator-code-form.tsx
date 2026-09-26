import { Controller, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { REGEXP_ONLY_DIGITS } from 'input-otp';
import { LoadingButton } from '@/components/loading-button';
import { Button } from '@/components/ui/button';
import { Checkbox } from '@/components/ui/checkbox';
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
    rememberMachine: z.boolean(),
});

export type AuthenticatorCodeFormValues = z.infer<typeof schema>;

interface AuthenticatorCodeFormProps {
    loading?: boolean;
    onSubmit: (values: AuthenticatorCodeFormValues) => void;
    onUseRecoveryCode: () => void;
}

export function AuthenticatorCodeForm({
    loading,
    onSubmit,
    onUseRecoveryCode,
}: AuthenticatorCodeFormProps) {
    const form = useForm<AuthenticatorCodeFormValues>({
        resolver: zodResolver(schema),
        defaultValues: {
            code: '',
            rememberMachine: false,
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
                        <Field
                            data-invalid={fieldState.invalid}
                            className="items-center *:w-auto"
                        >
                            <FieldLabel htmlFor={field.name}>
                                Authentication code
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

                <Controller
                    name="rememberMachine"
                    control={form.control}
                    render={({ field }) => (
                        <Field orientation="horizontal">
                            <Checkbox
                                id={field.name}
                                name={field.name}
                                checked={field.value}
                                onCheckedChange={field.onChange}
                                disabled={loading}
                            />
                            <FieldLabel
                                htmlFor={field.name}
                                className="font-normal"
                            >
                                Don&apos;t ask again on this browser
                            </FieldLabel>
                        </Field>
                    )}
                />

                <LoadingButton type="submit" loading={loading}>
                    Verify
                </LoadingButton>
                <Button
                    type="button"
                    variant="link"
                    onClick={onUseRecoveryCode}
                    disabled={loading}
                >
                    Use Recovery Code Instead
                </Button>
            </FieldGroup>
        </form>
    );
}
