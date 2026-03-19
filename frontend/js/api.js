// API Configuration
const API_BASE_URL = 'http://localhost:5003/api';

// Export API base URL
export { API_BASE_URL };

/**
 * Fetches data from the API with automatic JWT token attachment
 * @param {string} url - The API endpoint URL (relative to API_BASE_URL)
 * @param {object} options - Fetch options
 * @returns {Promise<object>} The response data
 */
export async function authFetch(url, options = {}) {
    const token = localStorage.getItem('token');
    
    const headers = {
        'Content-Type': 'application/json',
        ...options.headers,
    };

    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    const fullUrl = url.startsWith('http') ? url : `${API_BASE_URL}${url}`;
    
    const response = await fetch(fullUrl, {
        ...options,
        headers,
    });

    // If unauthorized, redirect to login
    if (response.status === 401) {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        window.location.href = '/login.html';
        throw new Error('Unauthorized - please login again');
    }

    // Try to parse JSON response
    let data;
    const contentType = response.headers.get('content-type');
    if (contentType && contentType.includes('application/json')) {
        data = await response.json();
    }

    if (!response.ok) {
        throw new Error(data?.message || `HTTP error! status: ${response.status}`);
    }

    return data;
}

/**
 * Decodes JWT token and returns user information
 * @returns {object|null} User object with id, email, and role, or null if no valid token
 */
export function getCurrentUser() {
    const token = localStorage.getItem('token');
    
    if (!token) {
        return null;
    }

    try {
        // Decode JWT (without verifying signature)
        const parts = token.split('.');
        if (parts.length !== 3) {
            return null;
        }

        const decoded = JSON.parse(atob(parts[1]));
        
        return {
            id: parsed.id || decoded.sub,
            email: decoded.email,
            role: decoded.role,
        };
    } catch (error) {
        console.error('Error decoding token:', error);
        return null;
    }
}

/**
 * Logs out the current user
 */
export function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    window.location.href = '/login.html';
}

/**
 * Checks if user is authenticated
 * @returns {boolean} True if user has valid JWT token
 */
export function isAuthenticated() {
    return !!localStorage.getItem('token');
}

/**
 * Decodes JWT token and returns the claims
 * @param {string} token - JWT token
 * @returns {object|null} Decoded token or null if invalid
 */
function decodeToken(token) {
    try {
        const parts = token.split('.');
        if (parts.length !== 3) return null;
        
        return JSON.parse(atob(parts[1]));
    } catch (error) {
        return null;
    }
}
