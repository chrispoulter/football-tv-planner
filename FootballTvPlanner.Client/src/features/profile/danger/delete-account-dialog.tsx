import { useState } from 'react';
import { useNavigate } from 'react-router';
import { toast } from 'sonner';
import { useAuth } from '@/components/auth-provider';
import { Button } from '@/components/ui/button';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
} from '@/components/ui/dialog';
import { useDeleteAccount } from '../profile-queries';

export function DeleteAccountDialog() {
    const [isOpen, setIsOpen] = useState(false);

    const navigate = useNavigate();

    const { refreshAuth } = useAuth();

    const { mutate: deleteAccount, isPending } = useDeleteAccount();

    function onDelete() {
        deleteAccount(undefined, {
            onSuccess: () => {
                setIsOpen(false);
                toast.success('Account deleted');
                refreshAuth();
                navigate('/login');
            },
            onError: (error) => toast.error(error.message),
        });
    }

    return (
        <Dialog open={isOpen} onOpenChange={setIsOpen}>
            <DialogTrigger asChild>
                <Button variant="destructive" className="w-full sm:w-auto">
                    Delete Account
                </Button>
            </DialogTrigger>
            <DialogContent>
                <DialogHeader>
                    <DialogTitle>Delete account?</DialogTitle>
                    <DialogDescription>
                        This action cannot be undone. Your account and all
                        associated data will be permanently deleted.
                    </DialogDescription>
                </DialogHeader>
                <DialogFooter>
                    <Button variant="outline" onClick={() => setIsOpen(false)}>
                        Cancel
                    </Button>
                    <Button
                        variant="destructive"
                        onClick={onDelete}
                        disabled={isPending}
                    >
                        {isPending ? 'Deleting...' : 'Delete'}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
}
