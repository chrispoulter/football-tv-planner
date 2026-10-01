import { Route } from 'react-router';
import { FixturesPage } from './get-fixtures/fixtures-page';

export const fixturesRoutes = (
    <Route path="fixtures">
        <Route path="/" element={<FixturesPage />} />
    </Route>
);
