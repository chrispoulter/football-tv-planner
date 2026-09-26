import { Route } from 'react-router';
import { RequireAuth } from '@/components/require-auth';
import { ProfileLayout } from './profile-layout';
import { ProfileTab } from './info/profile-tab';
import { SecurityTab } from './security/security-tab';
import { DangerTab } from './danger/danger-tab';

export const profileRoutes = (
    <Route path="profile" element={<RequireAuth />}>
        <Route element={<ProfileLayout />}>
            <Route index element={<ProfileTab />} />
            <Route path="security" element={<SecurityTab />} />
            <Route path="danger" element={<DangerTab />} />
        </Route>
    </Route>
);
