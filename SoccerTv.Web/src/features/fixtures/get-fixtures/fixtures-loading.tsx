import { Skeleton } from '@/components/ui/skeleton';

export function FixturesLoading() {
    return (
        <div className="space-y-2">
            <Skeleton className="h-9 w-1/2" />
            <Skeleton className="h-20" />
            <Skeleton className="h-20" />
            <Skeleton className="h-20" />
            <Skeleton className="h-20" />
        </div>
    );
}
