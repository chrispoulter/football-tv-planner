import { useId } from 'react';
import {
    Controller,
    type Control,
    type FieldPath,
    type FieldValues,
} from 'react-hook-form';
import { REGEXP_ONLY_DIGITS } from 'input-otp';
import { Field, FieldError, FieldLabel } from '@/components/ui/field';
import {
    InputOTP,
    InputOTPGroup,
    InputOTPSlot,
} from '@/components/ui/input-otp';

interface FormOtpFieldProps<T extends FieldValues> {
    control: Control<T>;
    name: FieldPath<T>;
    label: React.ReactNode;
    length?: number;
    autoFocus?: boolean;
    className?: string;
}

export function FormOtpField<T extends FieldValues>({
    control,
    name,
    label,
    length = 6,
    autoFocus,
    className,
}: FormOtpFieldProps<T>) {
    const id = useId();

    return (
        <Controller
            control={control}
            name={name}
            render={({ field, fieldState }) => (
                <Field data-invalid={fieldState.invalid} className={className}>
                    <FieldLabel htmlFor={id}>{label}</FieldLabel>
                    <InputOTP
                        id={id}
                        maxLength={length}
                        pattern={REGEXP_ONLY_DIGITS}
                        autoComplete="one-time-code"
                        autoFocus={autoFocus}
                        aria-invalid={fieldState.invalid}
                        {...field}
                    >
                        <InputOTPGroup>
                            {Array.from({ length }, (_, index) => (
                                <InputOTPSlot key={index} index={index} />
                            ))}
                        </InputOTPGroup>
                    </InputOTP>
                    {fieldState.invalid && (
                        <FieldError errors={[fieldState.error]} />
                    )}
                </Field>
            )}
        />
    );
}
