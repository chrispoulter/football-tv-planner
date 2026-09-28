import { useId } from 'react';
import {
    Controller,
    type Control,
    type FieldPath,
    type FieldValues,
} from 'react-hook-form';
import { Input } from '@/components/ui/input';
import { Field, FieldError, FieldLabel } from '@/components/ui/field';

type FormInputFieldProps<T extends FieldValues> = {
    control: Control<T>;
    name: FieldPath<T>;
    label: React.ReactNode;
} & Omit<
    React.ComponentProps<typeof Input>,
    'name' | 'id' | 'value' | 'onChange' | 'onBlur' | 'ref'
>;

export function FormInputField<T extends FieldValues>({
    control,
    name,
    label,
    ...inputProps
}: FormInputFieldProps<T>) {
    const id = useId();

    return (
        <Controller
            control={control}
            name={name}
            render={({ field, fieldState }) => (
                <Field data-invalid={fieldState.invalid}>
                    <FieldLabel htmlFor={id}>{label}</FieldLabel>
                    <Input
                        id={id}
                        aria-invalid={fieldState.invalid}
                        {...inputProps}
                        {...field}
                    />
                    {fieldState.invalid && (
                        <FieldError errors={[fieldState.error]} />
                    )}
                </Field>
            )}
        />
    );
}
