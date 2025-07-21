// Guide.js - JavaScript for the user guide page

document.addEventListener('DOMContentLoaded', function() {
    initializeGuideNavigation();
    initializeScrollSpy();
    initializeSmoothScrolling();
});

// Initialize guide navigation
function initializeGuideNavigation() {
    const navLinks = document.querySelectorAll('.nav-link');
    
    navLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Remove active class from all links
            navLinks.forEach(l => l.classList.remove('active'));
            
            // Add active class to clicked link
            this.classList.add('active');
            
            // Get target section
            const targetId = this.getAttribute('href').substring(1);
            const targetSection = document.getElementById(targetId);
            
            if (targetSection) {
                // Smooth scroll to section
                targetSection.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
}

// Initialize scroll spy to highlight current section
function initializeScrollSpy() {
    const sections = document.querySelectorAll('.guide-section');
    const navLinks = document.querySelectorAll('.nav-link');
    
    const observerOptions = {
        root: null,
        rootMargin: '-20% 0px -70% 0px',
        threshold: 0
    };
    
    const observer = new IntersectionObserver(function(entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const sectionId = entry.target.id;
                
                // Remove active class from all nav links
                navLinks.forEach(link => link.classList.remove('active'));
                
                // Add active class to corresponding nav link
                const activeLink = document.querySelector(`a[href="#${sectionId}"]`);
                if (activeLink) {
                    activeLink.classList.add('active');
                }
            }
        });
    }, observerOptions);
    
    // Observe all sections
    sections.forEach(section => {
        observer.observe(section);
    });
}

// Initialize smooth scrolling for all internal links
function initializeSmoothScrolling() {
    const internalLinks = document.querySelectorAll('a[href^="#"]');
    
    internalLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            
            const targetId = this.getAttribute('href').substring(1);
            const targetElement = document.getElementById(targetId);
            
            if (targetElement) {
                targetElement.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
}

// Add search functionality (future enhancement)
function initializeSearch() {
    const searchInput = document.getElementById('guide-search');
    const sections = document.querySelectorAll('.guide-section');
    
    if (searchInput) {
        searchInput.addEventListener('input', function() {
            const searchTerm = this.value.toLowerCase();
            
            sections.forEach(section => {
                const content = section.textContent.toLowerCase();
                const shouldShow = content.includes(searchTerm) || searchTerm === '';
                
                section.style.display = shouldShow ? 'block' : 'none';
            });
        });
    }
}

// Print functionality
function printGuide() {
    window.print();
}

// Copy section link to clipboard
function copyLinkToSection(sectionId) {
    const url = window.location.origin + window.location.pathname + '#' + sectionId;
    
    if (navigator.clipboard) {
        navigator.clipboard.writeText(url).then(function() {
            showNotification('Lien copié dans le presse-papiers!', 'success');
        });
    } else {
        // Fallback for older browsers
        const textArea = document.createElement('textarea');
        textArea.value = url;
        document.body.appendChild(textArea);
        textArea.select();
        document.execCommand('copy');
        document.body.removeChild(textArea);
        showNotification('Lien copié dans le presse-papiers!', 'success');
    }
}

// Show notification
function showNotification(message, type = 'info') {
    // Remove existing notifications
    const existingNotifications = document.querySelectorAll('.guide-notification');
    existingNotifications.forEach(notification => notification.remove());
    
    // Create new notification
    const notification = document.createElement('div');
    notification.className = `guide-notification ${type}`;
    notification.textContent = message;
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 15px 20px;
        border-radius: 5px;
        color: white;
        font-weight: bold;
        z-index: 1000;
        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        ${type === 'success' ? 'background-color: #28a745;' : 'background-color: #17a2b8;'}
    `;

    document.body.appendChild(notification);

    // Auto-remove notification after 3 seconds
    setTimeout(() => {
        if (notification.parentNode) {
            notification.remove();
        }
    }, 3000);
}

// Keyboard shortcuts
document.addEventListener('keydown', function(e) {
    // Ctrl/Cmd + F to focus search (if implemented)
    if ((e.ctrlKey || e.metaKey) && e.key === 'f') {
        const searchInput = document.getElementById('guide-search');
        if (searchInput) {
            e.preventDefault();
            searchInput.focus();
        }
    }
    
    // Escape to clear search (if implemented)
    if (e.key === 'Escape') {
        const searchInput = document.getElementById('guide-search');
        if (searchInput && searchInput.value) {
            searchInput.value = '';
            searchInput.dispatchEvent(new Event('input'));
        }
    }
});

// Export functions for global access
window.printGuide = printGuide;
window.copyLinkToSection = copyLinkToSection;
