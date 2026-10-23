import { Metadata } from '@/components/metadata';
import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from '@/components/ui/card';
import { DeleteAccountDialog } from './delete-account-dialog';

export function DangerTab() {
    return (
        <>
            <Metadata title="Danger Zone" />

            <Card>
                <CardHeader>
                    <CardTitle>
                        <h2>Delete Account</h2>
                    </CardTitle>
                    <CardDescription>
                        Permanently delete your account and all associated data.
                        This cannot be undone.
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    <DeleteAccountDialog className="w-full sm:w-auto" />
                </CardContent>
            </Card>
        </>
    );
}
