import './App.css';
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import RootLayout from './components/layout/RootLayout';
import ProtectedRoute from './components/hoc/ProtectedRoute';
import Login from './pages/Login';

function App() {
  const isAuthenticated = false; // Replace with actual authentication state
    const isLoading = false; // Replace with actual loading state

    if (isLoading) {
        // Optionally, show a loading indicator while checking authentication status
        return <div>Loading authentication...</div>;
    }

    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<RootLayout />}>
                    {/* Public Routes */}
                    <Route path="login" element={<Login />} />
                    {/* <Route path="about" element={<About />} /> Add back other public routes if needed */}


                    {/* Root path behavior based on authentication */}
                    <Route
                        index
                        element={
                            isAuthenticated ? (
                                <ProtectedRoute element={<>PRofile</>} /> // If authorized, show Profile
                            ) : (
                                <Navigate to="/login" replace /> // If not authorized, redirect to Login
                            )
                        }
                    />

                    {/* Other Protected Routes */}
                    {/* Example: A dashboard page that is always protected */}
                    <Route
                        path="dashboard"
                        element={<ProtectedRoute element={<>Dashboard</>} />} // Use ProtectedRoute for this path
                    />


                    {/* Catch-all route for 404 */}
                    <Route path="*" element={<>Not Found</>} />
                </Route>
            </Routes>
        </BrowserRouter>
    )
}

export default App;