import React, { useEffect, useState } from 'react';
import { useSearchParams } from 'react-router-dom';

const Error = () => {
  const [searchParams] = useSearchParams();
  const ERROR_ID = searchParams.get('errorId');

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState({
    error: 'Internal Server Error',
    description: 'Internal server error occurred',
  });

  useEffect(() => {
    if (ERROR_ID && ERROR_ID !== '') {
      getErrorInfo();
    } else {
      window.location.replace('/');
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const getErrorInfo = async () => {
    try {
      const response = await fetch(`/api/auth/error?errorId=${encodeURIComponent(ERROR_ID!)}`);
      if (response.ok) {
        const data = await response.json();
        setError({
          error: data.error,
          description: data.errorDescription,
        });
      }
    } catch (e) {
      // Optional: log error
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      {loading ? (
        <div className="flex items-center justify-center h-screen">
          <div className="text-lg font-medium text-gray-600">Loading...</div>
        </div>
      ) : (
        <div className="flex flex-col items-center justify-center min-h-screen px-4 text-center bg-gray-50">
          <h1 className="text-4xl font-bold text-red-600 mb-4">ERROR</h1>
          <p className="text-lg text-gray-800 mb-2">
            Sorry, there was an error: <strong>{error.error}</strong>
          </p>
          <p className="text-gray-600">{error.description}</p>
          <button
            onClick={() => window.location.assign('/')}
            className="mt-6 px-6 py-2 bg-pink-600 hover:bg-pink-700 text-white rounded-lg transition"
          >
            Back to Login
          </button>
        </div>
      )}
    </>
  );
};

export default Error;
