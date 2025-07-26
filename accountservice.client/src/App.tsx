// App.tsx
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { ProtectedRoute } from './components/hoc/ProtectedRoute';
import Login from './pages/Login';
import { AuthRoute } from './components/hoc/AuthRoute/AuthRoute';
import RootLayout from './components/layout/RootLayout';
import ProfilePage from './pages/Profile';
import Logout from './pages/Logout';
import Error from './pages/Error';
import OIDCLoading from './pages/OIDCLoading';

function App() {
    return (
        <AuthProvider>
            <BrowserRouter>
                <Routes>
                    <Route path="/" element={<RootLayout />}>
                        {/* Public routes inside layout */}
                        <Route element={<AuthRoute />}>
                            <Route path="error" element={<Error />} />
                            <Route path="login" element={<Login />} />
                        </Route>
                        <Route path="api/auth/logout" element={<Logout />} />
                        <Route path="connect/authorize/callback" element={<OIDCLoading />} />

                        {/* Protected routes inside layout */}
                        <Route element={<ProtectedRoute />}>
                            <Route index element={<ProfilePage />} />
                            <Route path="profile" element={<ProfilePage />} />
                        </Route>

                        {/* 404 fallback */}
                        <Route path="*" element={<div>Not Found</div>} />
                    </Route>
                </Routes>
            </BrowserRouter>
        </AuthProvider>
    );
}

export default App;
