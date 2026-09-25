import { Badge } from '@/components/ui/badge';
import { cn } from '@/lib/utils';
import { toLocalTime } from '@/lib/local-time';
import type { FixtureSummary } from '../fixtures-queries';
import { AddToCalendarMenu } from './add-to-calendar-menu';
import { BookmarkButton } from './bookmark-button';
import { ChannelBadge } from './channel-badge';

interface FixtureCardProps {
    fixture: FixtureSummary;
}

export function FixtureCard({ fixture }: FixtureCardProps) {
    const isOff =
        fixture.status === 'Cancelled' || fixture.status === 'Postponed';

    return (
        <div className="flex items-start gap-4 py-3">
            <div className="w-12 shrink-0 pt-0.5 text-lg font-semibold tabular-nums">
                {toLocalTime(fixture.kickoffUtc)}
            </div>

            <div className="min-w-0 flex-1 space-y-2">
                <div className="space-y-0.5">
                    <div
                        className={cn(
                            'text-base font-medium',
                            isOff && 'text-muted-foreground line-through'
                        )}
                    >
                        {fixture.homeTeam}{' '}
                        <span className="text-muted-foreground">v</span>{' '}
                        {fixture.awayTeam}
                    </div>
                </div>

                <div className="flex flex-wrap gap-1.5">
                    {isOff && (
                        <Badge variant="destructive">{fixture.status}</Badge>
                    )}
                    {fixture.channels.map((channel) => (
                        <ChannelBadge key={channel.name} channel={channel} />
                    ))}
                </div>
            </div>

            <div className="flex shrink-0 items-center">
                <BookmarkButton fixture={fixture} />
                <AddToCalendarMenu fixture={fixture} />
            </div>
        </div>
    );
}
