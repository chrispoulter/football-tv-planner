import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router';
import { ErrorBoundary } from 'react-error-boundary';
import { QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { Toaster } from '@/components/ui/sonner';
import { ThemeProvider } from '@/components/theme-provider';
import { queryClient } from '@/lib/query-client';
import App from './app';

import '@/index.css';

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <QueryClientProvider client={queryClient}>
            <BrowserRouter>
                <ThemeProvider>
                    <ErrorBoundary
                        fallback={
                            <div className="flex min-h-screen items-center justify-center p-8 text-center">
                                Something went wrong. Please refresh the page.
                            </div>
                        }
                    >
                        <App />
                    </ErrorBoundary>
                    <Toaster richColors />
                </ThemeProvider>
            </BrowserRouter>
            <ReactQueryDevtools initialIsOpen={false} />
        </QueryClientProvider>
    </StrictMode>
);
