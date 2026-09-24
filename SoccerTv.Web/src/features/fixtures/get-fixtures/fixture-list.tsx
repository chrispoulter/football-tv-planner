import type { FixtureSummary } from '../fixtures-queries';
import { FixtureCard } from '../components/fixture-card';

interface FixtureListProps {
    fixtures: FixtureSummary[];
}

/**
 * Groups a day's fixtures by competition, ordered by each competition's first kick-off.
 */
export function FixtureList({ fixtures }: FixtureListProps) {
    const groups = new Map<string, FixtureSummary[]>();

    for (const fixture of fixtures) {
        const group = groups.get(fixture.competition.id) ?? [];
        group.push(fixture);
        groups.set(fixture.competition.id, group);
    }

    return (
        <div className="space-y-6">
            {Array.from(groups.values()).map((group) => (
                <section key={group[0].competition.id} className="space-y-2">
                    <h2 className="scroll-m-20 border-b pb-2 text-xl font-semibold tracking-tight">
                        {group[0].competition.name}
                    </h2>
                    {group.map((fixture) => (
                        <FixtureCard key={fixture.id} fixture={fixture} />
                    ))}
                </section>
            ))}
        </div>
    );
}
