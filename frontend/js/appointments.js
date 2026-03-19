const API_BASE_URL = 'https://localhost:5001/api';
let currentAppointmentId = null;

async function initAppointments() {
    checkAuthentication();
    const user = JSON.parse(localStorage.getItem('user'));
    const token = localStorage.getItem('token');

    document.getElementById('userInfo').textContent = `Welcome, ${user.fullName}`;

    // Show book appointment button only for patients
    if (user.role === 'Patient') {
        document.getElementById('patientActions').style.display = 'flex';
        await loadDoctorsForBooking(token);
    }

    // Load appointments
    await loadAppointments(token, user);
}

async function loadDoctorsForBooking(token) {
    try {
        const response = await fetch(`${API_BASE_URL}/doctors`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        const doctors = await response.json();

        const select = document.getElementById('appointmentDoctor');
        doctors.forEach(doctor => {
            const option = document.createElement('option');
            option.value = doctor.id;
            option.textContent = `${doctor.doctorName} - ${doctor.specialization}`;
            select.appendChild(option);
        });
    } catch (error) {
        console.error('Error loading doctors:', error);
    }
}

async function loadAppointments(token, user) {
    try {
        const response = await fetch(`${API_BASE_URL}/appointments`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (response.status === 401) {
            logout();
            return;
        }

        let appointments = await response.json();

        // Filter appointments based on user role
        if (user.role === 'Patient') {
            // For patients, only show their own appointments
            const patients = await fetch(`${API_BASE_URL}/patients`, {
                headers: { 'Authorization': `Bearer ${token}` }
            }).then(r => r.json());
            
            const patientProfile = patients.find(p => p.userId.toString() === user.id.toString());
            if (patientProfile) {
                appointments = appointments.filter(a => a.patientId === patientProfile.id);
            }
        } else if (user.role === 'Doctor') {
            // For doctors, only show their own appointments
            const doctors = await fetch(`${API_BASE_URL}/doctors`, {
                headers: { 'Authorization': `Bearer ${token}` }
            }).then(r => r.json());
            
            const doctorProfile = doctors.find(d => d.userId.toString() === user.id.toString());
            if (doctorProfile) {
                appointments = appointments.filter(a => a.doctorId === doctorProfile.id);
            }
        }

        renderAppointments(appointments, user);
    } catch (error) {
        showToast('error', 'Failed to load appointments');
        console.error(error);
    }
}

function renderAppointments(appointments, user) {
    const container = document.getElementById('appointmentsContainer');

    if (appointments.length === 0) {
        container.innerHTML = '<div class="empty-state"><h3>No appointments found</h3></div>';
        return;
    }

    container.innerHTML = `
        <table class="table">
            <thead>
                <tr>
                    <th>Doctor</th>
                    <th>Patient</th>
                    <th>Date & Time</th>
                    <th>Status</th>
                    <th>Notes</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                ${appointments.map(apt => `
                    <tr>
                        <td>${apt.doctorName}</td>
                        <td>${apt.patientName}</td>
                        <td>${formatDateTime(apt.appointmentDate)}</td>
                        <td><span style="padding: 0.25rem 0.75rem; border-radius: 4px; background-color: ${getStatusColor(apt.status)}">${apt.status}</span></td>
                        <td>${apt.notes || '-'}</td>
                        <td>
                            <div class="btn-group">
                                ${getAppointmentActions(apt, user)}
                            </div>
                        </td>
                    </tr>
                `).join('')}
            </tbody>
        </table>
    `;
}

function getAppointmentActions(appointment, user) {
    const buttons = [];

    if ((user.role === 'Admin' || user.role === 'Doctor') && user.role !== 'Patient') {
        buttons.push(`<button class="btn btn-primary btn-small" onclick="openUpdateStatusModal(${appointment.id})">Update</button>`);
    }

    if (user.role === 'Admin') {
        buttons.push(`<button class="btn btn-danger btn-small" onclick="deleteAppointment(${appointment.id})">Delete</button>`);
    }

    return buttons.join('');
}

function openBookAppointmentModal() {
    currentAppointmentId = null;
    document.getElementById('bookAppointmentForm').reset();
    document.getElementById('bookAppointmentModal').classList.add('active');
}

function closeBookAppointmentModal() {
    document.getElementById('bookAppointmentModal').classList.remove('active');
}

function openUpdateStatusModal(appointmentId) {
    currentAppointmentId = appointmentId;
    document.getElementById('updateStatusForm').reset();
    document.getElementById('updateStatusModal').classList.add('active');
}

function closeUpdateStatusModal() {
    document.getElementById('updateStatusModal').classList.remove('active');
}

document.getElementById('bookAppointmentForm').addEventListener('submit', async (e) => {
    e.preventDefault();

    const token = localStorage.getItem('token');
    const user = JSON.parse(localStorage.getItem('user'));

    // Get patient profile ID
    const patients = await fetch(`${API_BASE_URL}/patients`, {
        headers: { 'Authorization': `Bearer ${token}` }
    }).then(r => r.json());

    const patientProfile = patients.find(p => p.userId.toString() === user.id.toString());

    if (!patientProfile) {
        showToast('error', 'Patient profile not found');
        return;
    }

    const appointmentData = {
        doctorId: parseInt(document.getElementById('appointmentDoctor').value),
        patientId: patientProfile.id,
        appointmentDate: new Date(document.getElementById('appointmentDate').value).toISOString(),
        notes: document.getElementById('appointmentNotes').value
    };

    try {
        const response = await fetch(`${API_BASE_URL}/appointments`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(appointmentData)
        });

        if (response.ok) {
            showToast('success', 'Appointment booked successfully');
            closeBookAppointmentModal();
            location.reload();
        } else {
            const error = await response.json();
            showToast('error', error.message || 'Failed to book appointment');
        }
    } catch (error) {
        showToast('error', error.message);
    }
});

document.getElementById('updateStatusForm').addEventListener('submit', async (e) => {
    e.preventDefault();

    const token = localStorage.getItem('token');

    const updateData = {
        status: document.getElementById('updateStatus').value,
        notes: document.getElementById('updateNotes').value
    };

    try {
        const response = await fetch(`${API_BASE_URL}/appointments/${currentAppointmentId}`, {
            method: 'PUT',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(updateData)
        });

        if (response.ok) {
            showToast('success', 'Appointment status updated successfully');
            closeUpdateStatusModal();
            location.reload();
        } else {
            const error = await response.json();
            showToast('error', error.message || 'Failed to update appointment');
        }
    } catch (error) {
        showToast('error', error.message);
    }
});

async function deleteAppointment(id) {
    if (!confirm('Are you sure you want to delete this appointment?')) return;

    const token = localStorage.getItem('token');
    try {
        const response = await fetch(`${API_BASE_URL}/appointments/${id}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (response.ok) {
            showToast('success', 'Appointment deleted successfully');
            location.reload();
        } else {
            showToast('error', 'Failed to delete appointment');
        }
    } catch (error) {
        showToast('error', error.message);
    }
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

document.addEventListener('DOMContentLoaded', initAppointments);
