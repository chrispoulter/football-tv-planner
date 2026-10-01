import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import type { FixtureSummary } from '../fixtures-queries';
import { FixtureCard } from '../fixture-card';

interface FixtureListProps {
    fixtures: FixtureSummary[];
}

export function FixtureList({ fixtures }: FixtureListProps) {
    const groups = new Map<string, FixtureSummary[]>();

    for (const fixture of fixtures) {
        const group = groups.get(fixture.competition) ?? [];
        group.push(fixture);
        groups.set(fixture.competition, group);
    }

    return (
        <div className="space-y-4">
            {Array.from(groups.values()).map((group) => (
                <Card key={group[0].competition} className="gap-2">
                    <CardHeader>
                        <CardTitle>{group[0].competition}</CardTitle>
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
