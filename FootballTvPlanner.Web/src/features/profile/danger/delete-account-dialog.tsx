import { useNavigate } from 'react-router';
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
import { useAuth } from '@/components/auth-provider';
import { Button } from '@/components/ui/button';
import { useDeleteAccount } from '../profile-queries';

interface DeleteAccountDialogProps {
    disabled?: boolean;
    className?: string;
}

export function DeleteAccountDialog({
    disabled,
    className,
}: DeleteAccountDialogProps) {
    const navigate = useNavigate();

    const { refreshAuth } = useAuth();

    const { mutate: deleteAccount, isPending: isDeleting } = useDeleteAccount();

    function onDelete() {
        deleteAccount(undefined, {
            onSuccess: () => {
                toast.success('Account deleted');
                refreshAuth();
                navigate('/');
            },
            onError: (error) => toast.error(error.message),
        });
    }

    return (
        <AlertDialog>
            <AlertDialogTrigger asChild>
                <Button
                    variant="destructive"
                    disabled={disabled || isDeleting}
                    className={className}
                >
                    {isDeleting ? 'Deleting...' : 'Delete Account'}
                </Button>
            </AlertDialogTrigger>
            <AlertDialogContent>
                <AlertDialogHeader>
                    <AlertDialogTitle>Delete Account</AlertDialogTitle>
                    <AlertDialogDescription>
                        Are you sure you want to delete your account? All of
                        your data will be permanently removed. This action
                        cannot be undone.
                    </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                    <AlertDialogCancel>Cancel</AlertDialogCancel>
                    <AlertDialogAction
                        disabled={disabled || isDeleting}
                        onClick={onDelete}
                    >
                        Continue
                    </AlertDialogAction>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    );
}
