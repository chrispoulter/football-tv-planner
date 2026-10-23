import { HashedBadge } from '@/components/hashed-badge';
import { BookmarkButton } from '@/features/schedule/bookmark-button';
import { toLocalTime } from '@/lib/date-time';
import type { FixtureSummary } from './fixtures-queries';
import { AddToCalendarMenu } from './add-to-calendar-menu';

interface FixtureCardProps {
    fixture: FixtureSummary;
}

export function FixtureCard({ fixture }: FixtureCardProps) {
    return (
        <div className="flex items-start gap-4 py-3">
            <time
                dateTime={fixture.kickoffUtc}
                className="w-12 shrink-0 pt-0.5 text-lg font-semibold tabular-nums"
            >
                {toLocalTime(fixture.kickoffUtc)}
            </time>

            <div className="min-w-0 flex-1 space-y-2">
                <div className="space-y-0.5">
                    <div className="text-base font-medium">
                        {fixture.homeTeam.name}{' '}
                        <span className="text-muted-foreground">v</span>{' '}
                        {fixture.awayTeam.name}
                    </div>
                </div>

                <div className="flex flex-wrap gap-1.5">
                    {fixture.channels.map((channel) => (
                        <HashedBadge key={channel.id} label={channel.name} />
                    ))}
                </div>
            </div>

            <div className="flex shrink-0 items-center">
                <BookmarkButton
                    fixtureId={fixture.id}
                    isBookmarked={!!fixture.isBookmarked}
                />
                <AddToCalendarMenu fixture={fixture} />
            </div>
        </div>
    );
}
