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
      newestOnTop: true,
      positionClass: 'toast-top-right',
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

  function buildAlertHtml(message, buttonText) {
    return (
      `<div class="mt-2 text-gray-800 dark:text-gray-100">${message}</div>` +
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
      switch (type) {
        case 'success': toastr.success(message); break;
        case 'error': toastr.error(message); break;
        case 'warning': toastr.warning(message); break;
        default: toastr.info(message); break;
      }
    },

    showAlertToast: function (title = 'Alert', message = '', buttonText = 'OK') {
      return new Promise((resolve) => {
        try {
          if (typeof toastr === 'undefined') {
            console.error('Toastr is not available for alert toast.');
            resolve(false);
            return;
          }
          const html = buildAlertHtml(message, buttonText);
          const $toast = toastr.info(html, title, {
            timeOut: 0,
            extendedTimeOut: 0,
            tapToDismiss: false,
            closeButton: true,
            progressBar: true,
            escapeHtml: false,
            positionClass: 'toast-top-center'
          });
          if ($toast && $toast.find) {
            $toast.find('.toastr-btn-ok').on('click', () => {
              resolve(true);
              $toast.remove();
            });
          } else {
            resolve(false);
          }
        } catch (e) {
          console.error('Error showing alert toast:', e);
          resolve(false);
        }
      });
    },

    showConfirmationToast: function (title = 'Confirm', message = '', confirmText = 'Yes', cancelText = 'No') {
      return new Promise((resolve) => {
        try {
          if (typeof toastr === 'undefined') {
            console.error('Toastr is not available for confirmation toast.');
            resolve(false);
            return;
          }
          const html = buildConfirmHtml(message, confirmText, cancelText);
          const $toast = toastr.info(html, title, {
            timeOut: 0,
            extendedTimeOut: 0,
            tapToDismiss: false,
            closeButton: true,
            progressBar: true,
            escapeHtml: false,
            positionClass: 'toast-top-center'
          });
          if ($toast && $toast.find) {
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
  window.showAlertToast = function (title, message, buttonText) { if (window.Notifications) { return window.Notifications.showAlertToast(title, message, buttonText); } return Promise.resolve(false); };
  window.showConfirmationToast = function (title, message, confirmText, cancelText) { if (window.Notifications) { return window.Notifications.showConfirmationToast(title, message, confirmText, cancelText); } return Promise.resolve(false); };

  // Configure on load and whenever dark mode toggles externally
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', configureToastrOptions);
  } else {
    configureToastrOptions();
  }
})();