import { Card, CardContent, CardHeader } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { FixtureCardSkeleton } from '../fixture-card-skeleton';

const GROUP_SIZES = [3, 2, 1];

export function FixturesLoading() {
    return (
        <div className="space-y-4">
            {GROUP_SIZES.map((size, i) => (
                <Card key={i} className="gap-2">
                    <CardHeader>
                        <Skeleton className="h-5 w-40" />
                    </CardHeader>
                    <CardContent className="divide-y">
                        {Array.from({ length: size }, (_, j) => (
                            <FixtureCardSkeleton key={j} />
                        ))}
                    </CardContent>
                </Card>
            ))}
        </div>
    );
}
