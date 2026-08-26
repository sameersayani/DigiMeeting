import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { Auth0Provider } from '@auth0/auth0-react'
import './index.css'
import App from './App.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <Auth0Provider
      domain="dev-0v1br8be3kxc61ey.us.auth0.com"
      clientId="5ZQj8QenNxK5RMnrQzABAqYgU8DtyNqt"
      authorizationParams={{
        redirect_uri: `${window.location.origin}/login`,
        audience: 'https://localhost:5209/api',
        scope: 'openid profile email',
      }}
    >
      <App />
    </Auth0Provider>
  </StrictMode>,
)
