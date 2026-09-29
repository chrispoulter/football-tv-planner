import { useId } from 'react';
import {
    Controller,
    type Control,
    type FieldPath,
    type FieldValues,
} from 'react-hook-form';
import { Checkbox } from '@/components/ui/checkbox';
import { Field, FieldError, FieldLabel } from '@/components/ui/field';

interface FormCheckboxFieldProps<T extends FieldValues> {
    control: Control<T>;
    name: FieldPath<T>;
    label: React.ReactNode;
}

export function FormCheckboxField<T extends FieldValues>({
    control,
    name,
    label,
}: FormCheckboxFieldProps<T>) {
    const id = useId();

    return (
        <Controller
            control={control}
            name={name}
            render={({ field, fieldState }) => (
                <Field
                    orientation="horizontal"
                    data-invalid={fieldState.invalid}
                >
                    <Checkbox
                        id={id}
                        name={field.name}
                        checked={field.value}
                        onCheckedChange={field.onChange}
                        onBlur={field.onBlur}
                        ref={field.ref}
                        aria-invalid={fieldState.invalid}
                    />
                    <FieldLabel htmlFor={id} className="font-normal">
                        {label}
                    </FieldLabel>
                    {fieldState.invalid && (
                        <FieldError errors={[fieldState.error]} />
                    )}
                </Field>
            )}
        />
    );
}
