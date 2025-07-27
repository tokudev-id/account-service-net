// src/components/layout/RootLayout.tsx
import { Suspense } from 'react';
import { Outlet, Link } from 'react-router-dom';
import { Toaster } from '@/components/ui/toaster';
// import ThemeToggle from '@/components/theme-toggle';
// import PageProgressBar from '@/components/page-progress-bar';

export default function RootLayout() {
  const appName = import.meta.env.VITE_APP_NAME || 'Auth Service';
  const logoUrl = import.meta.env.VITE_APP_LOGO_URL || 'https://res.cloudinary.com/de2skg7eg/image/upload/v1747821772/logo_imdtoi.png';

  return (
    <Suspense>
      <header className="sticky top-0 z-50 w-full border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
        <div className="container relative flex h-14 items-center">
          <div className="flex items-center pl-2">
            <Link to="/" className="flex items-center gap-2">
              <img src={logoUrl} alt="App Logo" className="h-12 w-12 text-primary" />
              <span className="font-semibold">{appName}</span>
            </Link>
          </div>
          <div className="absolute right-0 mr-6 flex items-center">
            {/* <ThemeToggle /> */}
          </div>
        </div>
      </header>

      {/* <PageProgressBar /> */}

      {/* <main className="min-h-[calc(100vh-theme(spacing.14))] flex flex-col">
        <div className="flex flex-1 items-center justify-center bg-secondary p-4">
            <div className="w-full max-w-md">
                <Outlet />
            </div>
        </div>
      </main> */}
      <main className="min-h-[calc(100vh-theme(spacing.14))] flex flex-col">
        <div className="flex-1 bg-muted/40">
          <Outlet />
        </div>
      </main>


      <Toaster />
    </Suspense>
  );
}
