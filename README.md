# V-Academy Connect

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-19-61DAFB?style=flat-square&logo=react&logoColor=111111)](https://react.dev/)
[![Vite](https://img.shields.io/badge/Vite-7-646CFF?style=flat-square&logo=vite&logoColor=ffffff)](https://vite.dev/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5-3178C6?style=flat-square&logo=typescript&logoColor=ffffff)](https://www.typescriptlang.org/)
[![MySQL](https://img.shields.io/badge/MySQL-8-4479A1?style=flat-square&logo=mysql&logoColor=ffffff)](https://www.mysql.com/)
[![Ant Design](https://img.shields.io/badge/Ant%20Design-5-0170FE?style=flat-square&logo=antdesign&logoColor=ffffff)](https://ant.design/)

A multi-tenant management platform for tutoring centers and academies: classes, staff, attendance, lesson evaluations, scores, tuition billing, and automated subscription billing, behind cookie-based JWT authentication with an OTP password-reset flow.

## Table Of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [API Overview](#api-overview)
- [Troubleshooting](#troubleshooting)
- [What's Changed](#whats-changed)
- [Security Notes](#security-notes)
- [Author](#author)

## Features

- Multi-tenant academy management: a SuperAdmin oversees every tenant; a TenantAdmin/Teacher only sees their own tenant's data.
- Class management with teacher assignment, weekly schedule, tuition, and student roster (enroll an existing student or create a new one inline).
- Attendance tracking per class session.
- Lesson evaluations with quick-comment templates.
- Scores and score statistics per class, with monthly calculation.
- Tuition fee tracking with a due-date estimate based on the student's actual attendance pace.
- Payments with a separate verification (payment-check) workflow.
- Staff management with role assignment scoped by tenant type (a Center tenant can assign the Teacher role, a Personal tenant cannot).
- Subscription plans and per-tenant subscription lifecycle: trial signup, renewal, and automatic downgrade to the Free plan on expiry.
- A Quartz.NET job runs daily to expire/downgrade tenant subscriptions and logs every run as a Background Job record, visible to SuperAdmin.
- Cookie-based JWT authentication (HttpOnly `AuthToken` cookie) plus Google Sign-In.
- OTP-based password reset: request a 6-digit email code, verify it, then set a new password — codes expire after 5 minutes.
- Role-based route protection on both the API (`[Authorize]` + role checks) and the React app (`PrivateRoute` with `allowedRoles`).
- Serilog logging to console and rolling daily log files, with request logging and machine/thread enrichment.
- A public marketing landing page, separate from the authenticated dashboard shell.
- Responsive Ant Design UI with a shared SCSS design system (color tokens, page/card mixins, breakpoints).

## Tech Stack

**Frontend**

- React 19
- TypeScript
- Vite 7
- React Router 7
- Ant Design 5
- Axios
- Day.js
- SCSS

**Backend**

- .NET 8
- ASP.NET Core Web API
- ASP.NET Core Identity
- Entity Framework Core
- MySQL 8
- Quartz.NET
- MailKit (SMTP email for OTP delivery)
- Serilog

## Architecture

The backend follows Clean Architecture:

- `VAcademyConnect.Domain`: entities and enums. No dependencies on any other project.
- `VAcademyConnect.Application`: use-case handlers (authentication, classes, tenants, subscriptions, billing jobs, etc.) and repository interfaces. Depends only on `VAcademyConnect.Domain` and `VAcademyConnect.DTOs`.
- `VAcademyConnect.Infrastructure`: EF Core persistence (DbContext, migrations, repository implementations), Identity, JWT/email services, and Quartz jobs. Implements the interfaces defined in `VAcademyConnect.Application`.
- `VAcademyConnect.API`: controllers, composition root (`Program.cs`), middleware configuration, CORS, auth, Swagger, Serilog bootstrap.
- `VAcademyConnect.DTOs`: request and response contracts.
- `VAcademyConnect.Commons`: shared enums.
- `MayNghien.Infrastructures`: shared infrastructure helpers (response wrappers, claim helpers, search helpers).

Request flow: Controllers (`VAcademyConnect.API`) → Handlers (`VAcademyConnect.Application`) → Repositories (`VAcademyConnect.Infrastructure`).

The frontend is page-oriented:

- `src/pages`: route-level pages, one folder per page with its own `style.scss`.
- `src/apis`: one file per backend resource, wrapping Axios calls.
- `src/components`: reusable UI (`MainLayout`, `PrivateRoute`, form inputs).
- `src/contexts`: `AuthContext` for session state.
- `src/interfaces`: typed request/response contracts, mirroring the backend DTOs.
- `src/commons`: shared enums (`TenantType`, `OtpPurpose`, ...).
- `src/scss`: shared design tokens (`_colors.scss`, `_fonts.scss`, `_variables.scss`) and layout mixins (`_page.scss`).

## Project Structure

```text
V-Academy-Connect/
├── VAcademyConnect.Client/
│   ├── src/
│   │   ├── apis/
│   │   ├── commons/
│   │   ├── components/
│   │   ├── configs/
│   │   ├── contexts/
│   │   ├── interfaces/
│   │   ├── pages/
│   │   └── scss/
│   ├── package.json
│   └── vite.config.ts
├── VAcademyConnect.Server/
│   └── src/
│       ├── MayNghien.Infrastructures/
│       ├── VAcademyConnect.API/
│       ├── VAcademyConnect.Application/
│       ├── VAcademyConnect.Commons/
│       ├── VAcademyConnect.Domain/
│       ├── VAcademyConnect.DTOs/
│       ├── VAcademyConnect.Infrastructure/
│       └── src.sln
└── README.md
```

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js 18 or newer
- MySQL 8

### Backend

From the repository root:

```powershell
cd VAcademyConnect.Server/src
dotnet restore
```

Configure local secrets from `VAcademyConnect.API`:

```powershell
cd VAcademyConnect.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=VAcademyConnect;User=root;Password=your_password;"
dotnet user-secrets set "Jwt:Key" "replace-with-a-long-random-secret"
dotnet user-secrets set "EmailSettings:SmtpUsername" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:SmtpPassword" "your-gmail-app-password"
```

Run migrations:

```powershell
cd ..
dotnet ef database update --project VAcademyConnect.Infrastructure --startup-project VAcademyConnect.API
```

Start the API:

```powershell
dotnet run --project VAcademyConnect.API
```

Default local API URLs:

- HTTP API: `http://localhost:5177`
- HTTPS API: `https://localhost:7241`
- Swagger: `http://localhost:5177/swagger`

### Frontend

```powershell
cd VAcademyConnect.Client
npm install
npm run dev
```

The dev server opens the landing page automatically at `http://localhost:5173/`. Login/Register lead into the authenticated dashboard.

## Configuration

Keep real secrets out of Git — use `dotnet user-secrets` for local backend development.

`Jwt:Issuer` / `Jwt:Audience` and the CORS allow-list in `ServiceExtensions.AddCorsConfiguration` are set for `http://localhost:5173` (frontend) and `http://localhost:5177` / `https://localhost:7241` (backend). Update them if you run on different ports.

### Gmail SMTP

Gmail requires an App Password, not your normal Gmail password.

```powershell
dotnet user-secrets set "EmailSettings:SmtpServer" "smtp.gmail.com"
dotnet user-secrets set "EmailSettings:SmtpPort" "587"
dotnet user-secrets set "EmailSettings:SmtpUsername" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:SmtpPassword" "your-gmail-app-password"
dotnet user-secrets set "EmailSettings:FromEmail" "your-email@gmail.com"
```

### Google Sign-In

Set `Authentication:Google:ClientId` in `appsettings.json` (or user-secrets) to your OAuth 2.0 Client ID. Only the Client ID is needed server-side — the frontend exchanges a Google ID token, which the backend verifies.

### Logging

Serilog writes to the console and to `logs/log-.txt` (rolling daily). Levels and sinks are configured per environment in `appsettings.json` / `appsettings.Development.json` under the `Serilog` section — no code changes needed to adjust log levels.

## API Overview

### Accounts

| Method | Route | Description |
| --- | --- | --- |
| `POST` | `/accounts/login` | Login and set the `AuthToken` cookie |
| `POST` | `/accounts/register` | Register a new account and tenant |
| `POST` | `/accounts/google` | Login with a Google ID token |
| `POST` | `/accounts/refresh-token` | Refresh the auth cookie |
| `POST` | `/accounts/logout` | Revoke session and clear cookies |
| `POST` | `/accounts/send-otp` | Send a 6-digit OTP for password change |
| `POST` | `/accounts/verify-otp` | Verify an OTP code |
| `POST` | `/accounts/change-password` | Change password after OTP verification |
| `GET` | `/accounts/me` | Get the current session's user info |
| `GET` | `/accounts/roles` | List available roles |
| `PUT` | `/accounts/profile` | Update the current user's email/phone |

### Core Resources

Every resource below follows the same REST shape unless noted: `POST {resource}/search`, `GET {resource}/{id}`, `POST {resource}`, `PUT {resource}`, `DELETE {resource}/{id}`.

| Resource | Route prefix | Notes |
| --- | --- | --- |
| Tenants | `/tenants` | No create/delete; `PUT /tenants/{tenantId}/renew-subscription` renews a Standard plan |
| Subscriptions | `/subscriptions` | Full CRUD |
| Staffs | `/staffs` | Full CRUD |
| Classes | `/classes` | No standalone `GET {id}`; roster handled via Class Students |
| Class Students | `/class-students` | `POST` enrolls a student, `POST search` lists a class roster |
| Students | `/students` | Create + search only |
| Attendances | `/attendances` | Full CRUD |
| Lesson Evaluations | `/lesson-evaluations` | Full CRUD |
| Scores | `/scores` | Full CRUD |
| Score Statistics | `/score-statistics` | Full CRUD, plus `POST /score-statistics/calculate` |
| Tuition Fees | `/tuition-fees` | Full CRUD, plus `POST /tuition-fees/estimate` |
| Payments | `/payments` | Full CRUD |
| Payment Checks | `/payment-checks` | Create + search only |
| Background Jobs | `/background-jobs` | Read-only (`GET {id}`, `POST search`) — billing job run history |

## Troubleshooting

### Login succeeds but the app keeps redirecting to `/login`

Check that the `AuthToken` cookie is actually being set and sent. Verify:

- `withCredentials: true` is set on the Axios instance (`src/configs/axios.ts`).
- The frontend origin (`http://localhost:5173`) is present in `AddCorsConfiguration`.
- The backend was restarted after any CORS/cookie configuration change.

### Gmail OTP fails with `535 5.7.8`

Gmail rejected SMTP authentication. Use a Gmail App Password (not your account password) for `EmailSettings:SmtpPassword`, and restart the backend after updating user-secrets.

### `Unknown database` on first run

The configured database does not exist yet. Run `dotnet ef database update --project VAcademyConnect.Infrastructure --startup-project VAcademyConnect.API` — EF Core creates the database and applies all migrations.

### Password reset says "Please verify the OTP" even after entering the right code

`ChangePassword` requires a *recently verified* OTP for `OtpPurpose.ChangePassword` on that exact email. Request a fresh code if more than a few minutes passed between verifying and submitting the new password, or if the email was typed differently between steps.

## What's Changed

### Recent

- Added a public landing page and moved the app's root route off `/login`.
- Rebuilt the OTP flow end-to-end (`send-otp` → 6-digit code input → `verify-otp` → change password), replacing an unused `/accounts/verify-email` endpoint the frontend was still calling.
- Redesigned the Login/Register/Verify-Email/Change-Password pages onto the shared design system.
- Added a Profile page (view + edit email/phone) and a `/accounts/profile` endpoint.
- Replaced the static demo dashboard on `/home` with real per-role counts (Tenants/Subscriptions/Staffs for SuperAdmin; Staffs/Classes/Payments for TenantAdmin).
- Added Serilog logging (console + rolling daily file) via `Serilog.AspNetCore`.
- `npm run dev` now opens the landing page automatically.

### Earlier

- Class CRUD with teacher assignment, schedule, and roster management.
- Background Jobs page for SuperAdmin billing history.
- Quartz.NET integration for scheduled tenant subscription billing.
- Payments and Payment Checks CRUD with verification support.
- Tuition Fees management with attendance-based due-date estimation.
- Score Statistics with monthly calculation.
- Scores and Lesson Evaluations CRUD.
- Attendance CRUD.
- Session-based auth (`/accounts/me`), TenantType support, and Google login.

## Security Notes

- Do not commit `appsettings.Development.json` with real secrets, SMTP credentials, JWT keys, or database passwords — use `dotnet user-secrets` for local development.
- Rotate any credential that was pasted into chat, terminal logs, or committed history.
- Use a Gmail App Password for SMTP, never the account password.
- Keep SuperAdmin-only endpoints behind role checks; do not widen `allowedRoles` without a reason.

## Author

Built by Rainy.
