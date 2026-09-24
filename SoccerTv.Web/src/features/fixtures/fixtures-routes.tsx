import { Route } from 'react-router';
import { FixturesPage } from './get-fixtures/fixtures-page';

export const fixturesRoutes = (
    <Route path="fixtures">
        <Route index element={<FixturesPage />} />
    </Route>
);
