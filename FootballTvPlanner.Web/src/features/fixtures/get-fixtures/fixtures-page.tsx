import { useSearchParams } from 'react-router';
import { z } from 'zod';
import { Star, Tv } from 'lucide-react';
import { useAuth } from '@/components/auth-provider';
import { Button } from '@/components/ui/button';
import {
    Empty,
    EmptyContent,
    EmptyDescription,
    EmptyHeader,
    EmptyMedia,
    EmptyTitle,
} from '@/components/ui/empty';
import { Metadata } from '@/components/metadata';
import { QueryError } from '@/components/query-error';
import {
    isDateString,
    toLongDayLabel,
    todayLocal,
    toUtcDayRange,
} from '@/lib/local-time';
import { useGetFixtures } from '../fixtures-queries';
import { DayStrip } from './day-strip';
import { FixtureFilters } from './fixture-filters';
import { FixtureList } from './fixture-list';
import { FixturesLoading } from './fixtures-loading';

const searchParamsSchema = z.object({
    date: z
        .string()
        .refine(isDateString)
        .catch(() => todayLocal()),
    competition: z.string().optional().catch(undefined),
    channel: z.string().optional().catch(undefined),
    mine: z
        .string()
        .optional()
        .transform((value) => value === 'true')
        .catch(false),
});

export function FixturesPage() {
    const [searchParams, setSearchParams] = useSearchParams();

    const { user } = useAuth();

    const request = searchParamsSchema.parse(Object.fromEntries(searchParams));

    const mine = request.mine && !!user;

    const { data, isPending, isPlaceholderData, isSuccess, error } =
        useGetFixtures({
            ...toUtcDayRange(request.date),
            competition: request.competition,
            channel: request.channel,
            bookmarked: mine,
        });

    function setParam(name: string, value?: string) {
        setSearchParams((prev) => {
            if (value) {
                prev.set(name, value);
            } else {
                prev.delete(name);
            }

            return prev;
        });
    }

    return (
        <div className="space-y-6">
            <Metadata title="Fixtures" />

            <h1 className="text-2xl font-bold tracking-tight">Fixtures</h1>

            <DayStrip
                date={request.date}
                onChange={(date) => setParam('date', date)}
            />

            <FixtureFilters
                competition={request.competition}
                channel={request.channel}
                onCompetitionChange={(value) => setParam('competition', value)}
                onChannelChange={(value) => setParam('channel', value)}
                mine={mine}
                onMineChange={(value) =>
                    setParam('mine', value ? 'true' : undefined)
                }
            />

            <h2 className="text-lg font-semibold">
                {toLongDayLabel(request.date)}
            </h2>

            {isPending ? (
                <FixturesLoading />
            ) : !isSuccess ? (
                <QueryError error={error} />
            ) : data.items.length ? (
                <div className={isPlaceholderData ? 'opacity-60' : undefined}>
                    <FixtureList fixtures={data.items} />
                </div>
            ) : mine ? (
                <Empty className="border border-dashed">
                    <EmptyHeader>
                        <EmptyMedia variant="icon">
                            <Star />
                        </EmptyMedia>
                        <EmptyTitle>Nothing Scheduled</EmptyTitle>
                        <EmptyDescription>
                            No starred games on this day.
                        </EmptyDescription>
                    </EmptyHeader>
                    <EmptyContent>
                        <Button onClick={() => setParam('mine', undefined)}>
                            Show All Games
                        </Button>
                    </EmptyContent>
                </Empty>
            ) : (
                <Empty className="border border-dashed">
                    <EmptyHeader>
                        <EmptyMedia variant="icon">
                            <Tv />
                        </EmptyMedia>
                        <EmptyTitle>No Games</EmptyTitle>
                        <EmptyDescription>
                            No televised games found for this day.
                        </EmptyDescription>
                    </EmptyHeader>
                </Empty>
            )}
        </div>
    );
}
