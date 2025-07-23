import React from 'react';
import { Navigate } from 'react-router-dom';
// import { useAuth } from '../hooks/useAuth'; // We will create this hook later

interface ProtectedRouteProps {
  element: React.ReactElement;
  // You can add more props here if needed, e.g., roles for role-based authorization
  // allowedRoles?: string[];
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ element }) => {
  // This is where you'll use your authentication hook to check if the user is authenticated.
  // const { isAuthenticated, userRoles, isLoading } = useAuth();

  // Placeholder for demonstration. Replace with your actual authentication check.
  const isAuthenticated = false; // Example: Check if user has a valid token or session
  // const userRoles = ['user']; // Example: Get user roles from authentication state
  // const isLoading = false; // Example: Check if authentication state is still loading

  // You might want to show a loading spinner while authentication status is being determined
  // if (isLoading) {
  //   return <div>Loading...</div>;
  // }

  if (!isAuthenticated) {
    // If not authenticated, redirect to the login page.
    // You can customize the redirect path.
    return <Navigate to="/login" replace />;
  }

  // Optional: Add role-based authorization check here
  // if (allowedRoles && !allowedRoles.some(role => userRoles.includes(role))) {
  //   // If user doesn't have the required role, redirect to an unauthorized page or show a message
  //   return <Navigate to="/unauthorized" replace />;
  // }

  // If authenticated (and authorized), render the protected element.
  return element;
};

export default ProtectedRoute;
