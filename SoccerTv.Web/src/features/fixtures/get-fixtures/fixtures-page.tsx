import { useSearchParams } from 'react-router';
import { z } from 'zod';
import { Tv } from 'lucide-react';
import {
    Empty,
    EmptyDescription,
    EmptyHeader,
    EmptyMedia,
    EmptyTitle,
} from '@/components/ui/empty';
import { Metadata } from '@/components/metadata';
import { QueryError } from '@/components/query-error';
import { isUkDate, toLongDayLabel, todayUk } from '@/lib/uk-time';
import { useGetFixtures } from '../fixtures-queries';
import { DayStrip } from './day-strip';
import { FixtureFilters } from './fixture-filters';
import { FixtureList } from './fixture-list';
import { FixturesLoading } from './fixtures-loading';

const searchParamsSchema = z.object({
    date: z
        .string()
        .refine(isUkDate)
        .catch(() => todayUk()),
    competition: z.string().optional().catch(undefined),
    provider: z.string().optional().catch(undefined),
});

export function FixturesPage() {
    const [searchParams, setSearchParams] = useSearchParams();

    const request = searchParamsSchema.parse(Object.fromEntries(searchParams));

    const { data, isPending, isPlaceholderData, isSuccess, error } =
        useGetFixtures({
            date: request.date,
            competitionId: request.competition,
            provider: request.provider,
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
        <main className="mx-auto max-w-screen-sm space-y-6 p-6">
            <Metadata title="Football on TV" />

            <h1 className="scroll-m-20 text-4xl font-extrabold tracking-tight text-balance">
                Football on TV
            </h1>

            <DayStrip
                date={request.date}
                onChange={(date) => setParam('date', date)}
            />

            <FixtureFilters
                competitionId={request.competition}
                provider={request.provider}
                onCompetitionChange={(value) => setParam('competition', value)}
                onProviderChange={(value) => setParam('provider', value)}
            />

            <h2 className="scroll-m-20 text-2xl font-semibold tracking-tight">
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
        </main>
    );
}
