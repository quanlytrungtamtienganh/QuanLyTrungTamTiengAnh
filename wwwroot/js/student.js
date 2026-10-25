/**
 * student.js - Student Portal interactions
 */

document.addEventListener('DOMContentLoaded', function () {
    const mobileMenuToggle = document.querySelector('.student-mobile-toggle');
    const studentNavMenu = document.querySelector('.student-nav-menu');

    if (mobileMenuToggle && studentNavMenu) {
        mobileMenuToggle.addEventListener('click', function () {
            studentNavMenu.classList.toggle('show');
        });
    }

    // Active menu item auto-highlighting
    const currentPath = window.location.pathname.toLowerCase();
    const navLinks = document.querySelectorAll('.student-nav-link');
    
    navLinks.forEach(link => {
        const href = link.getAttribute('href').toLowerCase();
        if (href !== '/' && currentPath.startsWith(href)) {
            link.classList.add('active');
        } else if (href === '/' && currentPath === '/') {
            link.classList.add('active');
        }
    });
});
