import { useState } from 'react';
import { useSearchParams } from 'react-router';
import { useSwipeable, type SwipeEventData } from 'react-swipeable';
import { z } from 'zod';
import { Star, Tv } from 'lucide-react';
import { useAuth } from '@/components/auth-provider';
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert';
import { Button } from '@/components/ui/button';
import {
    Empty,
    EmptyContent,
    EmptyDescription,
    EmptyHeader,
    EmptyMedia,
    EmptyTitle,
} from '@/components/ui/empty';
import { DayStrip } from '@/components/day-strip';
import { Metadata } from '@/components/metadata';
import {
    addDays,
    isDateString,
    toLongDayLabel,
    todayLocal,
    toUtcDayRange,
} from '@/lib/date-time';
import { useGetFixtures } from './fixtures-queries';
import { FixtureFilters } from './fixture-filters';
import { FixtureList } from './fixture-list';
import { FixturesLoading } from './fixtures-loading';

// Ignore swipes starting near the screen edges, which trigger browser back/forward.
const SWIPE_EDGE_PX = 24;

function isEdgeSwipe({ initial: [x] }: SwipeEventData) {
    return x < SWIPE_EDGE_PX || x > window.innerWidth - SWIPE_EDGE_PX;
}

const searchParamsSchema = z.object({
    date: z
        .string()
        .refine(isDateString)
        .catch(() => todayLocal()),
    mine: z
        .string()
        .optional()
        .transform((value) => value === 'true')
        .catch(false),
});

export function FixturesPage() {
    const [searchParams, setSearchParams] = useSearchParams();

    const { user, isLoading: isAuthLoading } = useAuth();

    const request = searchParamsSchema.parse(Object.fromEntries(searchParams));

    const [competitionId, setCompetitionId] = useState<string>();
    const [channelId, setChannelId] = useState<string>();

    const mine = request.mine && !!user;

    const { data, isPending, isPlaceholderData, isSuccess } = useGetFixtures(
        {
            ...toUtcDayRange(request.date),
            competitionId,
            channelId,
            bookmarked: mine,
        },
        { enabled: !(request.mine && isAuthLoading) }
    );

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

    function changeDate(date: string) {
        setParam('date', date);
        window.scrollTo({ top: 0, behavior: 'smooth' });
    }

    const swipeHandlers = useSwipeable({
        onSwipedLeft: (event) => {
            if (!isEdgeSwipe(event)) {
                changeDate(addDays(request.date, 1));
            }
        },
        onSwipedRight: (event) => {
            if (!isEdgeSwipe(event) && request.date > todayLocal()) {
                changeDate(addDays(request.date, -1));
            }
        },
        delta: 60,
    });

    return (
        <div className="space-y-6">
            <Metadata title="Fixtures" />

            <h1 className="text-2xl font-bold tracking-tight">Fixtures</h1>

            <div className="sticky top-14 z-40 -mx-4 border-b bg-background/95 px-4 py-2 backdrop-blur supports-backdrop-filter:bg-background/60">
                <DayStrip date={request.date} onChange={changeDate} />
            </div>

            <FixtureFilters
                competitionId={competitionId}
                channelId={channelId}
                onCompetitionChange={setCompetitionId}
                onChannelChange={setChannelId}
                mine={mine}
                onMineChange={(value) =>
                    setParam('mine', value ? 'true' : undefined)
                }
            />

            <div {...swipeHandlers} className="space-y-6">
                <h2 className="scroll-mt-32 text-lg font-semibold">
                    {toLongDayLabel(request.date)}
                </h2>

                {isPending || (isPlaceholderData && !data.items.length) ? (
                    <FixturesLoading />
                ) : !isSuccess ? (
                    <Alert variant="destructive">
                        <AlertTitle>Error</AlertTitle>
                        <AlertDescription>
                            Failed to load fixtures. Please try again.
                        </AlertDescription>
                    </Alert>
                ) : data.items.length ? (
                    <div
                        className={isPlaceholderData ? 'opacity-60' : undefined}
                    >
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
        </div>
    );
}
