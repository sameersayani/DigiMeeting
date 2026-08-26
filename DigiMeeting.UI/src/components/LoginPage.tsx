import { useEffect, useState } from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import { useNavigate } from 'react-router-dom';

export function LoginPage() {
  const { loginWithRedirect, isAuthenticated, getAccessTokenSilently, isLoading, user } = useAuth0();
  const [syncing, setSyncing] = useState(false);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    // After Auth0 login, sync with backend
    if (isAuthenticated && !syncing) {
      handleLoginSync();
    }
  }, [isAuthenticated]);

  const handleLoginSync = async () => {
    setSyncing(true);
    setError('');

    try {
      const token = await getAccessTokenSilently();

      const response = await fetch('http://localhost:5209/api/auth/login', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          email: user?.email,
          name: user?.name
        })
      });

      if (!response.ok) {
        const data = await response.json();
        throw new Error(data.message || 'Login sync failed');
      }

      const data = await response.json();
      console.log('User logged in:', data);

      // Redirect to dashboard
      navigate('/dashboard');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Login failed');
      setSyncing(false);
    }
  };

  if (isLoading) {
    return <div className="auth-container"><p>Loading...</p></div>;
  }

  if (isAuthenticated) {
    return (
      <div className="auth-container">
        <div className="auth-card">
          <h2>Completing login...</h2>
          {error && <div className="error-message">{error}</div>}
        </div>
      </div>
    );
  }

  return (
    <div className="auth-container">
      <div className="auth-card">
        <h2>Login</h2>

        {error && <div className="error-message">{error}</div>}

        <button 
          onClick={() => loginWithRedirect()}
          className="auth0-button"
        >
          Login with Auth0
        </button>

        <p className="auth-link">
          Don't have an account? <a href="/register">Register here</a>
        </p>
      </div>
    </div>
  );
}
