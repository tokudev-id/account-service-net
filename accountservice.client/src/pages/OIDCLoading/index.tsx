const OIDCLoading = () => {
  return (
    <div className="flex items-center justify-center min-h-screen bg-white">
      <div className="text-center space-y-6">
        <div>
          <p className="text-xl font-semibold text-gray-800">
            Signing in to Confiction <br /> Account
          </p>
        </div>
        <div className="flex justify-center">
          <div className="w-10 h-10 border-4 border-blue-500 border-t-transparent rounded-full animate-spin"></div>
        </div>
        <div>
          <p className="text-gray-600 text-sm">
            Redirecting to your dashboard… <br />
            Do not close this tab/window page
          </p>
        </div>
      </div>
    </div>
  );
};

export default OIDCLoading;
