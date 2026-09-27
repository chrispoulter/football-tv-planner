import { Card, CardContent, CardHeader } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { FixtureCardSkeleton } from '../fixture-card-skeleton';

// Fixtures per competition, so the placeholder looks like a typical day
const GROUP_SIZES = [3, 2, 1];

/**
 * Matches the layout of a FixtureList: a card of fixtures per competition.
 */
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
