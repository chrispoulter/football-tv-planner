import { Spinner } from '@/components/ui/spinner';

export function PageLoading() {
    return (
        <div className="flex flex-1 items-center justify-center">
            <Spinner className="h-6 w-6 text-muted-foreground" />
        </div>
    );
}
