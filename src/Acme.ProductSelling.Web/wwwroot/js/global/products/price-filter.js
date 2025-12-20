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
        this.onFilter = config.onFilter;

        // State
        this.priceSlider = null;
        this.currentPage = 1;
        this.sliderTouched = false;
        this.isPriceFiltered = false;
        this.isManufacturerFiltered = false;
        this.selectedManufacturerId = null;
        this.selectedManufacturerName = null;
        this.manufacturerTouched = false;
        this.currentSortValue = 'featured'; // ✅ ADD THIS
        this.currentSortLabel = 'Featured'; // ✅ ADD THIS

        // DOM Elements - Using simplified selectors
        this.priceDropdown = document.getElementById('priceDropdown');
        this.priceToggleBtn = document.getElementById('priceFilterToggle');
        this.manufacturerDropdown = document.getElementById('manufacturerDropdown');
        this.manufacturerToggleBtn = document.getElementById('manufacturerFilterToggle');
        this.sortDropdown = document.getElementById('sortDropdown'); // ✅ ADD THIS
        this.sortToggleBtn = document.getElementById('sortFilterToggle'); // ✅ ADD THIS
        this.sortButtonText = document.getElementById('sortButtonText'); // ✅ ADD THIS
        this.backdrop = document.getElementById('dropdownBackdrop');

        // Buttons
        this.resetBtn = document.getElementById('resetFilter');
        this.applyBtn = document.getElementById('applyFilter');
        this.resetManufacturerBtn = document.getElementById('resetManufacturerFilter');
        this.applyManufacturerBtn = document.getElementById('applyManufacturerFilter');

        // Active filter badges
        this.activePriceFilters = document.getElementById('activeFilters');
        this.activeManufacturerFilters = document.getElementById('activeManufacturerFilter');
        this.clearPriceFilterBtn = document.getElementById('clearPriceFilter');
        this.clearManufacturerFilterBtn = document.getElementById('clearManufacturerFilter');

        // Validate required elements
        if (!this.priceDropdown || !this.priceToggleBtn) {
            console.error('Price filter elements not found');
            return;
        }

        // Initialize
        this.init();
    }

    init() {
        console.log('Price Filter initialized with config:', {
            bounds: [this.minBound, this.maxBound],
            categorySlug: this.categorySlug,
            showManufacturerFilter: this.showManufacturerFilter
        });

        this.initPriceSlider();
        this.attachPriceFilterListeners();

        if (this.showManufacturerFilter && this.manufacturerDropdown) {
            this.initManufacturerFilter();
        }

        // ✅ ADD THIS - Initialize sort filter
        if (this.sortDropdown) {
            this.initSortFilter();
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
            range: { 'min': this.minBound, 'max': this.maxBound },
            step: this.getStep(this.maxBound - this.minBound),
            format: {
                to: (value) => Math.round(value),
                from: (value) => Number(value)
            }
        });

        this.priceSlider.on('update', (values) => {
            const minInput = document.getElementById('minPriceInput');
            const maxInput = document.getElementById('maxPriceInput');
            if (minInput) minInput.value = this.formatPriceNumber(values[0]);
            if (maxInput) maxInput.value = this.formatPriceNumber(values[1]);

            if (this.sliderTouched && this.applyBtn) {
                this.applyBtn.disabled = false;
            }
        });

        this.priceSlider.on('start', () => {
            this.sliderTouched = true;
        });
    }

    initManufacturerFilter() {
        const pills = document.querySelectorAll('.manufacturer-pill');
        pills.forEach(pill => {
            pill.addEventListener('click', () => {
                // Remove active class from all pills
                pills.forEach(p => p.classList.remove('active'));

                // Add active class to clicked pill
                pill.classList.add('active');

                // Get manufacturer data
                this.selectedManufacturerId = pill.dataset.manufacturerId || null;
                this.selectedManufacturerName = pill.dataset.manufacturerName || null;
                this.manufacturerTouched = true;

                // Enable apply button
                if (this.applyManufacturerBtn) {
                    this.applyManufacturerBtn.disabled = false;
                }
            });
        });

        this.attachManufacturerFilterListeners();
    }

    attachPriceFilterListeners() {
        // Toggle price dropdown
        this.priceToggleBtn?.addEventListener('click', (e) => {
            e.stopPropagation();
            const isOpen = this.priceDropdown.classList.contains('show');
            this.closeAllDropdowns();
            if (!isOpen) this.openPriceDropdown();
        });

        // Reset price
        this.resetBtn?.addEventListener('click', () => {
            this.sliderTouched = false;
            if (this.applyBtn) this.applyBtn.disabled = true;
            if (this.priceSlider) {
                this.priceSlider.set([this.minBound, this.maxBound]);
            }
        });

        // Apply price filter
        this.applyBtn?.addEventListener('click', () => {
            if (!this.sliderTouched) return;
            const values = this.priceSlider.get();
            this.applyFilters(values[0], values[1], this.selectedManufacturerId, 1);
            this.sliderTouched = false;
            this.closeAllDropdowns();
        });

        // Clear price filter
        this.clearPriceFilterBtn?.addEventListener('click', () => {
            this.isPriceFiltered = false;
            this.sliderTouched = false;
            this.activePriceFilters?.classList.remove('show');
            this.priceToggleBtn?.classList.remove('active');
            if (this.priceSlider) {
                this.priceSlider.set([this.minBound, this.maxBound]);
            }
            this.applyFilters(this.minBound, this.maxBound, this.selectedManufacturerId, 1);
            this.closeAllDropdowns();
        });

        // Close on backdrop click
        this.backdrop?.addEventListener('click', () => this.closeAllDropdowns());
    }

    attachManufacturerFilterListeners() {
        // Toggle manufacturer dropdown
        this.manufacturerToggleBtn?.addEventListener('click', (e) => {
            e.stopPropagation();
            const isOpen = this.manufacturerDropdown.classList.contains('show');
            this.closeAllDropdowns();
            if (!isOpen) this.openManufacturerDropdown();
        });

        // Reset manufacturer
        this.resetManufacturerBtn?.addEventListener('click', () => {
            this.manufacturerTouched = false;
            if (this.applyManufacturerBtn) this.applyManufacturerBtn.disabled = true;

            // Remove active from all pills
            document.querySelectorAll('.manufacturer-pill').forEach(p => p.classList.remove('active'));

            // Set first pill (All) as active
            const allPill = document.querySelector('.manufacturer-pill[data-manufacturer-id=""]');
            if (allPill) allPill.classList.add('active');

            this.selectedManufacturerId = null;
            this.selectedManufacturerName = null;
        });

        // Apply manufacturer filter
        this.applyManufacturerBtn?.addEventListener('click', () => {
            if (!this.manufacturerTouched) return;
            const priceValues = this.priceSlider.get();
            this.applyFilters(priceValues[0], priceValues[1], this.selectedManufacturerId, 1);
            this.manufacturerTouched = false;
            this.closeAllDropdowns();
        });

        // Clear manufacturer filter
        this.clearManufacturerFilterBtn?.addEventListener('click', () => {
            this.isManufacturerFiltered = false;
            this.manufacturerTouched = false;
            this.selectedManufacturerId = null;
            this.selectedManufacturerName = null;
            this.activeManufacturerFilters?.classList.remove('show');
            this.manufacturerToggleBtn?.classList.remove('active');

            // Reset pills
            document.querySelectorAll('.manufacturer-pill').forEach(p => p.classList.remove('active'));
            const allPill = document.querySelector('.manufacturer-pill[data-manufacturer-id=""]');
            if (allPill) allPill.classList.add('active');

            const priceValues = this.priceSlider.get();
            this.applyFilters(priceValues[0], priceValues[1], null, 1);
            this.closeAllDropdowns();
        });
    }

    openPriceDropdown() {
        this.priceDropdown.classList.add('show');
        this.backdrop.classList.add('show');
        if (!this.isPriceFiltered) {
            this.sliderTouched = false;
            if (this.applyBtn) this.applyBtn.disabled = true;
            if (this.priceSlider) {
                this.priceSlider.set([this.minBound, this.maxBound]);
            }
        }
    }

    openManufacturerDropdown() {
        this.manufacturerDropdown.classList.add('show');
        this.backdrop.classList.add('show');
        if (!this.isManufacturerFiltered) {
            this.manufacturerTouched = false;
            if (this.applyManufacturerBtn) this.applyManufacturerBtn.disabled = true;
        }
    }

    closeAllDropdowns() {
        // Remove 'show' class from all dropdowns using shared class
        document.querySelectorAll('.filter-dropdown').forEach(dropdown => {
            dropdown.classList.remove('show');
        });
        this.backdrop?.classList.remove('show');
    }

    applyFilters(minPrice, maxPrice, manufacturerId, page = 1) {
        if (this.onFilter) {
            this.onFilter(minPrice, maxPrice, page, manufacturerId);
            this.updateActiveFilters(minPrice, maxPrice, manufacturerId);
            return;
        }

        // Default AJAX implementation
        const overlay = document.querySelector('.loading-overlay');
        if (overlay) overlay.classList.add('active');

        let url = `${this.apiEndpoint}?minPrice=${minPrice}&maxPrice=${maxPrice}&page=${page}&pageSize=${this.pageSize}`;
        if (this.categorySlug) url += `&categorySlug=${this.categorySlug}`;
        if (this.manufacturerSlug) url += `&manufacturerSlug=${this.manufacturerSlug}`;
        if (this.searchKeyword) url += `&searchKeyword=${encodeURIComponent(this.searchKeyword)}`;
        if (manufacturerId) url += `&manufacturerId=${manufacturerId}`;

        fetch(url)
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    this.updateProductsGrid(data.data);
                    this.updateActiveFilters(minPrice, maxPrice, manufacturerId);
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

    updateActiveFilters(minPrice, maxPrice, manufacturerId) {
        // Update price filter
        const isPriceDefault = Math.round(minPrice) === this.minBound && Math.round(maxPrice) === this.maxBound;
        if (isPriceDefault) {
            this.isPriceFiltered = false;
            this.activePriceFilters?.classList.remove('show');
            this.priceToggleBtn?.classList.remove('active');
        } else {
            this.isPriceFiltered = true;
            const rangeSpan = document.getElementById('currentPriceRange');
            if (rangeSpan) {
                rangeSpan.textContent = `${this.formatPriceNumber(minPrice)} - ${this.formatPriceNumber(maxPrice)}`;
            }
            this.activePriceFilters?.classList.add('show');
            this.priceToggleBtn?.classList.add('active');
        }

        // Update manufacturer filter
        if (!manufacturerId) {
            this.isManufacturerFiltered = false;
            this.activeManufacturerFilters?.classList.remove('show');
            this.manufacturerToggleBtn?.classList.remove('active');
        } else {
            this.isManufacturerFiltered = true;
            const manufacturerSpan = document.getElementById('currentManufacturer');
            if (manufacturerSpan && this.selectedManufacturerName) {
                manufacturerSpan.textContent = this.selectedManufacturerName;
            }
            this.activeManufacturerFilters?.classList.add('show');
            this.manufacturerToggleBtn?.classList.add('active');
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
                        <p class="mb-0">Try adjusting your filters</p>
                    </div>
                </div>`;
            return;
        }

        let html = '<div class="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-lg-4 row-cols-xl-5 g-4">';
        products.forEach(product => html += this.renderProductCard(product));
        html += '</div>';
        container.innerHTML = html;
    }

    renderProductCard(product) {
        const price = product.discountedPrice || product.originalPrice;
        const formattedPrice = new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price);
        const originalPriceFormatted = new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(product.originalPrice);

        return `
            <div class="col">
                <div class="card product-card h-100 shadow-sm">
                    <a href="/products/${product.urlSlug}">
                        <img src="https://placehold.co/200x200?text=${product.productName.charAt(0)}"
                             class="card-img-top p-3" alt="${product.productName}"
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
                                <div class="text-muted text-decoration-line-through small mb-1">${originalPriceFormatted}</div>
                                <div class="d-flex align-items-center gap-2">
                                    <span class="fw-bold text-danger fs-6">${formattedPrice}</span>
                                    <span class="badge bg-danger small">-${product.discountPercent}%</span>
                                </div>
                            ` : `<div class="fw-bold text-danger fs-6">${formattedPrice}</div>`}
                        </div>
                    </div>
                    <div class="card-footer bg-transparent border-0 text-center pb-2">
                        <a href="/products/${product.urlSlug}" class="btn btn-sm btn-primary w-100">View Details</a>
                    </div>
                </div>
            </div>`;
    }

    updatePagination(totalPages, currentPage) {
        const container = document.getElementById('paginationContainer');
        if (!container || totalPages <= 1) {
            if (container) container.innerHTML = '';
            return;
        }

        let html = '<nav><ul class="pagination justify-content-center">';
        html += `<li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
            <a class="page-link" href="#" data-page="${currentPage - 1}">Previous</a></li>`;

        for (let i = 1; i <= totalPages; i++) {
            if (i === 1 || i === totalPages || (i >= currentPage - 2 && i <= currentPage + 2)) {
                html += `<li class="page-item ${i === currentPage ? 'active' : ''}">
                    <a class="page-link" href="#" data-page="${i}">${i}</a></li>`;
            } else if (i === currentPage - 3 || i === currentPage + 3) {
                html += '<li class="page-item disabled"><span class="page-link">...</span></li>';
            }
        }

        html += `<li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
            <a class="page-link" href="#" data-page="${currentPage + 1}">Next</a></li></ul></nav>`;
        container.innerHTML = html;

        container.querySelectorAll('.page-link').forEach(link => {
            link.addEventListener('click', (e) => {
                e.preventDefault();
                if (!link.parentElement.classList.contains('disabled') && !link.parentElement.classList.contains('active')) {
                    const page = parseInt(link.dataset.page);
                    const values = this.priceSlider.get();
                    this.applyFilters(values[0], values[1], this.selectedManufacturerId, page);
                    window.scrollTo({ top: 0, behavior: 'smooth' });
                }
            });
        });
    }

    showError(message) {
        const container = document.getElementById('productsContainer');
        if (container) {
            container.innerHTML = `<div class="alert alert-danger">
                <i class="bi bi-exclamation-triangle me-2"></i>${message}</div>`;
        }
    }

    getStep(range) {
        if (range >= 50000000) return 1000000;
        if (range >= 10000000) return 500000;
        if (range >= 5000000) return 250000;
        if (range >= 2000000) return 100000;
        if (range >= 1000000) return 50000;
        return 10000;
    }

    formatPriceNumber(value) {
        return new Intl.NumberFormat('vi-VN').format(Math.round(Number(value))) + 'đ';
    }

    updateBounds(minBound, maxBound) {
        this.minBound = minBound;
        this.maxBound = maxBound;
        if (this.priceSlider) {
            this.priceSlider.updateOptions({
                range: { 'min': minBound, 'max': maxBound },
                step: this.getStep(maxBound - minBound)
            });
            this.priceSlider.set([minBound, maxBound]);
        }
    }

    // ✅ ADD SORT FILTER METHODS
    initSortFilter() {
        const sortOptions = document.querySelectorAll('.sort-option');

        sortOptions.forEach(option => {
            option.addEventListener('click', () => {
                // Remove active from all options
                sortOptions.forEach(opt => opt.classList.remove('active'));

                // Add active to clicked option
                option.classList.add('active');

                // Get sort value and label
                this.currentSortValue = option.dataset.sortValue;
                this.currentSortLabel = option.dataset.sortLabel;

                // Update button text
                if (this.sortButtonText) {
                    this.sortButtonText.textContent = this.currentSortLabel;
                }

                // Apply sort and close dropdown
                this.applySortAndFilter();
                this.closeAllDropdowns();
            });
        });

        // Toggle sort dropdown
        this.sortToggleBtn?.addEventListener('click', (e) => {
            e.stopPropagation();
            const isOpen = this.sortDropdown.classList.contains('show');
            this.closeAllDropdowns();
            if (!isOpen) this.openSortDropdown();
        });
    }

    openSortDropdown() {
        this.sortDropdown.classList.add('show');
        this.backdrop.classList.add('show');
    }

    applySortAndFilter() {
        // Get current filter values
        const priceValues = this.priceSlider ? this.priceSlider.get() : [this.minBound, this.maxBound];

        // Sort products in the current view (frontend only)
        this.sortProducts(this.currentSortValue);

        console.log('Applied sort:', this.currentSortValue);
    }

    sortProducts(sortValue) {
        const container = document.getElementById('productsContainer');
        if (!container) return;

        const productGrid = container.querySelector('.row');
        if (!productGrid) return;

        const products = Array.from(productGrid.querySelectorAll('.col'));

        products.sort((a, b) => {
            switch (sortValue) {
                case 'name-asc':
                    return this.getProductName(a).localeCompare(this.getProductName(b));

                case 'name-desc':
                    return this.getProductName(b).localeCompare(this.getProductName(a));

                case 'price-asc':
                    return this.getProductPrice(a) - this.getProductPrice(b);

                case 'price-desc':
                    return this.getProductPrice(b) - this.getProductPrice(a);

                case 'featured':
                default:
                    return 0; // Keep original order
            }
        });

        // Reorder DOM elements
        products.forEach(product => productGrid.appendChild(product));
    }

    getProductName(productElement) {
        const nameElement = productElement.querySelector('.card-title a');
        return nameElement ? nameElement.textContent.trim() : '';
    }

    getProductPrice(productElement) {
        // Get the actual price (discounted or original)
        const priceElement = productElement.querySelector('.fw-bold.text-danger');
        if (!priceElement) return 0;

        // Remove currency symbols and parse
        const priceText = priceElement.textContent.replace(/[^\d]/g, '');
        return parseInt(priceText) || 0;
    }

    destroy() {
        if (this.priceSlider) this.priceSlider.destroy();
    }
}

window.PriceFilter = PriceFilter;