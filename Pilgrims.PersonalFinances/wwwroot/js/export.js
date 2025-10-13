// Export functionality for reports

// Export to PDF using html2pdf library
window.exportToPdf = async (htmlContent, fileName) => {
    try {
        // Create a temporary div to hold the HTML content
        const tempDiv = document.createElement('div');
        tempDiv.innerHTML = htmlContent;
        tempDiv.style.position = 'absolute';
        tempDiv.style.left = '-9999px';
        tempDiv.style.top = '-9999px';
        document.body.appendChild(tempDiv);

        // Use html2pdf if available, otherwise fallback to window.print
        if (typeof html2pdf !== 'undefined') {
            const opt = {
                margin: 1,
                filename: fileName,
                image: { type: 'jpeg', quality: 0.98 },
                html2canvas: { scale: 2 },
                jsPDF: { unit: 'in', format: 'letter', orientation: 'portrait' }
            };
            
            await html2pdf().set(opt).from(tempDiv).save();
        } else {
            // Fallback: open print dialog
            const printWindow = window.open('', '_blank');
            printWindow.document.write(htmlContent);
            printWindow.document.close();
            printWindow.print();
        }

        // Clean up
        document.body.removeChild(tempDiv);
    } catch (error) {
        console.error('Error exporting to PDF:', error);
        showToast('Error exporting to PDF. Please try again.', 'error');
    }
};

// Download file with specified content and type
window.downloadFile = (content, fileName, contentType) => {
    try {
        const blob = new Blob([content], { type: contentType });
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = fileName;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
    } catch (error) {
        console.error('Error downloading file:', error);
        showToast('Error downloading file. Please try again.', 'error');
    }
};

// Export element to image using html2canvas
window.exportToImage = async (elementId, fileName) => {
    try {
        const element = document.getElementById(elementId);
        if (!element) {
            throw new Error(`Element with ID '${elementId}' not found`);
        }

        // Use html2canvas if available
        if (typeof html2canvas !== 'undefined') {
            const canvas = await html2canvas(element, {
                backgroundColor: '#ffffff',
                scale: 2,
                useCORS: true,
                allowTaint: true
            });
            
            // Convert canvas to blob and download
            canvas.toBlob((blob) => {
                const url = window.URL.createObjectURL(blob);
                const link = document.createElement('a');
                link.href = url;
                link.download = fileName;
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                window.URL.revokeObjectURL(url);
            }, 'image/png');
        } else {
            throw new Error('html2canvas library not available');
        }
    } catch (error) {
        console.error('Error exporting to image:', error);
        showToast('Error exporting to image. Please try again.', 'error');
    }
};

// Export chart as SVG
window.exportChartAsSvg = (elementId, fileName) => {
    try {
        const element = document.getElementById(elementId);
        if (!element) {
            throw new Error(`Element with ID '${elementId}' not found`);
        }

        const svgElement = element.querySelector('svg');
        if (!svgElement) {
            throw new Error('No SVG element found in the specified element');
        }

        // Clone the SVG to avoid modifying the original
        const clonedSvg = svgElement.cloneNode(true);
        
        // Add XML declaration and styling
        const svgData = new XMLSerializer().serializeToString(clonedSvg);
        const svgBlob = new Blob([svgData], { type: 'image/svg+xml;charset=utf-8' });
        
        const url = window.URL.createObjectURL(svgBlob);
        const link = document.createElement('a');
        link.href = url;
        link.download = fileName.replace('.png', '.svg');
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
    } catch (error) {
        console.error('Error exporting SVG:', error);
        showToast('Error exporting chart as SVG. Please try again.', 'error');
    }
};

// Print specific element
window.printElement = (elementId) => {
    try {
        const element = document.getElementById(elementId);
        if (!element) {
            throw new Error(`Element with ID '${elementId}' not found`);
        }

        const printWindow = window.open('', '_blank');
        const printDocument = printWindow.document;
        
        // Copy styles from parent document
        const styles = Array.from(document.styleSheets)
            .map(styleSheet => {
                try {
                    return Array.from(styleSheet.cssRules)
                        .map(rule => rule.cssText)
                        .join('\n');
                } catch (e) {
                    return '';
                }
            })
            .join('\n');

        printDocument.write(`
            <!DOCTYPE html>
            <html>
            <head>
                <title>Print Report</title>
                <style>
                    ${styles}
                    @media print {
                        body { margin: 0; }
                        .no-print { display: none !important; }
                    }
                </style>
            </head>
            <body>
                ${element.outerHTML}
            </body>
            </html>
        `);
        
        printDocument.close();
        
        // Wait for content to load then print
        printWindow.onload = () => {
            printWindow.print();
            printWindow.close();
        };
    } catch (error) {
        console.error('Error printing element:', error);
        showToast('Error printing. Please try again.', 'error');
    }
};

// Copy data to clipboard
window.copyToClipboard = async (text) => {
    try {
        if (navigator.clipboard && window.isSecureContext) {
            await navigator.clipboard.writeText(text);
        } else {
            // Fallback for older browsers
            const textArea = document.createElement('textarea');
            textArea.value = text;
            textArea.style.position = 'fixed';
            textArea.style.left = '-999999px';
            textArea.style.top = '-999999px';
            document.body.appendChild(textArea);
            textArea.focus();
            textArea.select();
            document.execCommand('copy');
            document.body.removeChild(textArea);
        }
        
        // Show success message
        showToast('Data copied to clipboard!', 'success');
    } catch (error) {
        console.error('Error copying to clipboard:', error);
        showToast('Error copying to clipboard', 'error');
    }
};

// Show toast notification via Toastr (supports both (message, type) and (type, message) signatures)
// Removed: legacy implementation. Delegating to Notifications module.
window.showToast = function(arg1, arg2, arg3) {
    if (window.Notifications && typeof window.Notifications.showToast === 'function') {
        window.Notifications.showToast(arg1, arg2, arg3);
    } else {
        console.error('Notifications module not available for showToast');
    }
};

// Show alert styled via Toastr, returns a Promise<boolean)
// Removed: legacy implementation. Delegating to Notifications module.
window.showAlertToast = function(title = 'Alert', message = '', buttonText = 'OK') {
    if (window.Notifications && typeof window.Notifications.showAlertToast === 'function') {
        return window.Notifications.showAlertToast(title, message, buttonText);
    }
    console.error('Notifications module not available for showAlertToast');
    return Promise.resolve(false);
};

// Show confirmation styled via Toastr, returns a Promise<boolean)
// Removed: legacy implementation. Delegating to Notifications module.
window.showConfirmationToast = function(title = 'Confirm', message = '', confirmText = 'Yes', cancelText = 'No') {
    if (window.Notifications && typeof window.Notifications.showConfirmationToast === 'function') {
        return window.Notifications.showConfirmationToast(title, message, confirmText, cancelText);
    }
    console.error('Notifications module not available for showConfirmationToast');
    return Promise.resolve(false);
};

// Initialize export functionality when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    console.log('Export functionality initialized');
    // Configure Toastr defaults if loaded
    if (typeof toastr !== 'undefined') {
        toastr.options = {
            closeButton: true,
            progressBar: true,
            preventDuplicates: false,
            newestOnTop: true,
            positionClass: 'toast-top-right',
            showDuration: 300,
            hideDuration: 300,
            timeOut: 5000,
            extendedTimeOut: 1000,
            tapToDismiss: true,
            escapeHtml: false
        };
    }
    
    // Check for required libraries
    if (typeof html2canvas === 'undefined') {
        console.warn('html2canvas library not found. Image export may not work.');
    }
});