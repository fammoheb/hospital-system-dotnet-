const API_BASE_URL = 'https://localhost:5001/api';
let currentDepartmentId = null;

async function initDepartments() {
    checkAuthentication();
    const user = JSON.parse(localStorage.getItem('user'));
    const token = localStorage.getItem('token');

    document.getElementById('userInfo').textContent = `Welcome, ${user.fullName}`;

    // Show add department button only for admins
    if (user.role === 'Admin') {
        document.getElementById('adminActions').style.display = 'flex';
    }

    // Load departments
    await loadDepartments(token);
}

async function loadDepartments(token) {
    try {
        const response = await fetch(`${API_BASE_URL}/departments`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (response.status === 401) {
            logout();
            return;
        }

        const departments = await response.json();
        const container = document.getElementById('departmentsContainer');

        if (departments.length === 0) {
            container.innerHTML = '<div class="empty-state"><h3>No departments found</h3></div>';
            return;
        }

        container.innerHTML = `
            <div style="display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 1.5rem;">
                ${departments.map(dept => `
                    <div class="card">
                        <div class="card-header">
                            <h3 style="margin: 0;">${dept.name}</h3>
                        </div>
                        <p style="margin: 1rem 0; color: var(--text-light);">${dept.description}</p>
                        <div class="btn-group">
                            ${getActionButtons(dept)}
                        </div>
                    </div>
                `).join('')}
            </div>
        `;
    } catch (error) {
        showToast('error', 'Failed to load departments');
        console.error(error);
    }
}

function getActionButtons(department) {
    const user = JSON.parse(localStorage.getItem('user'));
    const buttons = [];

    if (user.role === 'Admin') {
        buttons.push(`<button class="btn btn-primary btn-small" onclick="editDepartment(${department.id})">Edit</button>`);
        buttons.push(`<button class="btn btn-danger btn-small" onclick="deleteDepartment(${department.id})">Delete</button>`);
    }

    return buttons.join('');
}

function openAddDepartmentModal() {
    currentDepartmentId = null;
    document.getElementById('modalTitle').textContent = 'Add Department';
    document.getElementById('departmentForm').reset();
    document.getElementById('departmentModal').classList.add('active');
}

function closeDepartmentModal() {
    document.getElementById('departmentModal').classList.remove('active');
}

async function editDepartment(id) {
    const token = localStorage.getItem('token');
    try {
        const response = await fetch(`${API_BASE_URL}/departments/${id}`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        const department = await response.json();

        currentDepartmentId = id;
        document.getElementById('modalTitle').textContent = 'Edit Department';
        document.getElementById('departmentName').value = department.name;
        document.getElementById('departmentDescription').value = department.description;
        document.getElementById('departmentModal').classList.add('active');
    } catch (error) {
        showToast('error', 'Failed to load department details');
    }
}

async function deleteDepartment(id) {
    if (!confirm('Are you sure you want to delete this department?')) return;

    const token = localStorage.getItem('token');
    try {
        const response = await fetch(`${API_BASE_URL}/departments/${id}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (response.ok) {
            showToast('success', 'Department deleted successfully');
            await loadDepartments(token);
        } else {
            showToast('error', 'Failed to delete department');
        }
    } catch (error) {
        showToast('error', error.message);
    }
}

document.getElementById('departmentForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const token = localStorage.getItem('token');

    const departmentData = {
        name: document.getElementById('departmentName').value,
        description: document.getElementById('departmentDescription').value,
    };

    try {
        let response;
        if (currentDepartmentId) {
            response = await fetch(`${API_BASE_URL}/departments/${currentDepartmentId}`, {
                method: 'PUT',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(departmentData)
            });
        } else {
            response = await fetch(`${API_BASE_URL}/departments`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(departmentData)
            });
        }

        if (response.ok) {
            showToast('success', currentDepartmentId ? 'Department updated successfully' : 'Department created successfully');
            closeDepartmentModal();
            await loadDepartments(token);
        } else {
            const error = await response.json();
            showToast('error', error.message || 'Failed to save department');
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

document.addEventListener('DOMContentLoaded', initDepartments);
