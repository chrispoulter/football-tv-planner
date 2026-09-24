import { Link } from 'react-router';
import { Star } from 'lucide-react';
import { Button } from '@/components/ui/button';
import {
    Empty,
    EmptyContent,
    EmptyDescription,
    EmptyHeader,
    EmptyMedia,
    EmptyTitle,
} from '@/components/ui/empty';
import { Skeleton } from '@/components/ui/skeleton';
import { Metadata } from '@/components/metadata';
import { QueryError } from '@/components/query-error';
import { toLongDayLabel, toUkDate } from '@/lib/uk-time';
import type { FixtureSummary } from '@/features/fixtures/fixtures-queries';
import { FixtureCard } from '@/features/fixtures/components/fixture-card';
import { useGetSchedule } from '../schedule-queries';
import { CalendarFeedCard } from './calendar-feed-card';

function groupByDay(fixtures: FixtureSummary[]) {
    const groups = new Map<string, FixtureSummary[]>();

    for (const fixture of fixtures) {
        const day = toUkDate(fixture.kickoffUtc);
        const group = groups.get(day) ?? [];
        group.push(fixture);
        groups.set(day, group);
    }

    return Array.from(groups.entries());
}

export function SchedulePage() {
    const { data, isPending, isSuccess, error } = useGetSchedule();

    return (
        <main className="mx-auto max-w-screen-sm space-y-6 p-6">
            <Metadata title="My Schedule" />

            <h1 className="scroll-m-20 text-4xl font-extrabold tracking-tight text-balance">
                My Schedule
            </h1>

            <CalendarFeedCard />

            {isPending ? (
                <div className="space-y-2">
                    <Skeleton className="h-9 w-1/2" />
                    <Skeleton className="h-20" />
                    <Skeleton className="h-20" />
                </div>
            ) : !isSuccess ? (
                <QueryError error={error} />
            ) : data.items.length ? (
                <div className="space-y-6">
                    {groupByDay(data.items).map(([day, fixtures]) => (
                        <section key={day} className="space-y-2">
                            <h2 className="scroll-m-20 border-b pb-2 text-xl font-semibold tracking-tight">
                                {toLongDayLabel(day)}
                            </h2>
                            {fixtures.map((fixture) => (
                                <FixtureCard
                                    key={fixture.id}
                                    fixture={fixture}
                                    showCompetition
                                />
                            ))}
                        </section>
                    ))}
                </div>
            ) : (
                <Empty className="border border-dashed">
                    <EmptyHeader>
                        <EmptyMedia variant="icon">
                            <Star />
                        </EmptyMedia>
                        <EmptyTitle>Nothing Scheduled</EmptyTitle>
                        <EmptyDescription>
                            Star games on the fixtures list to add them to your
                            schedule.
                        </EmptyDescription>
                    </EmptyHeader>
                    <EmptyContent>
                        <Button asChild>
                            <Link to="/fixtures">Browse Fixtures</Link>
                        </Button>
                    </EmptyContent>
                </Empty>
            )}
        </main>
    );
}
