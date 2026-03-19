using HospitalManagementSystem.Data;
using HospitalManagementSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Entity Framework Core with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=hospital.db"));

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"] ?? "DefaultSecretKey12345");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Add Authorization
builder.Services.AddAuthorization();

// Register services for dependency injection
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();

// Add Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hospital Management System API",
        Version = "v1",
        Description = "API for managing doctors, patients, appointments, and departments"
    });

    // Add JWT Bearer authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Add CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Apply migrations and seed database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
    SeedDatabase(dbContext);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Hospital Management System API");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Seed database with initial data
static void SeedDatabase(AppDbContext context)
{
    // Check if data already exists
    if (context.Users.Any())
        return;

    // Hash password function
    string HashPassword(string password)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    // Create Admin user
    var adminUser = new HospitalManagementSystem.Models.User
    {
        FullName = "Administrator",
        Email = "admin@clinic.com",
        PasswordHash = HashPassword("Admin123!"),
        Role = "Admin"
    };
    context.Users.Add(adminUser);

    // Create Departments
    var cardiology = new HospitalManagementSystem.Models.Department
    {
        Name = "Cardiology",
        Description = "Heart and cardiovascular diseases"
    };
    var neurology = new HospitalManagementSystem.Models.Department
    {
        Name = "Neurology",
        Description = "Brain and nervous system disorders"
    };
    context.Departments.AddRange(cardiology, neurology);
    context.SaveChanges();

    // Create Doctor Users
    var doctorUser1 = new HospitalManagementSystem.Models.User
    {
        FullName = "Dr. John Smith",
        Email = "john.smith@clinic.com",
        PasswordHash = HashPassword("Doctor123!"),
        Role = "Doctor"
    };
    var doctorUser2 = new HospitalManagementSystem.Models.User
    {
        FullName = "Dr. Sarah Johnson",
        Email = "sarah.johnson@clinic.com",
        PasswordHash = HashPassword("Doctor123!"),
        Role = "Doctor"
    };
    context.Users.AddRange(doctorUser1, doctorUser2);
    context.SaveChanges();

    // Create Doctor Profiles
    var doctorProfile1 = new HospitalManagementSystem.Models.DoctorProfile
    {
        UserId = doctorUser1.Id,
        Specialization = "Cardiology",
        Bio = "Experienced cardiologist with 10 years of practice",
        YearsOfExperience = 10
    };
    var doctorProfile2 = new HospitalManagementSystem.Models.DoctorProfile
    {
        UserId = doctorUser2.Id,
        Specialization = "Neurology",
        Bio = "Specialist in neurology with 8 years of experience",
        YearsOfExperience = 8
    };
    context.DoctorProfiles.AddRange(doctorProfile1, doctorProfile2);
    context.SaveChanges();

    // Create Doctors
    var doctor1 = new HospitalManagementSystem.Models.Doctor
    {
        UserId = doctorUser1.Id,
        DepartmentId = cardiology.Id,
        LicenseNumber = "LIC001"
    };
    var doctor2 = new HospitalManagementSystem.Models.Doctor
    {
        UserId = doctorUser2.Id,
        DepartmentId = neurology.Id,
        LicenseNumber = "LIC002"
    };
    context.Doctors.AddRange(doctor1, doctor2);
    context.SaveChanges();

    // Create Patient Users
    var patientUser1 = new HospitalManagementSystem.Models.User
    {
        FullName = "Alice Brown",
        Email = "alice.brown@email.com",
        PasswordHash = HashPassword("Patient123!"),
        Role = "Patient"
    };
    var patientUser2 = new HospitalManagementSystem.Models.User
    {
        FullName = "Bob Wilson",
        Email = "bob.wilson@email.com",
        PasswordHash = HashPassword("Patient123!"),
        Role = "Patient"
    };
    context.Users.AddRange(patientUser1, patientUser2);
    context.SaveChanges();

    // Create Patients
    var patient1 = new HospitalManagementSystem.Models.Patient
    {
        UserId = patientUser1.Id,
        DateOfBirth = new DateTime(1985, 5, 15),
        BloodType = "O+",
        Address = "123 Main Street, City, Country"
    };
    var patient2 = new HospitalManagementSystem.Models.Patient
    {
        UserId = patientUser2.Id,
        DateOfBirth = new DateTime(1990, 8, 22),
        BloodType = "A-",
        Address = "456 Second Avenue, City, Country"
    };
    context.Patients.AddRange(patient1, patient2);
    context.SaveChanges();

    // Create sample Appointments
    var appointment1 = new HospitalManagementSystem.Models.Appointment
    {
        DoctorId = doctor1.Id,
        PatientId = patient1.Id,
        AppointmentDate = DateTime.Now.AddDays(5),
        Status = "Pending",
        Notes = "Initial consultation"
    };
    var appointment2 = new HospitalManagementSystem.Models.Appointment
    {
        DoctorId = doctor2.Id,
        PatientId = patient2.Id,
        AppointmentDate = DateTime.Now.AddDays(7),
        Status = "Confirmed",
        Notes = "Follow-up appointment"
    };
    context.Appointments.AddRange(appointment1, appointment2);

    context.SaveChanges();
}
