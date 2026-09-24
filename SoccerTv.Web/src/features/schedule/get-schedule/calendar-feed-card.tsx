import { Copy } from 'lucide-react';
import { toast } from 'sonner';
import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
    AlertDialogTrigger,
} from '@/components/ui/alert-dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Skeleton } from '@/components/ui/skeleton';
import { LoadingButton } from '@/components/loading-button';
import {
    googleSubscribeUrl,
    outlook365SubscribeUrl,
    outlookComSubscribeUrl,
} from '@/lib/calendar-links';
import { useGetCalendarFeed, useResetCalendarFeed } from '../schedule-queries';

const CALENDAR_NAME = 'My Football on TV';

export function CalendarFeedCard() {
    const { data: feed, isPending, isSuccess } = useGetCalendarFeed();

    const { mutate: resetFeed, isPending: isResetting } =
        useResetCalendarFeed();

    if (isPending) {
        return <Skeleton className="h-48" />;
    }

    if (!isSuccess) {
        return null;
    }

    async function onCopy() {
        await navigator.clipboard.writeText(feed!.httpsUrl);
        toast.success('Calendar link copied.');
    }

    function onReset() {
        resetFeed(undefined, {
            onSuccess: () =>
                toast.success(
                    'A new calendar link has been created. Re-subscribe using the new link.'
                ),
            onError: (error) => toast.error(error.message),
        });
    }

    return (
        <section className="space-y-4 rounded-lg border p-4">
            <div className="space-y-1">
                <h2 className="text-lg font-semibold">
                    Subscribe in your calendar
                </h2>
                <p className="text-sm text-muted-foreground">
                    Games you star appear in your calendar automatically, with a
                    reminder before kick-off. Keep this link private.
                </p>
            </div>

            <div className="flex gap-2">
                <Input
                    readOnly
                    value={feed.httpsUrl}
                    aria-label="Calendar subscription link"
                    onFocus={(e) => e.currentTarget.select()}
                />
                <Button variant="outline" size="icon" onClick={onCopy}>
                    <Copy />
                    <span className="sr-only">Copy link</span>
                </Button>
            </div>

            <div className="flex flex-col gap-2 sm:flex-row sm:flex-wrap">
                <Button asChild variant="secondary">
                    <a
                        href={googleSubscribeUrl(feed.webcalUrl)}
                        target="_blank"
                        rel="noreferrer"
                    >
                        Google Calendar
                    </a>
                </Button>
                <Button asChild variant="secondary">
                    <a
                        href={outlookComSubscribeUrl(
                            feed.httpsUrl,
                            CALENDAR_NAME
                        )}
                        target="_blank"
                        rel="noreferrer"
                    >
                        Outlook.com
                    </a>
                </Button>
                <Button asChild variant="secondary">
                    <a
                        href={outlook365SubscribeUrl(
                            feed.httpsUrl,
                            CALENDAR_NAME
                        )}
                        target="_blank"
                        rel="noreferrer"
                    >
                        Outlook (Microsoft 365)
                    </a>
                </Button>
                <Button asChild variant="secondary">
                    <a href={feed.webcalUrl}>Apple / other</a>
                </Button>
            </div>

            <p className="text-xs text-muted-foreground">
                Google Calendar can take several hours to pick up changes to
                subscribed calendars. Outlook usually updates within a few
                hours.
            </p>

            <AlertDialog>
                <AlertDialogTrigger asChild>
                    <LoadingButton
                        variant="link"
                        loading={isResetting}
                        className="h-auto px-0"
                    >
                        Reset link
                    </LoadingButton>
                </AlertDialogTrigger>
                <AlertDialogContent>
                    <AlertDialogHeader>
                        <AlertDialogTitle>Reset Calendar Link</AlertDialogTitle>
                        <AlertDialogDescription>
                            The current link will stop working, and any
                            calendars subscribed to it will stop updating.
                        </AlertDialogDescription>
                    </AlertDialogHeader>
                    <AlertDialogFooter>
                        <AlertDialogCancel>Cancel</AlertDialogCancel>
                        <AlertDialogAction
                            disabled={isResetting}
                            onClick={onReset}
                        >
                            Continue
                        </AlertDialogAction>
                    </AlertDialogFooter>
                </AlertDialogContent>
            </AlertDialog>
        </section>
    );
}
