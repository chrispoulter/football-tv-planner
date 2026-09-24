import { Skeleton } from '@/components/ui/skeleton';

export function FixturesLoading() {
    return (
        <div className="space-y-4">
            {[3, 2, 2, 1].map((rows, i) => (
                <Skeleton
                    key={i}
                    className="rounded-xl"
                    style={{ height: `${4 + rows * 4.5}rem` }}
                />
            ))}
        </div>
    );
}
