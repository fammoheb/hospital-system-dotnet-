const API_BASE_URL = 'https://localhost:5001/api';

async function initDashboard() {
    checkAuthentication();

    const user = JSON.parse(localStorage.getItem('user'));
    const token = localStorage.getItem('token');

    // Update user info in navbar
    document.getElementById('userInfo').textContent = `Welcome, ${user.fullName}`;
    updateNavbar(user.role);

    // Show appropriate dashboard based on role
    if (user.role === 'Admin') {
        showAdminDashboard(token);
    } else if (user.role === 'Doctor') {
        showDoctorDashboard(token);
    } else if (user.role === 'Patient') {
        showPatientDashboard(token);
    }
}

function updateNavbar(role) {
    const navbar = document.getElementById('navbar-nav');
    
    if (role !== 'Admin') {
        // Hide certain links for non-admin users
        if (role === 'Patient') {
            // Patients can't see all links
            navbar.querySelectorAll('a').forEach(link => {
                const href = link.getAttribute('href');
                if (href === 'patients.html') {
                    link.parentElement.style.display = 'none';
                }
            });
        }
    }
}

async function showAdminDashboard(token) {
    document.getElementById('adminDashboard').style.display = 'block';

    try {
        // Fetch statistics
        const [doctors, patients, appointments, departments] = await Promise.all([
            fetchAPI('/doctors', token),
            fetchAPI('/patients', token),
            fetchAPI('/appointments', token),
            fetchAPI('/departments', token),
        ]);

        document.getElementById('doctorCount').textContent = doctors.length;
        document.getElementById('patientCount').textContent = patients.length;
        document.getElementById('appointmentCount').textContent = appointments.length;
        document.getElementById('departmentCount').textContent = departments.length;
    } catch (error) {
        showToast('error', 'Failed to load dashboard statistics');
        console.error(error);
    }
}

async function showDoctorDashboard(token) {
    document.getElementById('doctorDashboard').style.display = 'block';

    try {
        // Get doctor's profile first
        const doctors = await fetchAPI('/doctors', token);
        const user = JSON.parse(localStorage.getItem('user'));
        
        const doctorProfile = doctors.find(d => d.userId.toString() === user.id.toString());
        
        if (doctorProfile) {
            const appointments = await fetchAPI(`/appointments/doctor/${doctorProfile.id}`, token);
            
            const container = document.getElementById('doctorAppointmentsContainer');
            
            if (appointments.length === 0) {
                container.innerHTML = '<p style="text-align: center; color: var(--text-light);">No upcoming appointments</p>';
            } else {
                container.innerHTML = `
                    <table class="table">
                        <thead>
                            <tr>
                                <th>Patient Name</th>
                                <th>Date & Time</th>
                                <th>Status</th>
                                <th>Notes</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${appointments.map(apt => `
                                <tr>
                                    <td>${apt.patientName}</td>
                                    <td>${formatDateTime(apt.appointmentDate)}</td>
                                    <td><span style="padding: 0.25rem 0.75rem; border-radius: 4px; background-color: ${getStatusColor(apt.status)}">${apt.status}</span></td>
                                    <td>${apt.notes || '-'}</td>
                                </tr>
                            `).join('')}
                        </tbody>
                    </table>
                `;
            }
        }
    } catch (error) {
        const container = document.getElementById('doctorAppointmentsContainer');
        container.innerHTML = '<p style="text-align: center; color: var(--text-light);">Failed to load appointments</p>';
        console.error(error);
    }
}

async function showPatientDashboard(token) {
    document.getElementById('patientDashboard').style.display = 'block';

    try {
        // Get patient's profile first
        const patients = await fetchAPI('/patients', token);
        const user = JSON.parse(localStorage.getItem('user'));
        
        const patientProfile = patients.find(p => p.userId.toString() === user.id.toString());
        
        if (patientProfile) {
            const appointments = await fetchAPI(`/appointments/patient/${patientProfile.id}`, token);
            
            const container = document.getElementById('patientAppointmentsContainer');
            
            if (appointments.length === 0) {
                container.innerHTML = '<div class="empty-state"><h3>No appointments yet</h3><p>Book your first appointment with a doctor</p></div>';
            } else {
                container.innerHTML = `
                    <table class="table">
                        <thead>
                            <tr>
                                <th>Doctor Name</th>
                                <th>Specialization</th>
                                <th>Date & Time</th>
                                <th>Status</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${appointments.map(apt => `
                                <tr>
                                    <td>${apt.doctorName}</td>
                                    <td>${apt.patientName}</td>
                                    <td>${formatDateTime(apt.appointmentDate)}</td>
                                    <td><span style="padding: 0.25rem 0.75rem; border-radius: 4px; background-color: ${getStatusColor(apt.status)}">${apt.status}</span></td>
                                </tr>
                            `).join('')}
                        </tbody>
                    </table>
                `;
            }
        }
    } catch (error) {
        const container = document.getElementById('patientAppointmentsContainer');
        container.innerHTML = '<p style="text-align: center; color: var(--text-light);">Failed to load appointments</p>';
        console.error(error);
    }
}

async function fetchAPI(endpoint, token) {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        }
    });

    if (response.status === 401) {
        logout();
    }

    if (!response.ok) {
        throw new Error('Failed to fetch data');
    }

    return await response.json();
}

function checkAuthentication() {
    const token = localStorage.getItem('token');
    const user = localStorage.getItem('user');

    if (!token || !user) {
        window.location.href = 'login.html';
        return;
    }
}

function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    window.location.href = 'login.html';
}

function showToast(type, message) {
    const container = document.getElementById('toastContainer');
    const toast = document.createElement('div');
    toast.className = `toast ${type}`;
    
    const icons = {
        success: '✓',
        error: '✕',
        warning: '⚠',
        info: 'ℹ'
    };

    toast.innerHTML = `
        <div class="toast-icon">${icons[type]}</div>
        <div class="toast-message">${message}</div>
        <button class="toast-close" onclick="this.parentElement.remove()">×</button>
    `;

    container.appendChild(toast);

    setTimeout(() => {
        if (toast.parentElement) {
            toast.remove();
        }
    }, 5000);
}

function formatDateTime(dateString) {
    const date = new Date(dateString);
    return date.toLocaleString();
}

function getStatusColor(status) {
    const colors = {
        'Pending': '#fbbf24',
        'Confirmed': '#10b981',
        'Completed': '#3b82f6',
        'Cancelled': '#ef4444'
    };
    return colors[status] || '#e5e7eb';
}

// Initialize dashboard when page loads
document.addEventListener('DOMContentLoaded', initDashboard);
