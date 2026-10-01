import { Skeleton } from '@/components/ui/skeleton';

export function FixtureCardSkeleton() {
    return (
        <div className="flex items-start gap-4 py-3">
            <Skeleton className="mt-1 h-5 w-12 shrink-0" />

            <div className="min-w-0 flex-1 space-y-2">
                <Skeleton className="mt-1 h-5 w-3/4 max-w-72" />

                <div className="flex gap-1.5">
                    <Skeleton className="h-5 w-20 rounded-full" />
                    <Skeleton className="h-5 w-16 rounded-full" />
                </div>
            </div>

            <div className="flex shrink-0 gap-1">
                <Skeleton className="size-9 rounded-md" />
                <Skeleton className="size-9 rounded-md" />
            </div>
        </div>
    );
}
