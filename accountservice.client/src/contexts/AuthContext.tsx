import React, {
    createContext,
    useContext,
    useState,
    useEffect,
    useCallback,
    useRef
} from 'react';

interface AuthState {
    isAuthenticated: boolean;
    user: { name: string } | null;
    isLoading: boolean;
}

interface AuthContextType extends AuthState {
    checkAuthStatus: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
    children: React.ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
    const [authState, setAuthState] = useState<AuthState>({
        isAuthenticated: false,
        user: null,
        isLoading: true,
    });

    // Track mounted state to prevent state updates after unmount
    const isMountedRef = useRef(true);

    // Memoized API call with AbortController support
    const checkAuthStatus = useCallback(async () => {
        try {
            // Only show loading if not already loading
            setAuthState(prev => prev.isLoading ? prev : { ...prev, isLoading: true });

            const abortController = new AbortController();
            const timeoutId = setTimeout(() => abortController.abort(), 10000); // 10s timeout

            const response = await fetch('/api/auth/check-session', {
                signal: abortController.signal
            });

            clearTimeout(timeoutId);

            if (response.ok) {
                const data = await response.json();
                if (isMountedRef.current) {
                    setAuthState({
                        isAuthenticated: data.isAuthenticated,
                        user: data.isAuthenticated ? { name: data.userName } : null,
                        isLoading: false,
                    });
                }
            } else {
                throw new Error('Auth check failed');
            }
        } catch (error) {
            if (isMountedRef.current) {
                console.log(error);
                setAuthState({
                    isAuthenticated: false,
                    user: null,
                    isLoading: false,
                });
            }
        }
    }, []);

    // Initial auth check with cleanup
    useEffect(() => {
        isMountedRef.current = true;

        // Only fetch if we haven't loaded yet
        if (authState.isLoading) {
            checkAuthStatus();
        }

        return () => {
            isMountedRef.current = false;
        };
    // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [authState.isLoading]);

    const contextValue: AuthContextType = {
        ...authState,
        checkAuthStatus,
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
    const shouldRedirectToHome = () => {
        return !context.isLoading && context.isAuthenticated;
    };

    const shouldRedirectToLogin = () => {
        return !context.isLoading && !context.isAuthenticated;
    };

    return {
        ...context,
        shouldRedirectToHome,
        shouldRedirectToLogin,
    };
};