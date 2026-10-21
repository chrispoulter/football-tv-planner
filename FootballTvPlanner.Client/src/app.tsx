import { Routes, Route } from 'react-router';
import { RootLayout } from '@/components/layout/root-layout';
import { NotFoundPage } from '@/pages/not-found-page';
import { PrivacyPage } from '@/pages/privacy-page';

import { accountRoutes } from '@/features/account/account-routes';
import { fixturesRoutes } from '@/features/fixtures/fixtures-routes';
import { profileRoutes } from '@/features/profile/profile-routes';

export default function App() {
    return (
        <Routes>
            <Route element={<RootLayout />}>
                {fixturesRoutes}
                {accountRoutes}
                {profileRoutes}
                <Route path="/privacy" element={<PrivacyPage />} />
                <Route path="*" element={<NotFoundPage />} />
            </Route>
        </Routes>
    );
}
