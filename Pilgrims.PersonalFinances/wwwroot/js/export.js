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
        alert('Error exporting to PDF. Please try again.');
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
        alert('Error downloading file. Please try again.');
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
        alert('Error exporting to image. Please try again.');
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
        alert('Error exporting chart as SVG. Please try again.');
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
        alert('Error printing. Please try again.');
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

// Show toast notification
window.showToast = (message, type = 'info') => {
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.textContent = message;
    toast.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 12px 20px;
        border-radius: 6px;
        color: white;
        font-weight: 500;
        z-index: 10000;
        transition: all 0.3s ease;
        ${type === 'success' ? 'background-color: #10b981;' : ''}
        ${type === 'error' ? 'background-color: #ef4444;' : ''}
        ${type === 'info' ? 'background-color: #3b82f6;' : ''}
    `;
    
    document.body.appendChild(toast);
    
    // Animate in
    setTimeout(() => {
        toast.style.transform = 'translateX(0)';
        toast.style.opacity = '1';
    }, 10);
    
    // Remove after 3 seconds
    setTimeout(() => {
        toast.style.transform = 'translateX(100%)';
        toast.style.opacity = '0';
        setTimeout(() => {
            if (document.body.contains(toast)) {
                document.body.removeChild(toast);
            }
        }, 300);
    }, 3000);
};

// Initialize export functionality when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    console.log('Export functionality initialized');
    
    // Check for required libraries
    if (typeof html2canvas === 'undefined') {
        console.warn('html2canvas library not found. Image export may not work.');
    }
    
    if (typeof html2pdf === 'undefined') {
        console.warn('html2pdf library not found. PDF export will use print dialog fallback.');
    }
});