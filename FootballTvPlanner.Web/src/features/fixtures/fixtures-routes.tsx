import { Route } from 'react-router';
import { FixturesPage } from './fixtures-page';

export const fixturesRoutes = (
    <Route>
        <Route path="/" element={<FixturesPage />} />
    </Route>
);
