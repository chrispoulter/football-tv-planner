import { Skeleton } from '@/components/ui/skeleton';

export function UpdateProfileLoading() {
    return (
        <div className="mx-auto w-full max-w-2xl space-y-6">
            <Skeleton className="h-[30rem] rounded-xl" />
        </div>
    );
}
