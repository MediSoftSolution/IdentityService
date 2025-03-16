# IdentityService - Hospital System Microservices

**IdentityService** is responsible for handling user authentication and authorization in the **Hospital System Microservices** platform. It provides functionality for user login, registration, token-based authentication using JWT (JSON Web Tokens), and managing blocked tokens via Redis for added security.

## 📌 Overview

The **IdentityService** manages:

- **User Registration**: Allows users to sign up for the system.
- **Authentication**: Validates user credentials (username and password).
- **Authorization**: Issues JSON Web Tokens (JWT) to authorized users for secure access.
- **Role-based Access Control (RBAC)**: Supports different roles such as Admin, Doctor, Patient, etc.
- **Token Blacklist**: Admins can block tokens, and these blocked tokens are stored in Redis. Any attempt to log in with a blocked token is denied.
- **Integration**: Works seamlessly with other microservices like **APIGateway**, **HospitalService**, and more.

---

## 🔧 Technologies Used
- **Backend**: C# - .NET 8
- **Authentication**: JWT (JSON Web Tokens)
- **Database**: SQL Server
- **Service Discovery**: Consul
- **API Gateway**: Ocelot (for routing requests to services)
- **Cache**: Redis (for managing blocked tokens)

---

## 🚀 Features

- **User Registration**: Users can create an account by providing necessary details such as username, password, and role.
- **User Authentication**: Validates user credentials and issues a JWT token upon successful authentication.
- **JWT Token Generation**: Upon login, a JWT token is generated and returned to the user for subsequent requests.
- **Role-based Access**: Supports different roles like Admin, Doctor, Patient, etc. to provide proper access control across services.
- **Token Blacklisting**: **Admin** can blacklist specific tokens, preventing users with blocked tokens from logging into the system. Blacklisted tokens are stored in Redis and checked during login.

---

## 🔧 Configuration

### 1. **JWT Token Configuration**

By default, **IdentityService** uses **JWT** for token generation. You can configure the token lifespan, signing key, and other JWT-related settings in the `appsettings.json` file.

```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "IdentityService",
    "Audience": "HospitalSystem",
    "ExpiresInMinutes": 60
  }
}
