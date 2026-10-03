import { Link } from 'react-router';
import { Metadata } from '@/components/metadata';
import { Button } from '@/components/ui/button';
import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from '@/components/ui/card';

export function NotFoundPage() {
    return (
        <>
            <Metadata title="Page Not Found" />
            <div className="flex flex-1 items-center justify-center">
                <div className="w-full max-w-sm">
                    <Card>
                        <CardHeader>
                            <CardTitle className="text-2xl">
                                <h1>Page not found</h1>
                            </CardTitle>
                            <CardDescription>
                                The page you&apos;re looking for doesn&apos;t
                                exist.
                            </CardDescription>
                        </CardHeader>
                        <CardContent>
                            <Button asChild className="w-full">
                                <Link to="/">Home</Link>
                            </Button>
                        </CardContent>
                    </Card>
                </div>
            </div>
        </>
    );
}
