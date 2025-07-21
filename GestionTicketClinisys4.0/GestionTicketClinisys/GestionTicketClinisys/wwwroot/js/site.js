// Site-wide JavaScript for GestionTicketClinisys

// Authentication helper functions
function checkAuthStatus() {
    fetch('/Auth/CheckAuth')
        .then(response => response.json())
        .then(data => {
            if (!data.isAuthenticated) {
                window.location.href = '/';
            }
        })
        .catch(error => {
            console.log('Auth check failed:', error);
        });
}

// Initialize site functionality
document.addEventListener('DOMContentLoaded', function() {
    console.log('GestionTicketClinisys site.js loaded');
    
    // Add any global initialization here
});

// Utility functions
function showMessage(message, type = 'info') {
    // Simple message display function
    console.log(`${type.toUpperCase()}: ${message}`);
    
    // You can enhance this to show actual UI notifications
    if (type === 'error') {
        alert('Erreur: ' + message);
    } else if (type === 'success') {
        alert('Succès: ' + message);
    }
}

// Form validation helpers
function validateRequired(input) {
    if (!input.value.trim()) {
        input.classList.add('is-invalid');
        return false;
    }
    input.classList.remove('is-invalid');
    return true;
}

// Export functions for global use
window.GestionTicketClinisys = {
    checkAuthStatus,
    showMessage,
    validateRequired
};
