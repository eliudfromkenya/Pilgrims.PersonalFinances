// Financial UI Animation Functions

// Initialize counter animations
window.initializeCounters = function() {
    const counters = document.querySelectorAll('.counter[data-target]');
    
    counters.forEach(counter => {
        const target = parseInt(counter.getAttribute('data-target'));
        const duration = 2000; // 2 seconds
        const increment = target / (duration / 16); // 60fps
        let current = 0;
        
        const updateCounter = () => {
            if (current < target) {
                current += increment;
                if (current > target) current = target;
                
                // Format number with commas for large numbers
                const formatted = Math.floor(current).toLocaleString();
                
                // Check if it's a percentage
                if (counter.textContent.includes('%')) {
                    counter.textContent = Math.floor(current) + '%';
                } else {
                    counter.textContent = '$' + formatted;
                }
                
                requestAnimationFrame(updateCounter);
            }
        };
        
        // Start animation with a slight delay for staggered effect
        setTimeout(() => {
            updateCounter();
        }, 500);
    });
};

// Initialize chart animations
window.initializeCharts = function() {
    const chartBars = document.querySelectorAll('.chart-bar');
    
    chartBars.forEach((bar, index) => {
        // Reset height to 0 for animation
        bar.style.height = '0%';
        
        // Animate to target height with staggered delay
        setTimeout(() => {
            bar.style.transition = 'height 0.8s ease-out';
            bar.style.height = bar.getAttribute('style').match(/height:\s*(\d+%)/)[1];
        }, index * 100);
    });
};

// Initialize progress bar animations
window.initializeProgressBars = function() {
    const progressBars = document.querySelectorAll('.progress-fill, .progress-fill-horizontal');
    
    progressBars.forEach((bar, index) => {
        const targetWidth = bar.style.width;
        bar.style.width = '0%';
        
        setTimeout(() => {
            bar.style.transition = 'width 1.5s ease-out';
            bar.style.width = targetWidth;
        }, 800 + (index * 200));
    });
};

// Particle Animation System
class ParticleSystem {
    constructor(canvasId) {
        this.canvas = document.getElementById(canvasId);
        if (!this.canvas) return;
        
        this.ctx = this.canvas.getContext('2d');
        this.particles = [];
        this.particleCount = 50;
        
        this.resize();
        this.init();
        this.animate();
        
        window.addEventListener('resize', () => this.resize());
    }
    
    resize() {
        this.canvas.width = window.innerWidth;
        this.canvas.height = window.innerHeight;
    }
    
    init() {
        this.particles = [];
        for (let i = 0; i < this.particleCount; i++) {
            this.particles.push({
                x: Math.random() * this.canvas.width,
                y: Math.random() * this.canvas.height,
                vx: (Math.random() - 0.5) * 0.5,
                vy: (Math.random() - 0.5) * 0.5,
                size: Math.random() * 2 + 1,
                opacity: Math.random() * 0.5 + 0.2
            });
        }
    }
    
    animate() {
        this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
        
        // Check if dark mode is active
        const isDark = document.documentElement.classList.contains('dark');
        const particleColor = isDark ? '255, 255, 255' : '59, 130, 246'; // white for dark, blue for light
        
        this.particles.forEach(particle => {
            particle.x += particle.vx;
            particle.y += particle.vy;
            
            // Wrap around edges
            if (particle.x < 0) particle.x = this.canvas.width;
            if (particle.x > this.canvas.width) particle.x = 0;
            if (particle.y < 0) particle.y = this.canvas.height;
            if (particle.y > this.canvas.height) particle.y = 0;
            
            // Draw particle
            this.ctx.beginPath();
            this.ctx.arc(particle.x, particle.y, particle.size, 0, Math.PI * 2);
            this.ctx.fillStyle = `rgba(${particleColor}, ${particle.opacity})`;
            this.ctx.fill();
        });
        
        requestAnimationFrame(() => this.animate());
    }
}

// Initialize particle system when DOM is loaded
window.initializeParticles = function() {
    new ParticleSystem('particle-canvas');
};

// Floating Action Button interactions
window.initializeFAB = function() {
    const fabButtons = document.querySelectorAll('.fab-secondary');
    
    fabButtons.forEach(button => {
        button.addEventListener('mouseenter', () => {
            button.style.transform = 'scale(1.1) rotate(5deg)';
        });
        
        button.addEventListener('mouseleave', () => {
            button.style.transform = 'scale(1) rotate(0deg)';
        });
    });
};

// Intersection Observer for scroll animations
window.initializeScrollAnimations = function() {
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate-in');
            }
        });
    }, observerOptions);
    
    // Observe all animated elements
    const animatedElements = document.querySelectorAll('.animate-slide-up, .animate-slide-down');
    animatedElements.forEach(el => observer.observe(el));
};

// Theme transition effects
window.initializeThemeTransitions = function() {
    const themeToggle = document.querySelector('[data-theme-toggle]');
    
    if (themeToggle) {
        themeToggle.addEventListener('click', () => {
            document.body.style.transition = 'background-color 0.3s ease, color 0.3s ease';
            
            // Add ripple effect
            const ripple = document.createElement('div');
            ripple.className = 'theme-ripple';
            ripple.style.cssText = `
                position: fixed;
                top: 50%;
                left: 50%;
                width: 0;
                height: 0;
                border-radius: 50%;
                background: radial-gradient(circle, rgba(59, 130, 246, 0.3) 0%, transparent 70%);
                transform: translate(-50%, -50%);
                pointer-events: none;
                z-index: 9999;
                animation: themeRipple 0.6s ease-out forwards;
            `;
            
            document.body.appendChild(ripple);
            
            setTimeout(() => {
                ripple.remove();
            }, 600);
        });
    }
};

// Initialize all animations when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    // Small delay to ensure all elements are rendered
    setTimeout(() => {
        initializeParticles();
        initializeScrollAnimations();
        initializeFAB();
        initializeThemeTransitions();
        initializeProgressBars();
    }, 100);
});

// CSS keyframes for theme ripple animation
const style = document.createElement('style');
style.textContent = `
    @keyframes themeRipple {
        0% {
            width: 0;
            height: 0;
            opacity: 1;
        }
        100% {
            width: 200vw;
            height: 200vw;
            opacity: 0;
        }
    }
    
    .animate-in {
        opacity: 1 !important;
        transform: translateY(0) !important;
    }
`;
document.head.appendChild(style);