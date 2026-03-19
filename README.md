# Hospital Management System

A complete, full-stack application for managing hospitals and clinics. This system allows administrators to manage doctors, patients, departments, and appointments, while doctors and patients can also interact with the system based on their roles.

## Project Overview

This is a production-ready Hospital Management System built with:
- **Backend**: ASP.NET Core 8 Web API with Entity Framework Core and SQLite
- **Frontend**: Pure HTML5, CSS3, and Vanilla JavaScript (no frameworks)
- **Authentication**: JWT Bearer tokens
- **Authorization**: Role-based access control (Admin, Doctor, Patient)

## How to Run the Project

### Prerequisites

- .NET 8 SDK installed ([Download here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0))
- A modern web browser with JavaScript enabled
- A terminal or command prompt

### Backend Setup

1. **Navigate to the backend directory**:
   ```bash
   cd backend
   ```

2. **Restore NuGet packages**:
   ```bash
   dotnet restore
   ```

3. **Build the project**:
   ```bash
   dotnet build
   ```

4. **Run the project**:
   ```bash
   dotnet run
   ```

   The backend will start on `https://localhost:5001`

5. **Verify it's running**:
   - Open a browser and navigate to `https://localhost:5001/swagger/index.html`
   - You should see the Swagger API documentation

### Frontend Setup

1. **Navigate to the frontend directory**:
   ```bash
   cd frontend
   ```

2. **Open in a web server** (required for CORS and JWT handling):
   
   **Option A: Using Python** (if installed):
   ```bash
   # Python 3
   python -m http.server 8000
   
   # Or Python 2
   python -m SimpleHTTPServer 8000
   ```

   **Option B: Using Node.js** (if installed):
   ```bash
   npx http-server
   ```

   **Option C: Using VS Code Live Server extension**:
   - Install the "Live Server" extension in VS Code
   - Right-click on `index.html` and select "Open with Live Server"

3. **Access the application**:
   - If using Python: Open browser to `http://localhost:8000`
   - If using Node.js: Open browser to `http://localhost:8080` (or as shown in terminal)
   - If using Live Server: Open browser to the URL shown (typically `http://localhost:5500`)

## Technologies Used

### Backend

| Technology | Description |
|---|---|
| **ASP.NET Core 8** | Modern, high-performance web framework by Microsoft for building APIs |
| **Entity Framework Core** | Object-relational mapper (ORM) for database access with LINQ queries |
| **SQLite** | Lightweight, file-based relational database perfect for learning and small deployments |
| **JWT (JSON Web Tokens)** | Industry-standard stateless authentication mechanism for APIs |
| **Swagger/OpenAPI** | API documentation and interactive testing tool built into the backend |
| **C# 12** | Modern, type-safe programming language with LINQ support |

### Frontend

| Technology | Description |
|---|---|
| **HTML5** | Semantic markup for modern web applications |
| **CSS3** | Responsive design with flexbox and grid layouts; custom properties for theming |
| **Vanilla JavaScript (ES6+)** | No frameworks; pure JavaScript using async/await, fetch API, and modern DOM APIs |
| **Fetch API** | Modern browser API for HTTP requests with promise-based interface |
| **Local Storage** | Browser API for persistent client-side JWT token storage |

## HTTP-Only Cookies vs localStorage for JWT Tokens

### Why HTTP-Only Cookies Are Industry Standard

#### What are HTTP-Only Cookies?
HTTP-Only cookies are cookies with the `HttpOnly` flag set, preventing JavaScript from accessing them via `document.cookie`. These cookies are automatically sent by the browser with every HTTP request to the same domain.

#### Security Advantages of HTTP-Only Cookies:

1. **XSS (Cross-Site Scripting) Protection**
   - Malicious JavaScript cannot steal the token via `document.cookie` or `localStorage`
   - Even if an attacker injects JavaScript into your page, they cannot access the token
   - Example: A compromised third-party script cannot read the token and send it to a malicious server

2. **CSRF (Cross-Site Request Forgery) Mitigation**
   - When combined with SameSite attribute, prevents unauthorized requests from other sites
   - Browser automatically includes the cookie only for same-site requests
   - Protects against attacks where a user visits a malicious site that makes requests on their behalf

3. **No Manual Token Handling**
   - Tokens are automatically sent with every request (no need to manually add Authorization headers)
   - Reduces risk of accidentally exposing tokens in logs or error messages
   - Prevents tokens from being visible in browser DevTools

#### Why We Use localStorage for This Project:

While HTTP-Only cookies are more secure, this educational project uses `localStorage` because:

1. **Educational Purposes**: Demonstrates JWT authentication flow clearly
2. **Simplicity**: Easier to understand token-based authentication without server configuration
3. **Frontend-Only**: Pure HTML/CSS/JS without server-side cookie setup
4. **HTTPS Secure Context**: The project uses HTTPS (localhost:5001) which encrypts tokens in transit
5. **Limited Attack Surface**: As a local project, the risk of XSS attacks is minimal

**For Production Applications**: Always use HTTP-Only cookies with SameSite and Secure flags set at the server level.

### Comparison Table

| Feature | HTTP-Only Cookies | localStorage |
|---|---|---|
| XSS Protection | ✅ Excellent | ⚠️ Vulnerable |
| CSRF Protection | ✅ Yes (with SameSite) | ❌ No built-in |
| Automatic Sending | ✅ Yes | ❌ Manual headers |
| Server Control | ✅ Yes | ❌ Client control |
| JavaScript Access | ❌ No | ✅ Yes |
| Persistence | ✅ Configurable | ✅ Until cleared |
| **Recommendation** | **Use for production** | **OK for learning** |

## API Endpoints

All protected endpoints require a JWT token in the `Authorization: Bearer <token>` header.

### Authentication Endpoints

| Method | Endpoint | Role | Description |
|---|---|---|---|
| POST | `/api/auth/register` | Public | Register a new user (Doctor or Patient) |
| POST | `/api/auth/login` | Public | Login and receive JWT token |

### Doctor Endpoints

| Method | Endpoint | Allowed Roles | Description |
|---|---|---|---|
| GET | `/api/doctors` | Admin, Patient | Get all doctors |
| GET | `/api/doctors/{id}` | Admin, Patient | Get doctor by ID |
| POST | `/api/doctors` | Admin | Create new doctor |
| PUT | `/api/doctors/{id}` | Admin, Doctor | Update doctor details |
| DELETE | `/api/doctors/{id}` | Admin | Delete doctor |

### Patient Endpoints

| Method | Endpoint | Allowed Roles | Description |
|---|---|---|---|
| GET | `/api/patients` | Admin, Doctor | Get all patients |
| GET | `/api/patients/{id}` | Admin, Doctor | Get patient by ID |
| POST | `/api/patients` | Admin | Create new patient |
| PUT | `/api/patients/{id}` | Admin, Patient | Update patient details |
| DELETE | `/api/patients/{id}` | Admin | Delete patient |

### Appointment Endpoints

| Method | Endpoint | Allowed Roles | Description |
|---|---|---|---|
| GET | `/api/appointments` | Admin, Doctor, Patient | Get all appointments |
| GET | `/api/appointments/doctor/{doctorId}` | Admin, Doctor, Patient | Get appointments by doctor |
| GET | `/api/appointments/patient/{patientId}` | Admin, Doctor, Patient | Get appointments by patient |
| GET | `/api/appointments/{id}` | Admin, Doctor, Patient | Get appointment by ID |
| POST | `/api/appointments` | Admin, Patient | Book new appointment |
| PUT | `/api/appointments/{id}` | Admin, Doctor | Update appointment status |
| DELETE | `/api/appointments/{id}` | Admin | Delete appointment |

### Department Endpoints

| Method | Endpoint | Allowed Roles | Description |
|---|---|---|---|
| GET | `/api/departments` | All authenticated | Get all departments |
| GET | `/api/departments/{id}` | All authenticated | Get department by ID |
| POST | `/api/departments` | Admin | Create new department |
| PUT | `/api/departments/{id}` | Admin | Update department |
| DELETE | `/api/departments/{id}` | Admin | Delete department |

## Demo Credentials

Use these credentials to test the application:

### Admin Account
- **Email**: admin@clinic.com
- **Password**: Admin123!

### Doctor Account
- **Email**: john.smith@clinic.com
- **Password**: Doctor123!

### Patient Account
- **Email**: alice.brown@email.com
- **Password**: Patient123!

## Frontend Pages

1. **login.html** - User login page with role-based redirection
2. **register.html** - User registration for new doctors and patients
3. **dashboard.html** - Role-aware dashboard:
   - Admin sees statistics and quick action buttons
   - Doctor sees their upcoming appointments
   - Patient sees their booked appointments
4. **doctors.html** - List of all doctors with admin edit/delete functions
5. **patients.html** - List of all patients with admin edit/delete functions
6. **appointments.html** - Appointment management:
   - Patients can book new appointments
   - Doctors can update appointment status
   - Admin has full control
7. **departments.html** - Department management (admin only)

## Database Schema

### User Entity
- Stores login credentials and role information
- One-to-one relationship with DoctorProfile
- Referenced by Doctor and Patient entities

### DoctorProfile Entity
- Stores doctor specialization, bio, and years of experience
- One-to-one with User entity

### Doctor Entity
- Represents a doctor in the system
- Many-to-One with Department
- Has many Appointments

### Patient Entity
- Represents a patient in the system
- Has many Appointments

### Department Entity
- Represents hospital departments
- One-to-Many with Doctor

### Appointment Entity
- Represents appointments between doctors and patients
- Many-to-One with Doctor
- Many-to-One with Patient
- Tracks appointment status (Pending, Confirmed, Completed, Cancelled)

## Features Implemented

✅ Complete user authentication with JWT tokens
✅ Role-based access control (Admin, Doctor, Patient)
✅ All CRUD operations for doctors, patients, appointments, and departments
✅ Doctor profile with specialization and years of experience
✅ Appointment booking and status management
✅ Department management
✅ Responsive design for desktop and mobile
✅ Toast notifications for user feedback
✅ Loading spinners for async operations
✅ Data validation on both frontend and backend
✅ Error handling with meaningful error messages
✅ Swagger API documentation
✅ SQLite database with automatic seeding
✅ Async/await patterns throughout backend
✅ Entity Framework Core with AsNoTracking for optimization
✅ DTO pattern for API responses
✅ Clean separation of concerns with services and controllers

## Project Structure

```
/hospital-system-dotnet
  ├── /backend
  │   ├── /Controllers       # API endpoints
  │   ├── /Services          # Business logic
  │   ├── /Models            # Database entities
  │   ├── /DTOs              # Data transfer objects
  │   ├── /Data              # DbContext and migrations
  │   ├── Program.cs         # Application setup
  │   ├── appsettings.json   # Configuration
  │   └── HospitalManagementSystem.csproj
  ├── /frontend
  │   ├── /css
  │   │   └── style.css      # All styling
  │   ├── /js
  │   │   ├── api.js         # API utilities
  │   │   ├── login.js
  │   │   ├── register.js
  │   │   ├── dashboard.js
  │   │   ├── doctors.js
  │   │   ├── patients.js
  │   │   ├── appointments.js
  │   │   └── departments.js
  │   ├── index.html         # Redirect to login
  │   ├── login.html
  │   ├── register.html
  │   ├── dashboard.html
  │   ├── doctors.html
  │   ├── patients.html
  │   ├── appointments.html
  │   └── departments.html
  └── README.md
```

## Troubleshooting

### CORS Errors
- Make sure the backend is running on `https://localhost:5001`
- The CORS policy allows requests from any origin for development

### SSL/TLS Certificate Issues
- ASP.NET Core uses a self-signed certificate for HTTPS
- You may need to accept the certificate warning in your browser
- Proceed anyway - it's safe for local development

### Database Issues
- Delete `hospital.db` in the backend directory to reset the database
- Re-run the backend application to recreate and seed the database

### Token Expiration
- Default token expiration is 24 hours (1440 minutes)
- Change `ExpiryMinutes` in `appsettings.json` to adjust

### Port Already in Use
- If port 5001 is already in use, you can change it in the project settings
- Update the frontend `API_BASE_URL` to match your new port

## Testing Recommendations

1. Test all CRUD operations for each entity
2. Try accessing endpoints with wrong roles (should get 403 Forbidden)
3. Try accessing protected endpoints without token (should get 401 Unauthorized)
4. Test appointment booking and status updates
5. Test role-based dashboard views
6. Test token expiration behavior
7. Test form validation on both frontend and backend

## Future Enhancements

- Email notifications for appointment reminders
- Multi-factor authentication (MFA)
- Appointment time slot management
- Doctor availability calendar
- Patient medical history records
- Prescription management
- Payment processing integration
- SMS/Email notifications

## License

This project is created for educational purposes.

---

**Last Updated**: 2024
**Status**: Production Ready ✅
