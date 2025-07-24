// components/AuthRoute.tsx

import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../../../contexts/AuthContext';

export const AuthRoute = () => {
    const { shouldRedirectToHome, isLoading } = useAuth();

    if (isLoading) {
        return <div>Loading...</div>; // or your loading spinner
    }

    if (shouldRedirectToHome()) {
        return <><Navigate to="/" replace /> </>;
    }

    return <Outlet />;
};