/**
 * IZIDEAL - Modern JavaScript Utilities
 * Pure Vanilla JavaScript - No frameworks
 */

// =============================================
// DOM Ready Helper
// =============================================
const ready = (fn) => {
    if (document.readyState !== 'loading') {
        fn();
    } else {
        document.addEventListener('DOMContentLoaded', fn);
    }
};

// =============================================
// Toast Notification System
// =============================================
const Toast = {
    container: null,

    init() {
        if (!this.container) {
            this.container = document.createElement('div');
            this.container.className = 'toast-container';
            document.body.appendChild(this.container);
        }
    },

    show(message, type = 'info', duration = 4000) {
        this.init();

        const toast = document.createElement('div');
        toast.className = `toast-custom toast-${type}`;
        toast.innerHTML = `
            <div class="d-flex align-items-center gap-2">
                <span class="toast-icon">${this.getIcon(type)}</span>
                <span class="toast-message">${message}</span>
                <button class="btn-close btn-close-white ms-2" onclick="this.parentElement.parentElement.remove()"></button>
            </div>
        `;

        this.container.appendChild(toast);

        setTimeout(() => {
            toast.style.opacity = '0';
            toast.style.transform = 'translateX(100%)';
            setTimeout(() => toast.remove(), 300);
        }, duration);
    },

    getIcon(type) {
        const icons = {
            success: '✓',
            error: '✕',
            warning: '⚠',
            info: 'ℹ'
        };
        return icons[type] || icons.info;
    },

    success(message) { this.show(message, 'success'); },
    error(message) { this.show(message, 'error'); },
    warning(message) { this.show(message, 'warning'); },
    info(message) { this.show(message, 'info'); }
};

// =============================================
// Modal System
// =============================================
const Modal = {
    open(modalId) {
        const modal = document.getElementById(modalId);
        if (modal) {
            modal.classList.add('open');
            document.body.style.overflow = 'hidden';
        }
    },

    close(modalId) {
        const modal = document.getElementById(modalId);
        if (modal) {
            modal.classList.remove('open');
            document.body.style.overflow = '';
        }
    },

    confirm(title, message, onConfirm) {
        const modalHtml = `
            <div class="modal-backdrop" id="confirm-modal">
                <div class="modal">
                    <div class="modal-header">
                        <h5 class="modal-title">${title}</h5>
                        <button class="modal-close" onclick="Modal.close('confirm-modal')">&times;</button>
                    </div>
                    <div class="modal-body">
                        <p>${message}</p>
                    </div>
                    <div class="modal-footer">
                        <button class="btn btn-secondary" onclick="Modal.close('confirm-modal')">Annuler</button>
                        <button class="btn btn-danger" id="confirm-btn">Confirmer</button>
                    </div>
                </div>
            </div>
        `;

        document.body.insertAdjacentHTML('beforeend', modalHtml);
        const modal = document.getElementById('confirm-modal');
        modal.classList.add('open');
        document.body.style.overflow = 'hidden';

        document.getElementById('confirm-btn').onclick = () => {
            onConfirm();
            modal.remove();
            document.body.style.overflow = '';
        };

        modal.addEventListener('click', (e) => {
            if (e.target === modal) {
                modal.remove();
                document.body.style.overflow = '';
            }
        });
    }
};

// =============================================
// Mobile Navigation
// =============================================
const MobileNav = {
    init() {
        const toggleBtn = document.querySelector('.mobile-menu-btn');
        const mobileNav = document.querySelector('.mobile-nav');

        if (toggleBtn && mobileNav) {
            toggleBtn.addEventListener('click', () => {
                toggleBtn.classList.toggle('active');
                mobileNav.classList.toggle('open');
            });

            // Close on outside click
            document.addEventListener('click', (e) => {
                if (!toggleBtn.contains(e.target) && !mobileNav.contains(e.target)) {
                    toggleBtn.classList.remove('active');
                    mobileNav.classList.remove('open');
                }
            });
        }
    }
};

// =============================================
// Image Gallery
// =============================================
const Gallery = {
    currentIndex: 0,
    images: [],
    autoTimer: null,
    interval: 4500,
    progressBar: null,

    init(containerId) {
        const container = document.getElementById(containerId);
        if (!container) return;

        this.images = Array.from(container.querySelectorAll('.gallery-thumb img')).map(img => img.src);
        const mainImg = container.querySelector('.gallery-main img');
        const thumbs = container.querySelectorAll('.gallery-thumb');
        this.progressBar = container.querySelector('.gallery-progress-bar');

        if (this.progressBar) {
            this.progressBar.style.transition = `width ${this.interval}ms linear`;
        }

        thumbs.forEach((thumb, index) => {
            thumb.addEventListener('click', () => {
                this.currentIndex = index;
                this.updateMainImage(mainImg, thumbs);
                this.restartAutoPlay(container, mainImg, thumbs);
            });
        });

        // Navigation buttons
        const prevBtn = container.querySelector('.gallery-nav.prev');
        const nextBtn = container.querySelector('.gallery-nav.next');

        if (prevBtn) {
            prevBtn.addEventListener('click', () => {
                this.currentIndex = (this.currentIndex - 1 + this.images.length) % this.images.length;
                this.updateMainImage(mainImg, thumbs);
                this.restartAutoPlay(container, mainImg, thumbs);
            });
        }

        if (nextBtn) {
            nextBtn.addEventListener('click', () => {
                this.currentIndex = (this.currentIndex + 1) % this.images.length;
                this.updateMainImage(mainImg, thumbs);
                this.restartAutoPlay(container, mainImg, thumbs);
            });
        }

        // Keyboard navigation
        document.addEventListener('keydown', (e) => {
            if (e.key === 'ArrowLeft') {
                this.currentIndex = (this.currentIndex - 1 + this.images.length) % this.images.length;
                this.updateMainImage(mainImg, thumbs);
                this.restartAutoPlay(container, mainImg, thumbs);
            } else if (e.key === 'ArrowRight') {
                this.currentIndex = (this.currentIndex + 1) % this.images.length;
                this.updateMainImage(mainImg, thumbs);
                this.restartAutoPlay(container, mainImg, thumbs);
            }
        });

        // Auto-play with pause on hover
        if (this.images.length > 1) {
            this.startAutoPlay(container, mainImg, thumbs);

            container.addEventListener('mouseenter', () => this.stopAutoPlay());
            container.addEventListener('mouseleave', () => this.startAutoPlay(container, mainImg, thumbs));
        }
    },

    updateMainImage(mainImg, thumbs) {
        const nextSrc = this.images[this.currentIndex];

        if (mainImg && nextSrc && mainImg.dataset.current !== nextSrc) {
            const temp = new Image();
            temp.src = nextSrc;

            temp.onload = () => {
                mainImg.classList.add('is-fading');

                setTimeout(() => {
                    mainImg.src = nextSrc;
                    mainImg.dataset.current = nextSrc;

                    // Allow browser to paint before fading back in
                    requestAnimationFrame(() => {
                        requestAnimationFrame(() => mainImg.classList.remove('is-fading'));
                    });
                }, 150);
            };
        }

        thumbs.forEach((thumb, i) => {
            thumb.classList.toggle('active', i === this.currentIndex);
        });
    },

    startAutoPlay(container, mainImg, thumbs) {
        this.stopAutoPlay();
        this.animateProgress();
        this.autoTimer = setInterval(() => {
            this.currentIndex = (this.currentIndex + 1) % this.images.length;
            this.updateMainImage(mainImg, thumbs);
            this.animateProgress();
        }, this.interval);
    },

    stopAutoPlay() {
        if (this.autoTimer) {
            clearInterval(this.autoTimer);
            this.autoTimer = null;
        }
        this.resetProgress();
    },

    restartAutoPlay(container, mainImg, thumbs) {
        this.stopAutoPlay();
        this.startAutoPlay(container, mainImg, thumbs);
    },

    resetProgress() {
        if (!this.progressBar) return;
        this.progressBar.style.transition = 'none';
        this.progressBar.style.width = '0%';
    },

    animateProgress() {
        if (!this.progressBar) return;
        this.resetProgress();
        // Allow the browser to register the reset before animating
        requestAnimationFrame(() => {
            this.progressBar.style.transition = `width ${this.interval}ms linear`;
            this.progressBar.style.width = '100%';
        });
    }
};

// =============================================
// File Upload with Preview
// =============================================
const FileUpload = {
    init(inputId, previewContainerId, maxFiles = 5, maxSizeMB = 5) {
        const input = document.getElementById(inputId);
        const preview = document.getElementById(previewContainerId);

        if (!input || !preview) return;

        input.addEventListener('change', (e) => {
            preview.innerHTML = '';
            const files = Array.from(e.target.files).slice(0, maxFiles);
            const maxSize = maxSizeMB * 1024 * 1024;

            files.forEach((file, index) => {
                if (!file.type.startsWith('image/')) {
                    Toast.error(`${file.name} n'est pas une image valide`);
                    return;
                }

                if (file.size > maxSize) {
                    Toast.error(`${file.name} dépasse ${maxSizeMB}MB`);
                    return;
                }

                const reader = new FileReader();
                reader.onload = (e) => {
                    const item = document.createElement('div');
                    item.className = 'image-preview-item';
                    item.innerHTML = `
                        <img src="${e.target.result}" alt="Preview ${index + 1}">
                        <button type="button" class="image-preview-remove" data-index="${index}">&times;</button>
                    `;
                    preview.appendChild(item);
                };
                reader.readAsDataURL(file);
            });
        });

        // Handle remove buttons
        preview.addEventListener('click', (e) => {
            if (e.target.classList.contains('image-preview-remove')) {
                e.target.parentElement.remove();
                // Note: Can't actually remove files from FileList, would need DataTransfer API
            }
        });
    }
};

// =============================================
// Form Validation
// =============================================
const FormValidator = {
    init(formId) {
        const form = document.getElementById(formId);
        if (!form) return;

        form.addEventListener('submit', (e) => {
            let isValid = true;
            const requiredFields = form.querySelectorAll('[required]');

            requiredFields.forEach(field => {
                this.clearError(field);

                if (!field.value.trim()) {
                    this.showError(field, 'Ce champ est requis');
                    isValid = false;
                } else if (field.type === 'email' && !this.isValidEmail(field.value)) {
                    this.showError(field, 'Email invalide');
                    isValid = false;
                }
            });

            // Password confirmation
            const password = form.querySelector('[name="Password"]');
            const confirmPassword = form.querySelector('[name="ConfirmPassword"]');
            if (password && confirmPassword && password.value !== confirmPassword.value) {
                this.showError(confirmPassword, 'Les mots de passe ne correspondent pas');
                isValid = false;
            }

            if (!isValid) {
                e.preventDefault();
            }
        });

        // Real-time validation
        form.querySelectorAll('.form-control').forEach(field => {
            field.addEventListener('blur', () => {
                this.clearError(field);
                if (field.required && !field.value.trim()) {
                    this.showError(field, 'Ce champ est requis');
                }
            });

            field.addEventListener('input', () => {
                this.clearError(field);
            });
        });
    },

    showError(field, message) {
        field.classList.add('is-invalid');
        const error = document.createElement('div');
        error.className = 'form-error text-danger';
        error.textContent = message;
        field.parentElement.appendChild(error);
    },

    clearError(field) {
        field.classList.remove('is-invalid');
        const error = field.parentElement.querySelector('.form-error');
        if (error) error.remove();
    },

    isValidEmail(email) {
        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
    }
};

// =============================================
// Search & Filter
// =============================================
const Search = {
    debounceTimer: null,

    init(inputId, resultContainerId, searchFn) {
        const input = document.getElementById(inputId);
        if (!input) return;

        input.addEventListener('input', (e) => {
            clearTimeout(this.debounceTimer);
            this.debounceTimer = setTimeout(() => {
                searchFn(e.target.value);
            }, 300);
        });
    },

    highlight(text, query) {
        if (!query) return text;
        const regex = new RegExp(`(${query})`, 'gi');
        return text.replace(regex, '<mark>$1</mark>');
    }
};

// =============================================
// Lazy Loading Images
// =============================================
const LazyLoad = {
    init() {
        const images = document.querySelectorAll('img[data-src]');

        if ('IntersectionObserver' in window) {
            const observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        const img = entry.target;
                        img.src = img.dataset.src;
                        img.removeAttribute('data-src');
                        observer.unobserve(img);
                    }
                });
            }, { rootMargin: '50px' });

            images.forEach(img => observer.observe(img));
        } else {
            // Fallback for older browsers
            images.forEach(img => {
                img.src = img.dataset.src;
            });
        }
    }
};

// =============================================
// Smooth Scroll
// =============================================
const SmoothScroll = {
    init() {
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', (e) => {
                const target = document.querySelector(anchor.getAttribute('href'));
                if (target) {
                    e.preventDefault();
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });
    }
};

// =============================================
// Loading State
// =============================================
const Loading = {
    show(buttonId) {
        const btn = document.getElementById(buttonId);
        if (!btn) return;

        btn.disabled = true;
        btn.dataset.originalText = btn.innerHTML;
        btn.innerHTML = '<span class="spinner"></span> Chargement...';
    },

    hide(buttonId) {
        const btn = document.getElementById(buttonId);
        if (!btn) return;

        btn.disabled = false;
        btn.innerHTML = btn.dataset.originalText;
    }
};

// =============================================
// Character Counter
// =============================================
const CharCounter = {
    init(inputId, maxLength) {
        const input = document.getElementById(inputId);
        if (!input) return;

        const counter = document.createElement('small');
        counter.className = 'text-muted d-block mt-1';
        input.parentElement.appendChild(counter);

        const update = () => {
            const remaining = maxLength - input.value.length;
            counter.textContent = `${input.value.length}/${maxLength} caractères`;
            counter.classList.toggle('text-danger', remaining < 20);
        };

        input.addEventListener('input', update);
        update();
    }
};

// =============================================
// Price Formatter
// =============================================
const PriceFormatter = {
    format(value, currency = 'MAD') {
        return new Intl.NumberFormat('fr-MA', {
            style: 'currency',
            currency: currency
        }).format(value);
    },

    init(inputId) {
        const input = document.getElementById(inputId);
        if (!input) return;

        input.addEventListener('blur', () => {
            const value = parseFloat(input.value.replace(/[^\d.-]/g, ''));
            if (!isNaN(value)) {
                input.value = value.toFixed(2);
            }
        });
    }
};

// =============================================
// Copy to Clipboard
// =============================================
const Clipboard = {
    copy(text) {
        navigator.clipboard.writeText(text).then(() => {
            Toast.success('Copié dans le presse-papiers !');
        }).catch(() => {
            Toast.error('Erreur lors de la copie');
        });
    }
};

// =============================================
// Date Formatter
// =============================================
const DateFormatter = {
    relative(date) {
        const now = new Date();
        const diff = now - new Date(date);
        const seconds = Math.floor(diff / 1000);
        const minutes = Math.floor(seconds / 60);
        const hours = Math.floor(minutes / 60);
        const days = Math.floor(hours / 24);

        if (days > 30) {
            return new Date(date).toLocaleDateString('fr-FR');
        } else if (days > 0) {
            return `Il y a ${days} jour${days > 1 ? 's' : ''}`;
        } else if (hours > 0) {
            return `Il y a ${hours} heure${hours > 1 ? 's' : ''}`;
        } else if (minutes > 0) {
            return `Il y a ${minutes} minute${minutes > 1 ? 's' : ''}`;
        } else {
            return 'À l\'instant';
        }
    }
};

// =============================================
// Animations on Scroll
// =============================================
const ScrollAnimations = {
    init() {
        const elements = document.querySelectorAll('.animate-on-scroll');

        if ('IntersectionObserver' in window) {
            const observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        entry.target.classList.add('animated');
                        observer.unobserve(entry.target);
                    }
                });
            }, { threshold: 0.1 });

            elements.forEach(el => observer.observe(el));
        } else {
            elements.forEach(el => el.classList.add('animated'));
        }
    }
};

// =============================================
// Delete Confirmation
// =============================================
const DeleteConfirm = {
    init() {
        document.querySelectorAll('[data-confirm-delete]').forEach(btn => {
            btn.addEventListener('click', (e) => {
                e.preventDefault();
                const form = btn.closest('form');
                const message = btn.dataset.confirmDelete || 'Êtes-vous sûr de vouloir supprimer cet élément ?';

                Modal.confirm('Confirmation', message, () => {
                    form.submit();
                });
            });
        });
    }
};

// =============================================
// Initialize All on DOM Ready
// =============================================
ready(() => {
    MobileNav.init();
    LazyLoad.init();
    SmoothScroll.init();
    ScrollAnimations.init();
    DeleteConfirm.init();

    // Initialize gallery if present
    if (document.getElementById('gallery')) {
        Gallery.init('gallery');
    }

    // Initialize file upload if present
    if (document.getElementById('photos-input')) {
        FileUpload.init('photos-input', 'image-preview', 5, 5);
    }

    console.log('🚀 IZIDEAL JS Initialized');
});

// Export for global use
window.Toast = Toast;
window.Modal = Modal;
window.Gallery = Gallery;
window.FileUpload = FileUpload;
window.FormValidator = FormValidator;
window.Loading = Loading;
window.Clipboard = Clipboard;
window.DateFormatter = DateFormatter;
