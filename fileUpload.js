// Click element helper
function clickElement(element) {
    element.click();
}

// Download file helper
function downloadFile(storedFileName, originalFileName) {
    const url = `/api/files/download/${encodeURIComponent(storedFileName)}?originalName=${encodeURIComponent(originalFileName)}`;
    window.open(url, '_blank');
}

// Drag and drop enhancements
window.addEventListener('DOMContentLoaded', () => {
    // Prevent default drag behaviors
    ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
        document.addEventListener(eventName, preventDefaults, false);
    });

    function preventDefaults(e) {
        e.preventDefault();
        e.stopPropagation();
    }
});