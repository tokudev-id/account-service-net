
import { useState, useEffect, useCallback } from 'react';
import { useToast } from '@/hooks/use-toast';
import { useNavigate } from 'react-router';

export interface ProfileUserInfo {
  nickname?: string;
  gender?: string;
  country?: string;
  birthdate?: string;
  timezone?: string;
  picture?: string;
  has_password?: boolean;
  latest_password_changed_at?: string | null;
  userId: string;
  sub: string;
  email?: string | null;
  roles?: string[];
  isEmailVerified?: boolean;
  [key: string]: any;
}

export function useProfile() {
  const navigate = useNavigate();
  const { toast } = useToast();
  const [userInfo, setUserInfo] = useState<ProfileUserInfo | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isEditing, setIsEditing] = useState(false);
  const searchParams = new URLSearchParams(location.search);
  const returnUrl = searchParams.get('ReturnUrl') ?? '';

  useEffect(() => {
    setTimeout(() => {
      if (returnUrl) {
        const returnUrlUrl = new URL(returnUrl, window.location.origin);
        if (returnUrlUrl.origin === window.location.origin) {
          navigate(returnUrlUrl.pathname + returnUrlUrl.search + returnUrlUrl.hash);
        }
      }
    }, 500);
  }, []);

  const fetchUserInfo = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      const response = await fetch('/api/profile', {
        headers: {
          'Content-Type': 'application/json',
        },
      });

      if (response.status === 401) {
        toast({
          title: 'Session Expired',
          description: 'Please log in again.',
          variant: 'destructive',
        });
        navigate('/login');
        return;
      }

      if (!response.ok) {
        let errorData = { error_description: 'Failed to fetch user information' };
        try {
          errorData = await response.json();
        } catch (parseError) {
          console.error('Failed to parse error response:', parseError);
        }
        throw new Error(errorData.error_description);
      }

      const data: ProfileUserInfo = await response.json();

      setUserInfo({
        ...data.data,
        nickname: data.data.nickname || data.data.name || 'User',
        gender: data.data.gender || '',
        name: data.data.name || '',
        country: data.data.country || '',
        birthdate: data.data.birthDate || '',
        timezone: data.data.timezone || '',
        isEmailVerified: data.data.isEmailVerified,
        picture: data.data.picture,
        has_password: data.data.hasPassword,
        latest_password_changed_at: data.data.passwordLastUpdate,
      });
    } catch (err) {
      console.error('Failed to fetch user info:', err);
      const errorMessage =
        err instanceof Error ? err.message : 'An unknown error occurred';
      setError(errorMessage);

      if (
        errorMessage.toLowerCase().includes('token') ||
        errorMessage.toLowerCase().includes('login') ||
        errorMessage.toLowerCase().includes('session expired')
      ) {
        navigate('/login');
      } else {
        toast({
          title: 'Error',
          description: `Could not load user data: ${errorMessage}`,
          variant: 'destructive',
        });
      }
    } finally {
      setIsLoading(false);
    }
  }, [navigate, toast]);

  useEffect(() => {
    fetchUserInfo();
  }, [fetchUserInfo]);

  const handleLogout = useCallback(async () => {
    setIsLoading(true);
    try {
      const res = await fetch('/api/auth/logout', {
        method: 'POST',
        credentials: 'include',
      });

      if (!res.ok) {
        throw new Error('Failed to logout properly');
      }

      toast({
        title: 'Logged Out',
        description: 'You have been successfully logged out.',
      });

      window.location.reload()
    } catch (error) {
      console.error('Logout error:', error);
      toast({
        title: 'Logout Failed',
        description: 'Could not log out. Please try again.',
        variant: 'destructive',
      });
    } finally {
      setIsLoading(false);
    }
  }, [navigate, toast]);

  const handleEditSuccess = () => {
    setIsEditing(false);
    fetchUserInfo();
  };

  return {
    userInfo,
    isLoading,
    error,
    isEditing,
    setIsEditing,
    handleLogout,
    handleEditSuccess,
    fetchUserInfo,
  };
}
