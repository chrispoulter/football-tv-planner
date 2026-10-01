import { Link } from 'react-router';
import { cn } from '@/lib/utils';
import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from '@/components/ui/card';

interface AccountLayoutProps {
    title: React.ReactNode;
    description?: React.ReactNode;
    footer?: React.ReactNode;
    className?: string;
    children: React.ReactNode;
}

export function AccountLayout({
    title,
    description,
    footer,
    className,
    children,
}: AccountLayoutProps) {
    return (
        <div className="flex flex-1 items-center justify-center">
            <div className={cn('w-full max-w-sm space-y-6', className)}>
                <Card>
                    <CardHeader>
                        <CardTitle className="text-2xl">{title}</CardTitle>
                        {description && (
                            <CardDescription>{description}</CardDescription>
                        )}
                    </CardHeader>
                    <CardContent className="space-y-4">{children}</CardContent>
                </Card>

                {footer && (
                    <p className="text-center text-sm text-muted-foreground">
                        {footer}
                    </p>
                )}
            </div>
        </div>
    );
}

export function AccountLink({
    className,
    ...props
}: React.ComponentProps<typeof Link>) {
    return (
        <Link
            className={cn(
                'underline underline-offset-4 hover:text-foreground',
                className
            )}
            {...props}
        />
    );
}
