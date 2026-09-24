import { Route } from 'react-router';
import { RequireAuth } from '@/components/require-auth';
import { SchedulePage } from './get-schedule/schedule-page';

export const scheduleRoutes = (
    <Route path="schedule" element={<RequireAuth />}>
        <Route index element={<SchedulePage />} />
    </Route>
);
