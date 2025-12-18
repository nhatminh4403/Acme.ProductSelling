// Location: wwwroot/js/components/price-filter.js
/**
 * Price Filter Component
 * Reusable price filter with slider for product filtering
 * 
 * Usage:
 * const filter = new PriceFilter({
 *     minBound: 0,
 *     maxBound: 10000000,
 *     categorySlug: 'laptop',
 *     pageSize: 12,
 *     onFilter: (minPrice, maxPrice, page) => { ... }
 * });
 */

class PriceFilter {
    constructor(config) {
        // Configuration
        this.minBound = config.minBound || 0;
        this.maxBound = config.maxBound || 100000000;
        this.categorySlug = config.categorySlug || '';
        this.manufacturerSlug = config.manufacturerSlug || '';
        this.searchKeyword = config.searchKeyword || '';
        this.pageSize = config.pageSize || 12;
        this.showManufacturerFilter = config.showManufacturerFilter || false;
        this.apiEndpoint = config.apiEndpoint || '/api/products/filter-by-price';
        this.onFilter = config.onFilter; // Custom filter callback (optional)

        // State
        this.priceSlider = null;
        this.currentPage = 1;
        this.sliderTouched = false;
        this.isFiltered = false;
        this.selectedManufacturerId = null;

        // DOM Elements
        this.dropdown = document.getElementById('priceDropdown');
        this.toggleBtn = document.getElementById('priceFilterToggle');
        this.backdrop = document.getElementById('dropdownBackdrop');
        this.resetBtn = document.getElementById('resetFilter');
        this.applyBtn = document.getElementById('applyFilter');
        this.activeFilters = document.getElementById('activeFilters');
        this.clearFilterBtn = document.getElementById('clearPriceFilter');

        // Validate required elements
        if (!this.dropdown || !this.toggleBtn) {
            console.error('Price filter elements not found');
            return;
        }

        // Initialize
        this.init();
    }

    init() {
        console.log('Price Filter Configuration:', {
            bounds: [this.minBound, this.maxBound],
            categorySlug: this.categorySlug,
            manufacturerSlug: this.manufacturerSlug,
            searchKeyword: this.searchKeyword
        });

        this.initPriceSlider();
        this.attachEventListeners();

        // Initialize manufacturer filter if enabled
        if (this.showManufacturerFilter) {
            this.initManufacturerFilter();
        }
    }

    initPriceSlider() {
        const sliderElement = document.getElementById('priceSlider');

        if (!sliderElement) {
            console.error('Slider element not found');
            return;
        }

        this.priceSlider = noUiSlider.create(sliderElement, {
            start: [this.minBound, this.maxBound],
            connect: true,
            range: {
                'min': this.minBound,
                'max': this.maxBound
            },
            step: this.getStep(this.maxBound - this.minBound),
            format: {
                to: (value) => Math.round(value),
                from: (value) => Number(value)
            }
        });

        // Update input fields
        this.priceSlider.on('update', (values) => {
            const minInput = document.getElementById('minPriceInput');
            const maxInput = document.getElementById('maxPriceInput');

            if (minInput) minInput.value = this.formatPriceNumber(values[0]);
            if (maxInput) maxInput.value = this.formatPriceNumber(values[1]);

            // Enable button if slider was touched by user
            if (this.sliderTouched && this.applyBtn) {
                this.applyBtn.disabled = false;
            }
        });

        // Track when user starts dragging
        this.priceSlider.on('start', () => {
            this.sliderTouched = true;
        });

        console.log('Price slider initialized successfully');
    }

    initManufacturerFilter() {
        const checkboxes = document.querySelectorAll('.manufacturer-checkbox');
        checkboxes.forEach(checkbox => {
            checkbox.addEventListener('change', (e) => {
                this.selectedManufacturerId = e.target.value || null;
                this.sliderTouched = true; // Enable apply button
                if (this.applyBtn) {
                    this.applyBtn.disabled = false;
                }
            });
        });
    }

    attachEventListeners() {
        // Toggle dropdown
        this.toggleBtn?.addEventListener('click', (e) => {
            e.stopPropagation();
            const isOpen = this.dropdown.classList.contains('show');
            isOpen ? this.closeDropdown() : this.openDropdown();
        });

        // Close dropdown when clicking backdrop
        this.backdrop?.addEventListener('click', () => this.closeDropdown());

        // Reset button
        this.resetBtn?.addEventListener('click', () => {
            this.sliderTouched = false;
            if (this.applyBtn) this.applyBtn.disabled = true;
            if (this.priceSlider) {
                this.priceSlider.set([this.minBound, this.maxBound]);
            }
            // Reset manufacturer filter
            if (this.showManufacturerFilter) {
                const allCheckbox = document.getElementById('manufacturer-all');
                if (allCheckbox) allCheckbox.checked = true;
                this.selectedManufacturerId = null;
            }
        });

        // Apply filter
        this.applyBtn?.addEventListener('click', () => {
            if (!this.sliderTouched) return;

            const values = this.priceSlider.get();
            this.loadProducts(values[0], values[1], 1);
            this.sliderTouched = false;
            this.closeDropdown();
        });

        // Clear filter
        this.clearFilterBtn?.addEventListener('click', () => {
            this.isFiltered = false;
            this.sliderTouched = false;
            this.selectedManufacturerId = null;
            this.activeFilters?.classList.remove('show');
            this.toggleBtn?.classList.remove('active');
            if (this.priceSlider) {
                this.priceSlider.set([this.minBound, this.maxBound]);
            }
            this.loadProducts(this.minBound, this.maxBound, 1);
            this.closeDropdown();
        });
    }

    openDropdown() {
        this.dropdown.classList.add('show');
        this.backdrop.classList.add('show');
        if (!this.isFiltered) {
            this.sliderTouched = false;
            if (this.applyBtn) this.applyBtn.disabled = true;
            if (this.priceSlider) {
                this.priceSlider.set([this.minBound, this.maxBound]);
            }
        }
    }

    closeDropdown() {
        this.dropdown.classList.remove('show');
        this.backdrop.classList.remove('show');
        if (!this.isFiltered) {
            this.sliderTouched = false;
            if (this.applyBtn) this.applyBtn.disabled = true;
            if (this.priceSlider) {
                this.priceSlider.set([this.minBound, this.maxBound]);
            }
        }
    }

    loadProducts(minPrice, maxPrice, page = 1) {
        // If custom callback provided, use it
        if (this.onFilter) {
            this.onFilter(minPrice, maxPrice, page, this.selectedManufacturerId);
            this.updateActiveFilters(minPrice, maxPrice);
            return;
        }

        // Default AJAX implementation
        const overlay = document.querySelector('.loading-overlay');
        if (overlay) overlay.classList.add('active');

        // Build URL with parameters
        let url = `${this.apiEndpoint}?minPrice=${minPrice}&maxPrice=${maxPrice}&page=${page}&pageSize=${this.pageSize}`;

        if (this.categorySlug) url += `&categorySlug=${this.categorySlug}`;
        if (this.manufacturerSlug) url += `&manufacturerSlug=${this.manufacturerSlug}`;
        if (this.searchKeyword) url += `&searchKeyword=${encodeURIComponent(this.searchKeyword)}`;
        if (this.selectedManufacturerId) url += `&manufacturerId=${this.selectedManufacturerId}`;

        fetch(url)
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    this.updateProductsGrid(data.data);
                    this.updateActiveFilters(minPrice, maxPrice);
                    this.updatePagination(data.totalPages, data.currentPage);
                    this.currentPage = data.currentPage;
                } else {
                    this.showError(data.error || 'Failed to load products');
                }
            })
            .catch(error => {
                console.error('Error loading products:', error);
                this.showError('An error occurred while loading products');
            })
            .finally(() => {
                if (overlay) overlay.classList.remove('active');
            });
    }

    updateActiveFilters(minPrice, maxPrice) {
        const isDefault = Math.round(minPrice) === this.minBound &&
            Math.round(maxPrice) === this.maxBound &&
            !this.selectedManufacturerId;

        if (isDefault) {
            this.isFiltered = false;
            this.activeFilters?.classList.remove('show');
            this.toggleBtn?.classList.remove('active');
        } else {
            this.isFiltered = true;
            const minFormatted = this.formatPriceNumber(minPrice);
            const maxFormatted = this.formatPriceNumber(maxPrice);
            const rangeText = `${minFormatted} - ${maxFormatted}`;

            const rangeSpan = document.getElementById('currentPriceRange');
            if (rangeSpan) rangeSpan.textContent = rangeText;

            this.activeFilters?.classList.add('show');
            this.toggleBtn?.classList.add('active');
        }
    }

    updateProductsGrid(products) {
        const container = document.getElementById('productsContainer');
        if (!container) return;

        if (products.length === 0) {
            container.innerHTML = `
                <div class="alert alert-info d-flex align-items-center">
                    <i class="bi bi-info-circle me-2 fs-4"></i>
                    <div>
                        <h5 class="alert-heading mb-1">No Products Found</h5>
                        <p class="mb-0">Try adjusting your price range</p>
                    </div>
                </div>
            `;
            return;
        }

        let html = '<div class="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-lg-4 row-cols-xl-5 g-4">';
        products.forEach(product => {
            html += this.renderProductCard(product);
        });
        html += '</div>';

        container.innerHTML = html;
    }

    renderProductCard(product) {
        const price = product.discountedPrice || product.originalPrice;
        const formattedPrice = new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(price);

        const originalPriceFormatted = new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(product.originalPrice);

        return `
            <div class="col">
                <div class="card product-card h-100 shadow-sm">
                    <a href="/products/${product.urlSlug}">
                        <img src="https://placehold.co/200x200?text=${product.productName.charAt(0)}"
                             class="card-img-top p-3"
                             alt="${product.productName}"
                             style="max-height: 180px; object-fit: contain;">
                    </a>
                    <div class="card-body d-flex flex-column">
                        <h6 class="card-title mb-2" style="min-height: 40px;">
                            <a href="/products/${product.urlSlug}" class="text-decoration-none text-dark">
                                ${product.productName.length > 50 ? product.productName.substring(0, 47) + '...' : product.productName}
                            </a>
                        </h6>
                        <div class="mt-auto">
                            ${product.discountPercent > 0 && product.discountedPrice ? `
                                <div class="text-muted text-decoration-line-through small mb-1">
                                    ${originalPriceFormatted}
                                </div>
                                <div class="d-flex align-items-center gap-2">
                                    <span class="fw-bold text-danger fs-6">${formattedPrice}</span>
                                    <span class="badge bg-danger small">-${product.discountPercent}%</span>
                                </div>
                            ` : `
                                <div class="fw-bold text-danger fs-6">${formattedPrice}</div>
                            `}
                        </div>
                    </div>
                    <div class="card-footer bg-transparent border-0 text-center pb-2">
                        <a href="/products/${product.urlSlug}" class="btn btn-sm btn-primary w-100">
                            View Details
                        </a>
                    </div>
                </div>
            </div>
        `;
    }

    updatePagination(totalPages, currentPage) {
        const container = document.getElementById('paginationContainer');
        if (!container) return;

        if (totalPages <= 1) {
            container.innerHTML = '';
            return;
        }

        let html = '<nav><ul class="pagination justify-content-center">';

        html += `<li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
            <a class="page-link" href="#" data-page="${currentPage - 1}">Previous</a>
        </li>`;

        for (let i = 1; i <= totalPages; i++) {
            if (i === 1 || i === totalPages || (i >= currentPage - 2 && i <= currentPage + 2)) {
                html += `<li class="page-item ${i === currentPage ? 'active' : ''}">
                    <a class="page-link" href="#" data-page="${i}">${i}</a>
                </li>`;
            } else if (i === currentPage - 3 || i === currentPage + 3) {
                html += '<li class="page-item disabled"><span class="page-link">...</span></li>';
            }
        }

        html += `<li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
            <a class="page-link" href="#" data-page="${currentPage + 1}">Next</a>
        </li>`;

        html += '</ul></nav>';
        container.innerHTML = html;

        // Attach click handlers
        container.querySelectorAll('.page-link').forEach(link => {
            link.addEventListener('click', (e) => {
                e.preventDefault();
                if (!link.parentElement.classList.contains('disabled') &&
                    !link.parentElement.classList.contains('active')) {
                    const page = parseInt(link.dataset.page);
                    const values = this.priceSlider.get();
                    this.loadProducts(values[0], values[1], page);
                    window.scrollTo({ top: 0, behavior: 'smooth' });
                }
            });
        });
    }

    showError(message) {
        const container = document.getElementById('productsContainer');
        if (container) {
            container.innerHTML = `
                <div class="alert alert-danger">
                    <i class="bi bi-exclamation-triangle me-2"></i>${message}
                </div>
            `;
        }
    }

    getStep(range) {
        if (range >= 50000000) return 1000000;  // 1M steps
        if (range >= 10000000) return 500000;   // 500K steps
        if (range >= 5000000) return 250000;    // 250K steps
        if (range >= 2000000) return 100000;    // 100K steps
        if (range >= 1000000) return 50000;     // 50K steps
        return 10000; // 10K steps for small ranges
    }

    formatPriceNumber(value) {
        const num = Math.round(Number(value));
        return new Intl.NumberFormat('vi-VN').format(num) + 'đ';
    }

    // Public method to update bounds dynamically
    updateBounds(minBound, maxBound) {
        this.minBound = minBound;
        this.maxBound = maxBound;
        if (this.priceSlider) {
            this.priceSlider.updateOptions({
                range: {
                    'min': minBound,
                    'max': maxBound
                },
                step: this.getStep(maxBound - minBound)
            });
            this.priceSlider.set([minBound, maxBound]);
        }
    }

    // Public method to destroy instance
    destroy() {
        if (this.priceSlider) {
            this.priceSlider.destroy();
        }
    }
}

// Make it available globally
window.PriceFilter = PriceFilter;