import { Link } from 'react-router';
import type { FallbackProps } from 'react-error-boundary';
import { Metadata } from '@/components/metadata';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Button } from '@/components/ui/button';
import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from '@/components/ui/card';

export function ErrorPage({ resetErrorBoundary }: FallbackProps) {
    return (
        <>
            <Metadata title="Error" />
            <div className="flex flex-1 items-center justify-center">
                <div className="w-full max-w-sm">
                    <Card>
                        <CardHeader>
                            <CardTitle className="text-2xl">
                                <h1>Something went wrong</h1>
                            </CardTitle>
                            <CardDescription>
                                An unexpected error occurred
                            </CardDescription>
                        </CardHeader>
                        <CardContent className="space-y-4">
                            <Alert variant="destructive">
                                <AlertDescription>
                                    Sorry, something went wrong. Please try
                                    again or return to the home page.
                                </AlertDescription>
                            </Alert>
                            <div className="flex flex-col-reverse gap-2">
                                {resetErrorBoundary && (
                                    <Button
                                        variant="outline"
                                        onClick={resetErrorBoundary}
                                    >
                                        Try again
                                    </Button>
                                )}
                                <Button asChild>
                                    <Link to="/">Home</Link>
                                </Button>
                            </div>
                        </CardContent>
                    </Card>
                </div>
            </div>
        </>
    );
}
