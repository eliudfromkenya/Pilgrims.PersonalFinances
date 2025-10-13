(function () {
  function parseToastArgs(arg1, arg2, arg3) {
    const isType = (v) => typeof v === 'string' && ['success', 'error', 'warning', 'info'].includes(v.toLowerCase());
    let message = '';
    let type = 'info';
    let duration = 5000;
    if (isType(arg1) && typeof arg2 === 'string') {
      type = arg1.toLowerCase();
      message = arg2;
      if (typeof arg3 === 'number') duration = arg3;
    } else {
      message = typeof arg1 === 'string' ? arg1 : String(arg1);
      if (isType(arg2)) type = arg2.toLowerCase();
      if (typeof arg2 === 'number') duration = arg2;
      if (typeof arg3 === 'number') duration = arg3;
    }
    return { message, type, duration };
  }

  function configureToastrOptions() {
    if (typeof toastr === 'undefined') return;
    const isDark = document.documentElement.classList.contains('dark');
    toastr.options = {
      closeButton: true,
      progressBar: true,
      preventDuplicates: false,
      newestOnTop: false,
      positionClass: 'toast-bottom-right',
      showDuration: 250,
      hideDuration: 250,
      timeOut: 5000,
      extendedTimeOut: 1000,
      tapToDismiss: true,
      escapeHtml: false,
      showMethod: 'fadeIn',
      hideMethod: 'fadeOut',
      onShown: function () {
        try {
          const container = document.getElementById('toast-container');
          if (container) {
            container.style.zIndex = '100000';
            container.style.pointerEvents = 'auto';
          }
        } catch (e) {}
      }
    };

    // Minor dark-mode tweaks (colors are handled via CSS in index.html)
    const container = document.getElementById('toast-container');
    if (container) {
      container.classList.toggle('dark', isDark);
    }
  }

  function buildErrorIcon() {
    return (
      '<span class="inline-flex items-center mr-2" aria-hidden="true" role="img" style="color: var(--text-primary);">' +
      '<svg xmlns="http://www.w3.org/2000/svg" width="22" height="22" viewBox="0 0 24 24" fill="currentColor">' +
      '<path d="M11.001 10h2v5h-2z"></path><path d="M11 16h2v2h-2z"></path>' +
      '<path d="M12 2C6.486 2 2 6.486 2 12s4.486 10 10 10 10-4.486 10-10S17.514 2 12 2zm0 18c-4.411 0-8-3.589-8-8s3.589-8 8-8 8 3.589 8 8-3.589 8-8 8z"></path>' +
      '</svg>' +
      '</span>'
    );
  }

  // Build title HTML with larger header text and icon for error types
  function buildTitleHtml(title, type) {
    if (type === 'error') {
      return (
        '<span class="inline-flex items-center">' +
        buildErrorIcon() +
        '<span class="font-semibold" style="color: var(--text-primary);">' + title + '</span>' +
        '</span>'
      );
    }
    return '<span class="font-semibold" style="color: var(--text-primary);">' + title + '</span>';
  }

  function buildAlertHtml(message, buttonText, type) {
    // Use theme text color for message; keep neutral content styling
    return (
      `<div class="mt-2" style="color: var(--text-primary);">${message}</div>` +
      `<div class="mt-3 flex gap-2 justify-end">` +
      `<button class="toastr-btn-ok px-3 py-2 rounded-md bg-blue-600 hover:bg-blue-700 text-white font-semibold">${buttonText}</button>` +
      `</div>`
    );
  }

  function buildConfirmHtml(message, confirmText, cancelText) {
    return (
      `<div class="mt-2 text-gray-800 dark:text-gray-100">${message}</div>` +
      `<div class="mt-3 flex gap-2 justify-end">` +
      `<button class="toastr-btn-confirm px-3 py-2 rounded-md bg-green-600 hover:bg-green-700 text-white font-semibold">${confirmText}</button>` +
      `<button class="toastr-btn-cancel px-3 py-2 rounded-md bg-red-600 hover:bg-red-700 text-white font-semibold">${cancelText}</button>` +
      `</div>`
    );
  }

  window.Notifications = {
    updateToastrTheme: function () {
      configureToastrOptions();
    },

    showToast: function (arg1, arg2, arg3) {
      if (typeof toastr === 'undefined') {
        console.error('Toastr is not available.');
        return;
      }
      const { message, type, duration } = parseToastArgs(arg1, arg2, arg3);
      const opts = Object.assign({}, toastr.options, {
        timeOut: duration,
        extendedTimeOut: Math.min(1000, Math.floor(duration / 5))
      });
      toastr.options = opts;
      let $toast;
      switch (type) {
        case 'success': $toast = toastr.success(message); break;
        case 'error': $toast = toastr.error(message); break;
        case 'warning': $toast = toastr.warning(message); break;
        default: $toast = toastr.info(message); break;
      }
      if ($toast && $toast.find) {
        applyThemeToToast($toast);
        try { $toast.css('min-width', '420px'); $toast.css('width', '420px'); } catch (e) {}
      }
    },

    showAlertToast: function (title = 'Alert', message = '', buttonText = 'OK', type = 'info') {
      return new Promise((resolve) => {
        try {
          if (typeof toastr === 'undefined') {
            console.error('Toastr is not available for alert toast.');
            resolve(false);
            return;
          }
          let resolved = false;
          const resolveOnce = (val) => { if (!resolved) { resolved = true; resolve(val); } };

          const html = buildAlertHtml(message, buttonText, type);
          const options = {
            timeOut: 0,
            extendedTimeOut: 0,
            tapToDismiss: false,
            closeButton: true,
            progressBar: true,
            escapeHtml: false,
            positionClass: 'toast-bottom-right',
            onCloseClick: function () { resolveOnce(false); },
            onHidden: function () { resolveOnce(false); }
          };

          // Always use info toast to avoid harsh error background; style title/icon red for error
          const titleHtml = buildTitleHtml(title, type);
          const $toast = toastr.info(html, titleHtml, options);
          if ($toast && $toast.find) {
            try { $toast.css('min-width', '420px'); $toast.css('width', '420px'); } catch (e) {}
            applyThemeToToast($toast);
            $toast.find('.toastr-btn-ok').on('click', () => {
              resolveOnce(true);
              $toast.remove();
            });
            $toast.find('.toast-close-button').on('click', () => {
              resolveOnce(false);
            });
          } else {
            resolveOnce(false);
          }
        } catch (e) {
          console.error('Error showing alert toast:', e);
          resolve(false);
        }
      });
    },

    showConfirmationToast: function (title = 'Confirm', message = '', confirmText = 'Yes', cancelText = 'No', type = 'info') {
      return new Promise((resolve) => {
        try {
          if (typeof toastr === 'undefined') {
            console.error('Toastr is not available for confirmation toast.');
            resolve(false);
            return;
          }
          const html = buildConfirmHtml(message, confirmText, cancelText);
          const titleHtml = buildTitleHtml(title, type);
          const $toast = toastr.info(html, titleHtml, {
            timeOut: 0,
            extendedTimeOut: 0,
            tapToDismiss: false,
            closeButton: true,
            progressBar: true,
            escapeHtml: false,
            positionClass: 'toast-bottom-right'
          });
          if ($toast && $toast.find) {
            try { $toast.css('min-width', '420px'); $toast.css('width', '420px'); } catch (e) {}
            applyThemeToToast($toast);
            $toast.find('.toastr-btn-confirm').on('click', () => {
              resolve(true);
              $toast.remove();
            });
            $toast.find('.toastr-btn-cancel').on('click', () => {
              resolve(false);
              $toast.remove();
            });
          } else {
            resolve(false);
          }
        } catch (e) {
          console.error('Error showing confirmation toast:', e);
          resolve(false);
        }
      });
    }
  };

  // Global wrappers to preserve existing interop identifiers
  window.updateToastrTheme = function () { if (window.Notifications) { window.Notifications.updateToastrTheme(); } };
  window.showToast = function (arg1, arg2, arg3) { if (window.Notifications) { window.Notifications.showToast(arg1, arg2, arg3); } };
  window.showAlertToast = function (title, message, buttonText, type) { if (window.Notifications) { return window.Notifications.showAlertToast(title, message, buttonText, type); } return Promise.resolve(false); };
  window.showConfirmationToast = function (title, message, confirmText, cancelText, type) { if (window.Notifications) { return window.Notifications.showConfirmationToast(title, message, confirmText, cancelText, type); } return Promise.resolve(false); };

  // Configure on load and whenever dark mode toggles externally
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', configureToastrOptions);
  } else {
    configureToastrOptions();
  }
})();


// Apply theme-blended styles to a toastr jQuery element
function applyThemeToToast($toast) {
  try {
    if (!$toast || !$toast[0]) return;
    const el = $toast[0];
    if (el && el.style && el.style.setProperty) {
      el.style.setProperty('background-image', 'none', 'important');
      // Use alternate theme background (fallback to financial-ui surface bg)
      el.style.setProperty('background-color', 'var(--secondary-bg, var(--color-surface-bg))', 'important');
      el.style.setProperty('color', 'var(--text-primary)', 'important');
      el.style.setProperty('opacity', '1', 'important');
      el.style.setProperty('border', '5px solid var(--border-color)', 'important');
      el.style.setProperty('border-radius', '10px', 'important');
      el.style.setProperty('padding', '15px 16px', 'important');
      el.style.setProperty('box-shadow', '0 10px 25px -3px var(--shadow-color), 0 4px 6px -2px var(--shadow-color)');
      // Heavy entry animations on load
      el.style.setProperty('animation', 'fadeInUp 0.9s ease-out, pulse 1.8s 2');
    }
    $toast.find('.toast-close-button').css('color', 'var(--text-secondary)');
  } catch (e) { /* no-op */ }
}