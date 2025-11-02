(function ($) {


    $(function () {

        initializeProductForm();
    });

    function initializeProductForm() {
        // --- Slug Generation ---
        handleSlugGeneration();

        // --- Image Preview and Input Toggling ---
        handleImageInputs();

        // --- Dynamic Specification Form Display ---
        handleSpecificationDisplay();

        // --- Automatic Discount Calculation ---
        handleDiscountCalculation();
    }

    function handleSlugGeneration() {
        var nameInput = $('#Product_ProductName');
        var slugInput = $('#Product_UrlSlug');

        if (!nameInput.length || !slugInput.length) return;

        function generateSlug(text) {
            if (!text) return "";
            text = text.replace(/Đ/g, 'D').replace(/đ/g, 'd');
            text = text.normalize('NFD').replace(/[\u0300-\u036f]/g, '');
            text = text.replace(/\s+/g, '-');
            text = text.replace(/[^\w-]+/g, '');
            text = text.toLowerCase();
            text = text.replace(/-+/g, '-').replace(/^-+|-+$/g, '');
            return text;
        }

        nameInput.on('input keyup', function () {
            slugInput.val(generateSlug($(this).val()));
        });

        if (nameInput.val()) {
            slugInput.val(generateSlug(nameInput.val()));
        }
    }

    function handleImageInputs() {
        const imageSourceUrlRadio = document.getElementById('imageSourceUrl');
        const imageSourceUploadRadio = document.getElementById('imageSourceUpload');
        const imageUrlInputContainer = document.getElementById('imageUrlInputContainer');
        const imageUploadInputContainer = document.getElementById('imageUploadInputContainer');
        const productImageFileInput = document.getElementById('productImageFile');
        const imagePreviewContainer = document.getElementById('imagePreviewContainer');
        const imagePreview = document.getElementById('imagePreview');
        const imageUrlInput = imageUrlInputContainer ? imageUrlInputContainer.querySelector("input[name='Product.ImageUrl']") : null;

        if (!imageSourceUrlRadio) return; // Exit if the elements don't exist

        function toggleImageInputFields() {
            if (imageSourceUrlRadio.checked) {
                imageUrlInputContainer.style.display = 'block';
                imageUploadInputContainer.style.display = 'none';
                productImageFileInput.value = ''; // Clear file input

                if (imageUrlInput && imageUrlInput.value) {
                    displayImagePreview(imageUrlInput.value);
                } else {
                    imagePreviewContainer.style.display = 'none';
                }
            } else { // Upload is checked
                imageUrlInputContainer.style.display = 'none';
                imageUploadInputContainer.style.display = 'block';

                if (productImageFileInput.files && productImageFileInput.files[0]) {
                    displayImagePreview(productImageFileInput.files[0]);
                } else {
                    // Check if there was a URL before switching, if so, keep preview
                    if (!imageUrlInput.value) {
                        imagePreviewContainer.style.display = 'none';
                    }
                }
            }
        }

        function displayImagePreview(fileOrUrl) {
            imagePreviewContainer.style.display = 'block';
            if (typeof fileOrUrl === 'string') { // It's a URL
                imagePreview.src = fileOrUrl;
            } else { // It's a file object
                const reader = new FileReader();
                reader.onload = e => imagePreview.src = e.target.result;
                reader.readAsDataURL(fileOrUrl);
            }
        }

        imageSourceUrlRadio.addEventListener('change', toggleImageInputFields);
        imageSourceUploadRadio.addEventListener('change', toggleImageInputFields);

        if (imageUrlInput) {
            imageUrlInput.addEventListener('input', () => {
                if (imageSourceUrlRadio.checked) {
                    displayImagePreview(imageUrlInput.value);
                }
            });
        }

        if (productImageFileInput) {
            productImageFileInput.addEventListener('change', () => {
                if (imageSourceUploadRadio.checked && productImageFileInput.files[0]) {
                    displayImagePreview(productImageFileInput.files[0]);
                }
            });
        }
        if (imageUrlInput && imageUrlInput.value && imageSourceUrlRadio.checked) {
            displayImagePreview(imageUrlInput.value);
        }

        // Initial call to set the correct state on page load
        toggleImageInputFields();
    }

    function handleSpecificationDisplay() {
        const form = document.querySelector("#createProductForm, #editProductForm");
        if (!form) return;

        // *** THIS IS THE NEW WAY TO GET THE DATA ***
        // 1. Read the JSON string from the data attribute.
        const categorySpecJson = form.dataset.categorySpecs;
        // 2. Parse it into a JavaScript object.
        const categorySpecTypes = JSON.parse(categorySpecJson || '{}');

        const categorySelect = document.getElementById("Product_CategoryId");
        const formContainer = document.getElementById("productFormContainer");
        const specsSection = document.getElementById("productSpecsSection");
        const allSpecForms = specsSection.querySelectorAll(".spec-form");

        if (!categorySelect) return;

        function showSpecForm(categoryId) {
            allSpecForms.forEach(form => {
                form.style.display = 'none';
                form.querySelectorAll('input, select, textarea').forEach(input => input.disabled = true);
            });

            if (!categoryId || categoryId === "") {
                specsSection.style.display = 'none';
                formContainer.classList.remove('two-columns-layout');
                return;
            }

            specsSection.style.display = 'block';
            formContainer.classList.add('two-columns-layout');

            const specType = categorySpecTypes[categoryId.toString()];
            const formToShowId = specType ? `spec-${specType}` : "spec-None";
            const formToDisplay = document.getElementById(formToShowId) || document.getElementById('spec-None');

            if (formToDisplay) {
                formToDisplay.style.display = 'block';
                formToDisplay.querySelectorAll('input, select, textarea').forEach(input => input.disabled = false);
            }
        }

        showSpecForm(categorySelect.value);
        categorySelect.addEventListener("change", (e) => showSpecForm(e.target.value));
    }

    function handleDiscountCalculation() {
        const originalPriceInput = document.querySelector("input[name='Product.OriginalPrice']");
        const discountPercentInput = document.querySelector("input[name='Product.DiscountPercent']");
        const discountedPriceInput = document.querySelector("input[name='Product.DiscountedPrice']");

        if (!originalPriceInput || !discountPercentInput || !discountedPriceInput) return;

        function calculateDiscountedPrice() {
            const originalPrice = parseFloat(originalPriceInput.value) || 0;
            let discountPercent = parseFloat(discountPercentInput.value) || 0;

            if (discountPercent < 0) discountPercent = 0;
            if (discountPercent > 100) discountPercent = 100;

            const discountedPrice = originalPrice * (1 - (discountPercent / 100));
            discountedPriceInput.value = discountedPrice.toFixed(2);
        }

        originalPriceInput.addEventListener('input', calculateDiscountedPrice);
        discountPercentInput.addEventListener('input', calculateDiscountedPrice);
        calculateDiscountedPrice(); // Initial calculation on page load
    }

})(jQuery);