import { Skeleton } from '@/components/ui/skeleton';

export function FixturesLoading() {
    return (
        <div className="gap-4 lg:columns-2 2xl:columns-3">
            {[3, 2, 2, 1].map((rows, i) => (
                <Skeleton
                    key={i}
                    className="mb-4 break-inside-avoid rounded-xl"
                    style={{ height: `${4 + rows * 4.5}rem` }}
                />
            ))}
        </div>
    );
}
