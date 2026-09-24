import { Skeleton } from '@/components/ui/skeleton';

export function ProfileLoading() {
    return (
        <div className="mx-auto w-full max-w-2xl space-y-6">
            <div className="space-y-2">
                <Skeleton className="h-8 w-48" />
                <Skeleton className="h-4 w-64" />
            </div>

            <Skeleton className="h-56 rounded-xl" />
            <Skeleton className="h-40 rounded-xl" />
            <Skeleton className="h-40 rounded-xl" />
        </div>
    );
}
