import { Metadata } from '@/components/metadata';
import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from '@/components/ui/card';
import { DeleteAccountButton } from './delete-account-button';

export function DangerTab() {
    return (
        <>
            <Metadata title="Danger Zone" />

            <Card>
                <CardHeader>
                    <CardTitle>Delete Account</CardTitle>
                    <CardDescription>
                        Permanently delete your account, your schedule and your
                        calendar feed. This cannot be undone.
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    <DeleteAccountButton className="w-full sm:w-auto" />
                </CardContent>
            </Card>
        </>
    );
}
