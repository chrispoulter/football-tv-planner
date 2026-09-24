import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import type { FixtureSummary } from '../fixtures-queries';
import { FixtureCard } from '../components/fixture-card';

interface FixtureListProps {
    fixtures: FixtureSummary[];
}

/**
 * Groups a day's fixtures by competition, ordered by each competition's first kick-off.
 * Each competition is a card, and the cards flow into as many columns as fit.
 */
export function FixtureList({ fixtures }: FixtureListProps) {
    const groups = new Map<string, FixtureSummary[]>();

    for (const fixture of fixtures) {
        const group = groups.get(fixture.competition.id) ?? [];
        group.push(fixture);
        groups.set(fixture.competition.id, group);
    }

    return (
        <div className="gap-4 lg:columns-2 2xl:columns-3">
            {Array.from(groups.values()).map((group) => (
                <Card
                    key={group[0].competition.id}
                    className="mb-4 break-inside-avoid gap-2"
                >
                    <CardHeader>
                        <CardTitle>{group[0].competition.name}</CardTitle>
                    </CardHeader>
                    <CardContent className="divide-y">
                        {group.map((fixture) => (
                            <FixtureCard key={fixture.id} fixture={fixture} />
                        ))}
                    </CardContent>
                </Card>
            ))}
        </div>
    );
}
