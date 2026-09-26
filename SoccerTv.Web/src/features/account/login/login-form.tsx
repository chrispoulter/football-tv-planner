import { Link } from 'react-router';
import { Controller, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { TextField } from '@/components/form/text-field';
import { LoadingButton } from '@/components/loading-button';
import { Checkbox } from '@/components/ui/checkbox';
import { Field, FieldGroup, FieldLabel } from '@/components/ui/field';

const schema = z.object({
    emailAddress: z.email('Invalid email address'),
    password: z.string().min(1, 'Password is required'),
    rememberMe: z.boolean(),
});

export type LoginFormValues = z.infer<typeof schema>;

interface LoginFormProps {
    loading?: boolean;
    onSubmit: (values: LoginFormValues) => void;
}

export function LoginForm({ loading, onSubmit }: LoginFormProps) {
    const form = useForm<LoginFormValues>({
        resolver: zodResolver(schema),
        defaultValues: {
            emailAddress: '',
            password: '',
            rememberMe: false,
        },
    });

    return (
        <form noValidate onSubmit={form.handleSubmit(onSubmit)}>
            <FieldGroup>
                <TextField
                    control={form.control}
                    name="emailAddress"
                    label="Email"
                    type="email"
                    placeholder="john@example.com"
                    maxLength={254}
                    autoComplete="username"
                    required
                />

                <TextField
                    control={form.control}
                    name="password"
                    label="Password"
                    type="password"
                    placeholder="••••••••"
                    maxLength={50}
                    autoComplete="current-password"
                    required
                />

                <div className="flex items-center justify-between">
                    <Controller
                        name="rememberMe"
                        control={form.control}
                        render={({ field }) => (
                            <Field orientation="horizontal">
                                <Checkbox
                                    id={field.name}
                                    name={field.name}
                                    checked={field.value}
                                    onCheckedChange={field.onChange}
                                />
                                <FieldLabel
                                    htmlFor={field.name}
                                    className="font-normal"
                                >
                                    Remember me
                                </FieldLabel>
                            </Field>
                        )}
                    />

                    <Link
                        to="/account/forgot-password"
                        className="text-sm whitespace-nowrap text-muted-foreground underline-offset-4 hover:underline"
                    >
                        Forgot password?
                    </Link>
                </div>

                <LoadingButton
                    type="submit"
                    loading={loading}
                    loadingText="Signing In..."
                >
                    Sign In
                </LoadingButton>
            </FieldGroup>
        </form>
    );
}
