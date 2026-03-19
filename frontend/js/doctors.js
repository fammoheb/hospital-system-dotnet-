const API_BASE_URL = 'https://localhost:5001/api';
let currentDoctorId = null;

async function initDoctors() {
    checkAuthentication();
    const user = JSON.parse(localStorage.getItem('user'));
    const token = localStorage.getItem('token');

    document.getElementById('userInfo').textContent = `Welcome, ${user.fullName}`;

    // Show add doctor button only for admins
    if (user.role === 'Admin') {
        document.getElementById('adminActions').style.display = 'flex';
    }

    // Load departments for modal
    await loadDepartments(token);

    // Load doctors
    await loadDoctors(token);
}

async function loadDepartments(token) {
    try {
        const response = await fetch(`${API_BASE_URL}/departments`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        const departments = await response.json();
        
        const select = document.getElementById('doctorDepartment');
        departments.forEach(dept => {
            const option = document.createElement('option');
            option.value = dept.id;
            option.textContent = dept.name;
            select.appendChild(option);
        });
    } catch (error) {
        console.error('Error loading departments:', error);
    }
}

async function loadDoctors(token) {
    try {
        const response = await fetch(`${API_BASE_URL}/doctors`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (response.status === 401) {
            logout();
            return;
        }

        const doctors = await response.json();
        const container = document.getElementById('doctorsContainer');

        if (doctors.length === 0) {
            container.innerHTML = '<div class="empty-state"><h3>No doctors found</h3></div>';
            return;
        }

        container.innerHTML = `
            <table class="table">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Specialization</th>
                        <th>Department</th>
                        <th>Experience</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    ${doctors.map(doctor => `
                        <tr>
                            <td>${doctor.doctorName}</td>
                            <td>${doctor.specialization}</td>
                            <td>${doctor.departmentName}</td>
                            <td>${doctor.yearsOfExperience} years</td>
                            <td>
                                <div class="btn-group">
                                    ${getActionButtons(doctor)}
                                </div>
                            </td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        `;
    } catch (error) {
        showToast('error', 'Failed to load doctors');
        console.error(error);
    }
}

function getActionButtons(doctor) {
    const user = JSON.parse(localStorage.getItem('user'));
    const buttons = [];

    if (user.role === 'Admin') {
        buttons.push(`<button class="btn btn-primary btn-small" onclick="editDoctor(${doctor.id})">Edit</button>`);
        buttons.push(`<button class="btn btn-danger btn-small" onclick="deleteDoctor(${doctor.id})">Delete</button>`);
    }

    return buttons.join('');
}

function openAddDoctorModal() {
    currentDoctorId = null;
    document.getElementById('modalTitle').textContent = 'Add Doctor';
    document.getElementById('doctorForm').reset();
    document.getElementById('doctorPassword').style.display = 'block';
    document.getElementById('doctorModal').classList.add('active');
}

function closeDoctorModal() {
    document.getElementById('doctorModal').classList.remove('active');
}

async function editDoctor(id) {
    const token = localStorage.getItem('token');
    try {
        const response = await fetch(`${API_BASE_URL}/doctors/${id}`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        const doctor = await response.json();

        currentDoctorId = id;
        document.getElementById('modalTitle').textContent = 'Edit Doctor';
        document.getElementById('doctorFullName').value = doctor.doctorName;
        document.getElementById('doctorSpecialization').value = doctor.specialization;
        document.getElementById('doctorBio').value = doctor.bio;
        document.getElementById('doctorYearsOfExperience').value = doctor.yearsOfExperience;
        document.getElementById('doctorLicenseNumber').value = doctor.licenseNumber;
        document.getElementById('doctorDepartment').value = doctor.departmentId;
        document.getElementById('doctorPassword').style.display = 'none';
        document.getElementById('doctorModal').classList.add('active');
    } catch (error) {
        showToast('error', 'Failed to load doctor details');
    }
}

async function deleteDoctor(id) {
    if (!confirm('Are you sure you want to delete this doctor?')) return;

    const token = localStorage.getItem('token');
    try {
        const response = await fetch(`${API_BASE_URL}/doctors/${id}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (response.ok) {
            showToast('success', 'Doctor deleted successfully');
            await loadDoctors(token);
        } else {
            showToast('error', 'Failed to delete doctor');
        }
    } catch (error) {
        showToast('error', error.message);
    }
}

document.getElementById('doctorForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const token = localStorage.getItem('token');
    const user = JSON.parse(localStorage.getItem('user'));

    const doctorData = {
        userId: user.id,
        departmentId: parseInt(document.getElementById('doctorDepartment').value),
        specialization: document.getElementById('doctorSpecialization').value,
        bio: document.getElementById('doctorBio').value,
        yearsOfExperience: parseInt(document.getElementById('doctorYearsOfExperience').value),
        licenseNumber: document.getElementById('doctorLicenseNumber').value,
    };

    try {
        let response;
        if (currentDoctorId) {
            response = await fetch(`${API_BASE_URL}/doctors/${currentDoctorId}`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(doctorData)
            });
        } else {
            response = await fetch(`${API_BASE_URL}/doctors`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(doctorData)
            });
        }

        if (response.ok) {
            showToast('success', currentDoctorId ? 'Doctor updated successfully' : 'Doctor created successfully');
            closeDoctorModal();
            await loadDoctors(token);
        } else {
            const error = await response.json();
            showToast('error', error.message || 'Failed to save doctor');
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

    const userData = JSON.parse(user);
    if (userData.role === 'Patient') {
        // Redirect patients to dashboard
        window.location.href = 'dashboard.html';
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

document.addEventListener('DOMContentLoaded', initDoctors);
