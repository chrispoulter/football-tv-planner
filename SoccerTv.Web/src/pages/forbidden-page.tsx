import { Link } from 'react-router';
import { Button } from '@/components/ui/button';
import { Metadata } from '@/components/metadata';
import { Card, CardContent } from '@/components/ui/card';

export function ForbiddenPage() {
    return (
        <div className="flex flex-1 items-center justify-center">
            <Card className="w-full max-w-md">
                <CardContent className="space-y-6">
                    <Metadata title="Forbidden" />

                    <div className="space-y-1">
                        <h1 className="text-2xl font-bold tracking-tight">
                            Forbidden
                        </h1>
                        <p className="text-sm text-muted-foreground">
                            Sorry, you do not have access to this resource.
                        </p>
                    </div>

                    <Button asChild className="w-full sm:w-auto">
                        <Link to="/">Home</Link>
                    </Button>
                </CardContent>
            </Card>
        </div>
    );
}
