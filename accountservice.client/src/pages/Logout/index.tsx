import { useEffect, Fragment, useState } from 'react';
import { useSearchParams } from 'react-router-dom';

// Placeholder for your loading component
const FullPageLoading = () => <div>Logging out...</div>;

const RETURN_URI_KEY_STORAGE = 'return_uri_key'; // replace this with the actual key if different

const Logout = () => {
  const [searchParams] = useSearchParams();
  const [showLogoutNotice, setShowLogoutNotice] = useState<boolean>(false);

  const LOGOUT_ID = searchParams.get('logoutId');

  const processLogout = async () => {
    try {
      const url = new URL('/api/auth/logout', window.location.origin);
      if (LOGOUT_ID) {
        url.searchParams.append('logoutId', LOGOUT_ID);
      }

      const response = await fetch(url.toString(), {
        method: 'GET',
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json'
        }
      });

      showLogoutSuccess();

      if (response.ok) {
        const data = await response.json();
        if (data?.postLogoutRedirectUri) {
          setTimeout(() => {
            window.location.assign(data.postLogoutRedirectUri);
          }, 1000);
        }
      }
    } catch (error) {
      console.error('Logout failed:', error);
      showLogoutSuccess();
    }
  };

  const showLogoutSuccess = () => {
    setShowLogoutNotice(true);
    sessionStorage.removeItem(RETURN_URI_KEY_STORAGE);
    localStorage.removeItem('isLogin');
  };

  useEffect(() => {
    processLogout();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <Fragment>
      {!showLogoutNotice ? <FullPageLoading /> : null}
    </Fragment>
  );
};

export default Logout;
