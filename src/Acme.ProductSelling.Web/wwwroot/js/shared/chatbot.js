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
        const question = $(this).data('question') || $(this).text().trim();
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
            const img = p.imageUrl || 'https://via.placeholder.com/300x200?text=No+Image';

            const finalPrice = p.discountedPrice || p.originalPrice;
            const priceDisplay = currencyFormatter.format(finalPrice);

            let badgeHtml = '';
            if (p.discountPercent > 0) {
                badgeHtml = `<span class="badge bg-danger position-absolute top-0 end-0 m-1" style="font-size: 0.65rem;">-${p.discountPercent}%</span>`;
            }

            // Fixed small width (160px), reduced fonts, and added flex-shrink-0
            cardsHtml += `
                <div class="card mb-0 shadow-sm border-0 position-relative flex-shrink-0" style="width: 160px; min-height: 200px;">
                    ${badgeHtml}
                    <img src="${img}" class="card-img-top" style="height: 100px; object-fit: contain; padding:10px;">
                    <div class="card-body p-2 d-flex flex-column">
                        <h6 class="card-title text-truncate fw-bold mb-1" title="${p.productName}" style="font-size: 0.8rem;">
                            ${p.productName}
                        </h6>

                        <div class="text-muted text-truncate mb-2" title="${p.manufacturerName || 'Acme Gear'}" style="font-size:0.7rem;">
                            ${p.manufacturerName || 'Acme Gear'}
                        </div>

                        <div class="mt-auto">
                            <div class="fw-bold text-primary mb-2" style="font-size: 0.85rem;">${priceDisplay}</div>
                            <a href="/products/${p.urlSlug}" target="_blank" class="btn btn-outline-primary w-100 stretched-link" style="font-size: 0.75rem; padding: 0.2rem 0.5rem;">
                                ${btnText}
                            </a>
                        </div>
                    </div>
                </div>
            `;
        });

        const foundHeader = l('Chatbot:Header:FoundProducts');

        // Removed w-100, clamped size to 85% to match chat bubbles.
        // Swapped Bootstrap Grid (.row / .col) for a standard inline horizontal Flex layout (.d-flex, gap-2)
        const wrapper = `
            <div class="d-flex justify-content-start mb-4">
                <div class="w-100" style="max-width: 85%;">
                    <small class="text-muted fw-bold mb-2 ps-1 d-block text-uppercase" style="font-size: 0.75rem;">
                        ${foundHeader}
                    </small>
                    
                    <div class="d-flex gap-2 align-items-stretch pb-2" style="overflow-x: auto; scrollbar-width: thin;">
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