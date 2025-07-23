import React, { createContext, useContext, useState, useEffect, type ReactNode } from 'react';

interface AuthState {
  isAuthenticated: boolean;
  user: { name: string } | null; // Define user structure based on your backend response
  isLoading: boolean;
}

interface AuthContextType extends AuthState {
  checkAuthStatus: () => Promise<void>; // Function to manually re-check status
  // Add other functions like login/logout if you handle them in this context
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [authState, setAuthState] = useState<AuthState>({
    isAuthenticated: false,
    user: null,
    isLoading: true, // Start in loading state
  });

  // Function to check authentication status from the backend
  const checkAuthStatus = async () => {
    setAuthState(prevState => ({ ...prevState, isLoading: true }));
    try {
      // Make an API call to your backend status endpoint
      const response = await fetch('/api/auth/check-session');
      if (response.ok) {
        const data = await response.json();
        setAuthState({
          isAuthenticated: data.isAuthenticated,
          user: data.isAuthenticated ? { name: data.userName } : null, // Adapt based on your backend response
          isLoading: false,
        });
      } else {
        // Handle error or non-OK response
        setAuthState({
          isAuthenticated: false,
          user: null,
          isLoading: false,
        });
      }
    } catch (error) {
      console.error("Error checking auth status:", error);
      setAuthState({
        isAuthenticated: false,
        user: null,
        isLoading: false,
      });
    }
  };

  // Check authentication status when the provider mounts
  useEffect(() => {
    checkAuthStatus();
  }, []);

  // You might add login/logout functions here that make API calls to your backend
  // const login = async (credentials) => { ... make login API call, then checkAuthStatus() ... };
  // const logout = async () => { ... make logout API call, then checkAuthStatus() ... };


  const contextValue: AuthContextType = {
    ...authState,
    checkAuthStatus,
    // Include login/logout functions here if defined
  };

  return (
    <AuthContext.Provider value={contextValue}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
