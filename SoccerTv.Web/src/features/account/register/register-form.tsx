import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { FormInputField } from '@/components/form/form-input-field';
import { Button } from '@/components/ui/button';
import { FieldGroup } from '@/components/ui/field';

const schema = z
    .object({
        name: z
            .string()
            .trim()
            .min(1, 'Name is required')
            .max(100, 'Name must be no more than 100 characters'),
        emailAddress: z.email('Invalid email address'),
        password: z
            .string()
            .min(8, 'Password must be at least 8 characters')
            .max(50, 'Password must be no more than 50 characters'),
        confirmPassword: z.string(),
    })
    .refine((data) => data.password === data.confirmPassword, {
        message: 'Passwords do not match',
        path: ['confirmPassword'],
    });

export type RegisterFormValues = z.infer<typeof schema>;

interface RegisterFormProps {
    loading?: boolean;
    onSubmit: (values: RegisterFormValues) => void;
}

export function RegisterForm({ loading, onSubmit }: RegisterFormProps) {
    const form = useForm<RegisterFormValues>({
        resolver: zodResolver(schema),
        defaultValues: {
            name: '',
            emailAddress: '',
            password: '',
            confirmPassword: '',
        },
    });

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

                <FormInputField
                    control={form.control}
                    name="emailAddress"
                    label="Email"
                    type="email"
                    placeholder="john@example.com"
                    maxLength={254}
                    autoComplete="username"
                    required
                />

                <FormInputField
                    control={form.control}
                    name="password"
                    label="Password"
                    type="password"
                    placeholder="••••••••"
                    maxLength={50}
                    autoComplete="new-password"
                    required
                />

                <FormInputField
                    control={form.control}
                    name="confirmPassword"
                    label="Confirm password"
                    type="password"
                    placeholder="••••••••"
                    maxLength={50}
                    autoComplete="new-password"
                    required
                />

                <Button type="submit" disabled={loading}>
                    {loading ? 'Creating Account...' : 'Create Account'}
                </Button>
            </FieldGroup>
        </form>
    );
}
