const API_BASE_URL = 'https://localhost:5001/api';
let currentPatientId = null;

async function initPatients() {
    checkAuthentication();
    const user = JSON.parse(localStorage.getItem('user'));
    const token = localStorage.getItem('token');

    document.getElementById('userInfo').textContent = `Welcome, ${user.fullName}`;

    // Show add patient button only for admins
    if (user.role === 'Admin') {
        document.getElementById('adminActions').style.display = 'flex';
    }

    // Load patients
    await loadPatients(token);
}

async function loadPatients(token) {
    try {
        const response = await fetch(`${API_BASE_URL}/patients`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (response.status === 401) {
            logout();
            return;
        }

        const patients = await response.json();
        const container = document.getElementById('patientsContainer');

        if (patients.length === 0) {
            container.innerHTML = '<div class="empty-state"><h3>No patients found</h3></div>';
            return;
        }

        container.innerHTML = `
            <table class="table">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Email</th>
                        <th>Date of Birth</th>
                        <th>Blood Type</th>
                        <th>Address</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    ${patients.map(patient => `
                        <tr>
                            <td>${patient.patientName}</td>
                            <td>${patient.userId}</td>
                            <td>${new Date(patient.dateOfBirth).toLocaleDateString()}</td>
                            <td>${patient.bloodType}</td>
                            <td>${patient.address}</td>
                            <td>
                                <div class="btn-group">
                                    ${getActionButtons(patient)}
                                </div>
                            </td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        `;
    } catch (error) {
        showToast('error', 'Failed to load patients');
        console.error(error);
    }
}

function getActionButtons(patient) {
    const user = JSON.parse(localStorage.getItem('user'));
    const buttons = [];

    if (user.role === 'Admin') {
        buttons.push(`<button class="btn btn-primary btn-small" onclick="editPatient(${patient.id})">Edit</button>`);
        buttons.push(`<button class="btn btn-danger btn-small" onclick="deletePatient(${patient.id})">Delete</button>`);
    }

    return buttons.join('');
}

function openAddPatientModal() {
    currentPatientId = null;
    document.getElementById('modalTitle').textContent = 'Add Patient';
    document.getElementById('patientForm').reset();
    document.getElementById('patientPassword').style.display = 'block';
    document.getElementById('patientModal').classList.add('active');
}

function closePatientModal() {
    document.getElementById('patientModal').classList.remove('active');
}

async function editPatient(id) {
    const token = localStorage.getItem('token');
    try {
        const response = await fetch(`${API_BASE_URL}/patients/${id}`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        const patient = await response.json();

        currentPatientId = id;
        document.getElementById('modalTitle').textContent = 'Edit Patient';
        document.getElementById('patientFullName').value = patient.patientName;
        document.getElementById('patientDateOfBirth').value = patient.dateOfBirth.split('T')[0];
        document.getElementById('patientBloodType').value = patient.bloodType;
        document.getElementById('patientAddress').value = patient.address;
        document.getElementById('patientPassword').style.display = 'none';
        document.getElementById('patientModal').classList.add('active');
    } catch (error) {
        showToast('error', 'Failed to load patient details');
    }
}

async function deletePatient(id) {
    if (!confirm('Are you sure you want to delete this patient?')) return;

    const token = localStorage.getItem('token');
    try {
        const response = await fetch(`${API_BASE_URL}/patients/${id}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (response.ok) {
            showToast('success', 'Patient deleted successfully');
            await loadPatients(token);
        } else {
            showToast('error', 'Failed to delete patient');
        }
    } catch (error) {
        showToast('error', error.message);
    }
}

document.getElementById('patientForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const token = localStorage.getItem('token');

    const patientData = {
        userId: currentPatientId ? currentPatientId : parseInt(localStorage.getItem('currentUserId') || '0'),
        dateOfBirth: document.getElementById('patientDateOfBirth').value,
        bloodType: document.getElementById('patientBloodType').value,
        address: document.getElementById('patientAddress').value,
    };

    try {
        let response;
        if (currentPatientId) {
            response = await fetch(`${API_BASE_URL}/patients/${currentPatientId}`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(patientData)
            });
        } else {
            response = await fetch(`${API_BASE_URL}/patients`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(patientData)
            });
        }

        if (response.ok) {
            showToast('success', currentPatientId ? 'Patient updated successfully' : 'Patient created successfully');
            closePatientModal();
            await loadPatients(token);
        } else {
            const error = await response.json();
            showToast('error', error.message || 'Failed to save patient');
        }
    } catch (error) {
        showToast('error', error.message);
    }
});

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

document.addEventListener('DOMContentLoaded', initPatients);
