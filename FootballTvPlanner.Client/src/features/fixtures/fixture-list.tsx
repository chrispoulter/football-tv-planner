import { useMemo } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import type { FixtureSummary } from './fixtures-queries';
import { FixtureCard } from './fixture-card';

interface FixtureListProps {
    fixtures: FixtureSummary[];
}

function groupFixturesByCompetition(fixtures: FixtureSummary[]) {
    const groups = new Map<string, FixtureSummary[]>();

    for (const fixture of fixtures) {
        const group = groups.get(fixture.competition.id) ?? [];
        group.push(fixture);
        groups.set(fixture.competition.id, group);
    }

    for (const group of groups.values()) {
        group.sort((a, b) => a.kickoffUtc.localeCompare(b.kickoffUtc));
    }

    return Array.from(groups.values()).sort((a, b) =>
        a[0].competition.name.localeCompare(b[0].competition.name)
    );
}

export function FixtureList({ fixtures }: FixtureListProps) {
    const groups = useMemo(
        () => groupFixturesByCompetition(fixtures),
        [fixtures]
    );

    return (
        <div className="space-y-4">
            {groups.map((group) => (
                <Card key={group[0].competition.id} className="gap-2">
                    <CardHeader>
                        <CardTitle>
                            <h3>{group[0].competition.name}</h3>
                        </CardTitle>
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
