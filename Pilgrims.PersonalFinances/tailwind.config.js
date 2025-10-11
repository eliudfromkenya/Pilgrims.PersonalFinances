/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Pages/**/*.{razor,html,js}",
    "./Shared/**/*.{razor,html,js}",
    "./Components/**/*.{razor,html,js}",
    "./wwwroot/**/*.{html,js}",
    "./**/*.razor"
  ],
  darkMode: 'class', // Enable dark mode with class strategy
  theme: {
    extend: {
      colors: {
        // Light Theme Colors
        light: {
          primary: '#E3F2FD', // bg-blue-50
          surface: '#FFFFFF',  // bg-white
          card: '#FFFFFF',     // bg-white
          'text-primary': '#1A237E', // text-blue-900
          'text-secondary': '#424242', // text-gray-600
          accent: '#2196F3',   // bg-blue-500
          success: '#4CAF50',  // bg-green-500
          warning: '#FF9800',  // bg-orange-500
          error: '#F44336',    // bg-red-500
        },
        // Dark Theme Colors
        dark: {
          primary: '#0D1421',  // bg-slate-900
          surface: '#1E2A3A',  // bg-slate-800
          card: '#1E2A3A',     // bg-slate-800
          'text-primary': '#E3F2FD', // text-blue-100
          'text-secondary': '#D1D5DB', // text-gray-300
          accent: '#60A5FA',   // bg-blue-400
          success: '#4ADE80',  // bg-green-400
          warning: '#FB923C',  // bg-orange-400
          error: '#F87171',    // bg-red-400
        },
        // Enhanced Primary Color Palette
        primary: {
          50: '#E3F2FD',
          100: '#BBDEFB',
          200: '#90CAF9',
          300: '#64B5F6',
          400: '#42A5F5',
          500: '#2196F3',
          600: '#1E88E5',
          700: '#1976D2',
          800: '#1565C0',
          900: '#1A237E',
        },
        // Financial App Specific Colors
        finance: {
          income: '#4CAF50',
          expense: '#F44336',
          transfer: '#2196F3',
          investment: '#9C27B0',
          savings: '#4CAF50',
          debt: '#FF5722',
          budget: '#FF9800',
          goal: '#3F51B5',
        },
        // Status Colors
        status: {
          pending: '#FF9800',
          cleared: '#4CAF50',
          reconciled: '#2196F3',
          overdue: '#F44336',
          upcoming: '#FF9800',
        }
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
        mono: ['JetBrains Mono', 'Consolas', 'monospace'],
      },
      fontSize: {
        'xs': ['0.75rem', { lineHeight: '1rem' }],
        'sm': ['0.875rem', { lineHeight: '1.25rem' }],
        'base': ['1rem', { lineHeight: '1.5rem' }],
        'lg': ['1.125rem', { lineHeight: '1.75rem' }],
        'xl': ['1.25rem', { lineHeight: '1.75rem' }],
        '2xl': ['1.5rem', { lineHeight: '2rem' }],
        '3xl': ['1.875rem', { lineHeight: '2.25rem' }],
        '4xl': ['2.25rem', { lineHeight: '2.5rem' }],
        '5xl': ['3rem', { lineHeight: '1' }],
        '6xl': ['3.75rem', { lineHeight: '1' }],
      },
      spacing: {
        '18': '4.5rem',
        '88': '22rem',
        '128': '32rem',
      },
      borderRadius: {
        'xl': '0.75rem',
        '2xl': '1rem',
        '3xl': '1.5rem',
      },
      boxShadow: {
        'soft': '0 2px 15px -3px rgba(0, 0, 0, 0.07), 0 10px 20px -2px rgba(0, 0, 0, 0.04)',
        'card': '0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06)',
        'card-hover': '0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)',
        'financial': '0 4px 14px 0 rgba(33, 150, 243, 0.15)',
        'success': '0 4px 14px 0 rgba(76, 175, 80, 0.15)',
        'warning': '0 4px 14px 0 rgba(255, 152, 0, 0.15)',
        'error': '0 4px 14px 0 rgba(244, 67, 54, 0.15)',
      },
      animation: {
        'fade-in': 'fadeIn 0.5s ease-in-out',
        'fade-in-up': 'fadeInUp 0.6s ease-out',
        'slide-up': 'slideUp 0.3s ease-out',
        'slide-down': 'slideDown 0.3s ease-out',
        'slide-left': 'slideLeft 0.3s ease-out',
        'slide-right': 'slideRight 0.3s ease-out',
        'scale-in': 'scaleIn 0.2s ease-out',
        'bounce-gentle': 'bounceGentle 0.6s ease-out',
        'pulse-slow': 'pulse 3s cubic-bezier(0.4, 0, 0.6, 1) infinite',
        'count-up': 'countUp 1.5s ease-out',
        'progress-fill': 'progressFill 1s ease-out',
        'shimmer': 'shimmer 2s linear infinite',
      },
      keyframes: {
        fadeIn: {
          '0%': { opacity: '0' },
          '100%': { opacity: '1' },
        },
        fadeInUp: {
          '0%': { opacity: '0', transform: 'translateY(20px)' },
          '100%': { opacity: '1', transform: 'translateY(0)' },
        },
        slideUp: {
          '0%': { transform: 'translateY(10px)', opacity: '0' },
          '100%': { transform: 'translateY(0)', opacity: '1' },
        },
        slideDown: {
          '0%': { transform: 'translateY(-10px)', opacity: '0' },
          '100%': { transform: 'translateY(0)', opacity: '1' },
        },
        slideLeft: {
          '0%': { transform: 'translateX(10px)', opacity: '0' },
          '100%': { transform: 'translateX(0)', opacity: '1' },
        },
        slideRight: {
          '0%': { transform: 'translateX(-10px)', opacity: '0' },
          '100%': { transform: 'translateX(0)', opacity: '1' },
        },
        scaleIn: {
          '0%': { transform: 'scale(0.95)', opacity: '0' },
          '100%': { transform: 'scale(1)', opacity: '1' },
        },
        bounceGentle: {
          '0%, 20%, 53%, 80%, 100%': { transform: 'translate3d(0,0,0)' },
          '40%, 43%': { transform: 'translate3d(0, -8px, 0)' },
          '70%': { transform: 'translate3d(0, -4px, 0)' },
          '90%': { transform: 'translate3d(0, -2px, 0)' },
        },
        countUp: {
          '0%': { transform: 'translateY(20px)', opacity: '0' },
          '100%': { transform: 'translateY(0)', opacity: '1' },
        },
        progressFill: {
          '0%': { width: '0%' },
          '100%': { width: 'var(--progress-width)' },
        },
        shimmer: {
          '0%': { transform: 'translateX(-100%)' },
          '100%': { transform: 'translateX(100%)' },
        },
      },
      backdropBlur: {
        xs: '2px',
      },
      transitionDuration: {
        '400': '400ms',
        '600': '600ms',
      },
    },
  },
  plugins: [
    // Custom plugin for financial app utilities
    function({ addUtilities, theme }) {
      const newUtilities = {
        '.card-financial': {
          '@apply bg-white dark:bg-slate-800 rounded-xl shadow-card hover:shadow-card-hover transition-all duration-300 border border-gray-100 dark:border-slate-700': {},
        },
        '.btn-primary': {
          '@apply bg-primary-500 hover:bg-primary-600 text-white font-medium py-2 px-4 rounded-lg transition-colors duration-200 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2': {},
        },
        '.btn-secondary': {
          '@apply bg-gray-100 hover:bg-gray-200 dark:bg-slate-700 dark:hover:bg-slate-600 text-gray-700 dark:text-gray-200 font-medium py-2 px-4 rounded-lg transition-colors duration-200': {},
        },
        '.text-financial-positive': {
          '@apply text-finance-income font-semibold': {},
        },
        '.text-financial-negative': {
          '@apply text-finance-expense font-semibold': {},
        },
        '.bg-financial-positive': {
          '@apply bg-finance-income text-white': {},
        },
        '.bg-financial-negative': {
          '@apply bg-finance-expense text-white': {},
        },
        '.progress-ring': {
          'transform': 'rotate(-90deg)',
          'transform-origin': '50% 50%',
        },
      }
      addUtilities(newUtilities)
    }
  ],
}