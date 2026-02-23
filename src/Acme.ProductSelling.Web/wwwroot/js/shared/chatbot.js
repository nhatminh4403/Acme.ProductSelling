$(document).ready(function () {

    const l = abp.localization.getResource('ProductSelling');
    const chatMessages = $('#chatMessages');
    const messageInput = $('#messageInput');
    const sendButton = $('#sendButton');
    const chatForm = $('#chatForm');
    const userRoleBadge = $('#userRoleBadge');

    const currencyFormatter = new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    });

    let conversationHistory = [];


    messageInput.attr('placeholder', l('Chatbot:InputPlaceholder'));

    // Add Welcome Message (System style)
    addMessageToChat('system', l('Chatbot:WelcomeMessage'));

    chatForm.on('submit', function (e) {
        e.preventDefault();
        const message = messageInput.val().trim();
        if (message) {
            sendMessage(message);
            messageInput.val('');
        }
    });

    // Handle "Quick Question" buttons
    $('.quick-question').on('click', function () {
        const question = $(this).text(); // Use the localized text from the button
        sendMessage(question);
    });

    //MAIN LOGIC


    function sendMessage(message) {
        // Show User Message
        addMessageToChat('user', message);

        // Lock UI & Show Loading
        const loadingId = showLoading();
        toggleInput(false);


        // Send to Backend
        acme.productSelling.chatbot.chatbot.sendMessage({
            message: message,
            conversationHistory: conversationHistory
        }).then(function (response) {
            removeLoading(loadingId);

            // A. Render Text Response (from Gemini)
            addMessageToChat('assistant', response.response);

            // B. Render Product Cards (if DB found items)
            if (response.relatedProducts && response.relatedProducts.length > 0) {
                renderProductGrid(response.relatedProducts);
            }

            // C. Update History & Badge
            conversationHistory.push({ role: 'user', content: message });
            conversationHistory.push({ role: 'assistant', content: response.response });

            // Keep history short
            if (conversationHistory.length > 10) conversationHistory = conversationHistory.slice(-10);

            updateRoleBadge(response.userRole);

        }).catch(function (err) {
            removeLoading(loadingId);
            addMessageToChat('error', l('Chatbot:System:Error'));
            console.error("Chatbot API Error:", err);
        }).always(function () {
            toggleInput(true);
            messageInput.focus();
        });
    }

    // ----------------------------------------------------------------
    // 5. UI HELPERS
    // ----------------------------------------------------------------

    function addMessageToChat(role, content) {
        // Styles
        const isUser = role === 'user';
        const isError = role === 'error';
        const isSystem = role === 'system';

        let alignClass, bubbleClass, iconHtml = '';

        if (isUser) {
            alignClass = 'justify-content-end';
            bubbleClass = 'bg-primary text-white shadow-sm';
        } else if (isError) {
            alignClass = 'justify-content-start';
            bubbleClass = 'bg-danger text-white shadow-sm';
            iconHtml = '<i class="fas fa-exclamation-triangle me-2"></i>';
        } else if (isSystem) {
            alignClass = 'justify-content-start';
            bubbleClass = 'bg-white text-dark border'; // Neutral look
            iconHtml = '<i class="fas fa-info-circle text-primary me-2"></i>';
        } else {
            // Assistant
            alignClass = 'justify-content-start';
            bubbleClass = 'bg-white border text-dark shadow-sm';
            iconHtml = '<i class="fas fa-robot text-primary me-2"></i>';
        }

        // Render Markdown for AI, Plain Text for User
        let finalContent;
        if (isUser) {
            finalContent = $('<div>').text(content).html(); // Escape HTML
        } else {
            // Parse markdown (bold, lists)
            finalContent = marked.parse(content);
        }

        const html = `
                    <div class="d-flex ${alignClass} mb-3">
                        <div class="p-3 rounded message-bubble ${bubbleClass}" style="max-width: 85%;">
                            ${iconHtml}
                            <span class="d-inline-block">${finalContent}</span>
                        </div>
                    </div>
                `;

        chatMessages.append(html);
        scrollToBottom();
    }

    function renderProductGrid(products) {
        let cardsHtml = '';
        const btnText = l('Chatbot:Button:View');

        products.forEach(p => {
            // Image fallback
            const img = p.imageUrl || 'https://via.placeholder.com/300x200?text=No+Image';

            // Price Formatting logic (Standard Logic)
            const finalPrice = p.discountedPrice || p.originalPrice;
            const priceDisplay = currencyFormatter.format(finalPrice);

            // Badges for Discount
            let badgeHtml = '';
            if (p.discountPercent > 0) {
                badgeHtml = `<span class="badge bg-danger position-absolute top-0 end-0 m-2">-${p.discountPercent}%</span>`;
            }

            cardsHtml += `
                        <div class="col">
                            <div class="card h-100 shadow-sm border-0 position-relative">
                                ${badgeHtml}
                                <img src="${img}" class="card-img-top" style="height: 120px; object-fit: contain; padding:10px;">
                                <div class="card-body p-2 d-flex flex-column">
                                    <h6 class="card-title text-truncate small mb-1" title="${p.productName}">
                                        ${p.productName}
                                    </h6>

                                    <!-- Manufacturer / Brand -->
                                    <div class="text-muted small mb-2" style="font-size:0.75rem;">
                                        ${p.manufacturerName || 'Acme Gear'}
                                    </div>

                                    <!-- Price Block -->
                                    <div class="mt-auto">
                                        <div class="fw-bold text-primary mb-2">${priceDisplay}</div>
                                        <a href="/products/${p.urlSlug}" target="_blank" class="btn btn-sm btn-outline-primary w-100 stretched-link">
                                            ${btnText}
                                        </a>
                                    </div>
                                </div>
                            </div>
                        </div>
                    `;
        });

        const foundHeader = l('Chatbot:Header:FoundProducts');
        const wrapper = `
                    <div class="d-flex justify-content-start mb-4">
                        <div class="p-3 bg-light rounded w-100 border">
                            <small class="text-muted fw-bold mb-2 d-block text-uppercase">${foundHeader}</small>
                            <!-- Grid with Responsive Columns -->
                            <div class="row row-cols-2 row-cols-sm-2 row-cols-md-3 g-2">
                                ${cardsHtml}
                            </div>
                        </div>
                    </div>
                `;

        chatMessages.append(wrapper);
        scrollToBottom();
    }

    function showLoading() {
        const id = 'load-' + Date.now();
        const text = l('Chatbot:Thinking');
        const html = `
                    <div id="${id}" class="d-flex justify-content-start mb-3">
                        <div class="bg-light p-2 px-3 rounded text-muted small border">
                            <div class="spinner-border spinner-border-sm text-secondary me-2" role="status"></div>
                            ${text}
                        </div>
                    </div>`;
        chatMessages.append(html);
        scrollToBottom();
        return id;
    }

    function removeLoading(id) { $('#' + id).remove(); }

    function toggleInput(enabled) {
        messageInput.prop('disabled', !enabled);
        sendButton.prop('disabled', !enabled);
    }

    function scrollToBottom() {
        setTimeout(() => {
            chatMessages.animate({ scrollTop: chatMessages[0].scrollHeight }, 300);
        }, 100);
    }

    function updateRoleBadge(roleKey) {
        // Map the server keys to clean UI labels if needed
        const map = {
            'admin': 'Admin / SuperUser',
            'manager': 'Manager',
            'sales_agent': 'Sales Staff',
            'warehouse': 'Warehouse',
            'customer': 'Guest'
        };

        // You could also wrap these strings in L[] calls on server side for full localization
        const label = map[roleKey] || 'Guest';
        userRoleBadge.text(label);
    }
});