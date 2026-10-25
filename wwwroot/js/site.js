/**
 * site.js - Common JS utilities for TrungTamNgoaiNgu
 */

document.addEventListener('DOMContentLoaded', function () {
    // Auto-dismiss notification alerts after 4 seconds
    const alerts = document.querySelectorAll('.edu-alert-auto-dismiss');
    alerts.forEach(function (alert) {
        setTimeout(function () {
            alert.style.transition = 'opacity 0.4s ease, transform 0.4s ease';
            alert.style.opacity = '0';
            alert.style.transform = 'translateY(-10px)';
            setTimeout(function () {
                alert.remove();
            }, 400);
        }, 4000);
    });
});

// Confirmation helper for Delete or State change actions
function confirmAction(message, formId) {
    if (confirm(message || 'Bạn có chắc chắn muốn thực hiện thao tác này không?')) {
        if (formId) {
            document.getElementById(formId).submit();
        }
        return true;
    }
    return false;
}
