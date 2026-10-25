/**
 * admin.js - Admin Layout interactions
 */

document.addEventListener('DOMContentLoaded', function () {
    const sidebar = document.querySelector('.admin-sidebar');
    const toggleBtn = document.querySelector('.admin-sidebar-toggle');
    const overlay = document.querySelector('.sidebar-overlay');

    if (toggleBtn && sidebar && overlay) {
        toggleBtn.addEventListener('click', function () {
            sidebar.classList.toggle('show');
            overlay.classList.toggle('active');
        });

        overlay.addEventListener('click', function () {
            sidebar.classList.remove('show');
            overlay.classList.remove('active');
        });
    }

    // Active menu item auto-highlighting
    const currentPath = window.location.pathname.toLowerCase();
    const navLinks = document.querySelectorAll('.admin-nav-link');
    
    navLinks.forEach(link => {
        const href = link.getAttribute('href').toLowerCase();
        if (href !== '/' && currentPath.startsWith(href)) {
            link.classList.add('active');
        } else if (href === '/' && (currentPath === '/' || currentPath === '/thongke' || currentPath === '/thongke/index')) {
            link.classList.add('active');
        }
    });
});
