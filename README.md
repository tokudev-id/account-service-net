# AccountService Application

This application is a template project demonstrating a full-stack application with a .NET Core backend and a React frontend.

## Planning: Integrating Duende Identity Server

The primary goal for this application is to evolve it into a robust identity server using Duende Identity Server for the backend and integrating it with the React frontend. This will enable secure authentication and authorization for the application.

### Backend (AccountService.Server) Goals:

*   Implement Duende Identity Server for authentication and authorization.
*   Define API resources and clients within Identity Server.
*   Integrate a user store (e.g., Entity Framework Core) with Identity Server.
*   Create secure API endpoints protected by Identity Server.

### Frontend (accountservice.client) Goals:

*   Integrate an OIDC client library for React.
*   Implement the authentication flow (login, logout, token management).
*   Protect routes and make authenticated API calls.
*   Manage user authentication state.

### Connection Goals:

*   Configure the React client and backend API as resources in Identity Server.
*   Ensure seamless communication and token exchange between the frontend and backend.

This plan outlines the key steps to transform this template into a secure, identity-aware application.