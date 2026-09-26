import { Button } from '@/components/ui/button';

interface LoadingButtonProps extends React.ComponentProps<typeof Button> {
    loading?: boolean;
    // Shown in place of the label while loading, e.g. "Saving..."
    loadingText: string;
}

export function LoadingButton({
    loading,
    loadingText,
    children,
    disabled,
    ...rest
}: LoadingButtonProps) {
    return (
        <Button {...rest} disabled={disabled || loading}>
            {loading ? loadingText : children}
        </Button>
    );
}
