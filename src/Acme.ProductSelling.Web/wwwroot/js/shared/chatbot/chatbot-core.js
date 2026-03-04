window.ChatbotCore = (function () {

    const STORAGE_KEYS = {
        admin: 'chatbot_admin',
        customer: 'chatbot_customer'
    };

    const MAX_HISTORY = 10;

    const ROLE_LABELS = {
        admin: 'Admin / SuperUser',
        manager: 'Manager',
        sales_agent: 'Sales Staff',
        warehouse: 'Warehouse',
        customer: 'Customer',
        anonymous: 'Guest'
    };

    const currencyFormatter = new Intl.NumberFormat('vi-VN', {
        style: 'currency', currency: 'VND'
    });

    // ── Role resolution (synchronous, no network call) ──────────────────────
    /**
     * Reads abp.currentUser which ABP injects into every page.
     * Maps ABP role strings → internal role keys used for storage scoping.
     *
     * Priority order mirrors ChatbotAppService.DetermineUserRole() exactly.
     * Role strings must match your IdentityRoleConsts values.
     *
     * To verify: open browser console and run:  abp.currentUser.roles
     */
    function resolveCurrentRole() {
        try {
            if (!abp.currentUser || !abp.currentUser.isAuthenticated) return 'anonymous';

            const roles = abp.currentUser.roles || [];

            // Exact strings from IdentityRoleConsts.cs — priority mirrors
            // ChatbotAppService.DetermineUserRole() on the server.
            if (roles.includes('admin')) return 'admin';
            if (roles.includes('manager')) return 'manager';
            if (roles.includes('warehouse_staff')) return 'warehouse';
            if (roles.includes('seller') ||
                roles.includes('cashier')) return 'sales_agent';
            if (roles.includes('customer')) return 'customer';

            return 'anonymous';
        } catch {
            return 'anonymous';
        }
    }

    // ── Persistence ──────────────────────────────────────────────────────────
    /**
     * Loads history for the given storage key.
     * If the stored role doesn't match currentRole, wipes it and returns [].
     * Returns the clean messages array directly — no { stale } wrapper needed
     * because resolution is now synchronous.
     */
    function loadHistory(storageKey, currentRole) {
        try {
            const raw = sessionStorage.getItem(storageKey);
            if (!raw) return [];

            const payload = JSON.parse(raw);

            // Legacy format (plain array) — keep as-is
            if (Array.isArray(payload)) return payload;

            // Role mismatch — discard immediately
            if (payload.role && payload.role !== currentRole) {
                sessionStorage.removeItem(storageKey);
                console.info(
                    `[Chatbot] Cleared history: stored role "${payload.role}" ≠ current role "${currentRole}"`
                );
                return [];
            }

            return payload.messages || [];
        } catch {
            return [];
        }
    }

    function saveHistory(storageKey, messages, role) {
        try {
            sessionStorage.setItem(storageKey, JSON.stringify({ role, messages }));
        } catch { /* storage full — silently ignore */ }
    }

    function clearHistory(storageKey) {
        sessionStorage.removeItem(storageKey);
    }

    // ── Rendering ────────────────────────────────────────────────────────────

    function renderMessage($container, role, content) {
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
            bubbleClass = 'bg-white text-dark border';
            iconHtml = '<i class="fas fa-info-circle text-primary me-2"></i>';
        } else {
            alignClass = 'justify-content-start';
            bubbleClass = 'bg-white border text-dark shadow-sm';
            iconHtml = '<i class="fas fa-robot text-primary me-2"></i>';
        }

        const finalContent = isUser
            ? $('<div>').text(content).html()
            : (typeof marked !== 'undefined' ? marked.parse(content) : content);

        const html = `
            <div class="d-flex ${alignClass} mb-3">
                <div class="p-3 rounded chatbot-message-bubble ${bubbleClass}" style="max-width:85%;">
                    ${iconHtml}<span class="d-inline-block">${finalContent}</span>
                </div>
            </div>`;

        $container.append(html);
        scrollToBottom($container);
    }

    function renderProductGrid($container, products, l) {
        const btnText = l ? l('Chatbot:Button:View') : 'View';
        const foundHeader = l ? l('Chatbot:Header:FoundProducts') : 'Found Products';

        let cardsHtml = '';
        products.forEach(p => {
            const img = p.imageUrl || 'https://via.placeholder.com/300x200?text=No+Image';
            const finalPrice = p.discountedPrice || p.originalPrice;
            const priceHtml = currencyFormatter.format(finalPrice);
            const badge = p.discountPercent > 0
                ? `<span class="badge bg-danger position-absolute top-0 end-0 m-1" style="font-size:.65rem;">-${p.discountPercent}%</span>`
                : '';

            cardsHtml += `
                <div class="card mb-0 shadow-sm border-0 position-relative flex-shrink-0" style="width:160px;min-height:200px;">
                    ${badge}
                    <img src="${img}" class="card-img-top" style="height:100px;object-fit:contain;padding:10px;">
                    <div class="card-body p-2 d-flex flex-column">
                        <h6 class="card-title text-truncate fw-bold mb-1" title="${p.productName}" style="font-size:.8rem;">${p.productName}</h6>
                        <div class="text-muted text-truncate mb-2" title="${p.manufacturerName || ''}" style="font-size:.7rem;">${p.manufacturerName || 'Acme Gear'}</div>
                        <div class="mt-auto">
                            <div class="fw-bold text-primary mb-2" style="font-size:.85rem;">${priceHtml}</div>
                            <a href="/products/${p.urlSlug}" target="_blank"
                               class="btn btn-outline-primary w-100 stretched-link"
                               style="font-size:.75rem;padding:.2rem .5rem;">${btnText}</a>
                        </div>
                    </div>
                </div>`;
        });

        const wrapper = `
            <div class="d-flex justify-content-start mb-4">
                <div class="w-100" style="max-width:85%;">
                    <small class="text-muted fw-bold mb-2 ps-1 d-block text-uppercase" style="font-size:.75rem;">${foundHeader}</small>
                    <div class="d-flex gap-2 align-items-stretch pb-2" style="overflow-x:auto;scrollbar-width:thin;">
                        ${cardsHtml}
                    </div>
                </div>
            </div>`;

        $container.append(wrapper);
        scrollToBottom($container);
    }

    function showLoading($container, l) {
        const id = 'load-' + Date.now();
        const text = l ? l('Chatbot:Thinking') : 'Thinking…';
        const html = `
            <div id="${id}" class="d-flex justify-content-start mb-3">
                <div class="bg-light p-2 px-3 rounded text-muted small border">
                    <div class="spinner-border spinner-border-sm text-secondary me-2" role="status"></div>${text}
                </div>
            </div>`;
        $container.append(html);
        scrollToBottom($container);
        return id;
    }

    function removeLoading(id) { $('#' + id).remove(); }

    function scrollToBottom($container) {
        setTimeout(() => {
            $container.animate({ scrollTop: $container[0].scrollHeight }, 300);
        }, 80);
    }

    // ── Role badge ───────────────────────────────────────────────────────────

    function updateRoleBadge($badge, roleKey) {
        if ($badge && $badge.length) {
            $badge.text(ROLE_LABELS[roleKey] || 'Guest');
        }
    }

    // ── Core send ────────────────────────────────────────────────────────────

    /**
     * opts = { $container, $input, $sendBtn, $badge, l,
     *          history, storageKey, currentRole, onHistoryUpdate }
     */
    function sendMessage(message, opts) {
        const { $container, $input, $sendBtn, $badge, l,
            history, storageKey, currentRole, onHistoryUpdate } = opts;

        renderMessage($container, 'user', message);
        const loadId = showLoading($container, l);
        if ($input) $input.prop('disabled', true);
        if ($sendBtn) $sendBtn.prop('disabled', true);

        acme.productSelling.chatbot.chatbot.sendMessage({
            message: message,
            conversationHistory: history
        }).then(function (response) {
            removeLoading(loadId);
            renderMessage($container, 'assistant', response.response);

            if (response.relatedProducts && response.relatedProducts.length > 0) {
                renderProductGrid($container, response.relatedProducts, l);
            }

            history.push({ role: 'user', content: message });
            history.push({ role: 'assistant', content: response.response });
            while (history.length > MAX_HISTORY) history.splice(0, 2);

            // Save with the server-confirmed role (most authoritative source)
            saveHistory(storageKey, history, response.userRole);
            updateRoleBadge($badge, response.userRole);

            if (onHistoryUpdate) onHistoryUpdate(history, response.userRole);

        }).catch(function (err) {
            removeLoading(loadId);
            renderMessage($container, 'error', l ? l('Chatbot:System:Error') : 'Something went wrong.');
            console.error('Chatbot API Error:', err);
        }).always(function () {
            if ($input) { $input.prop('disabled', false); $input.focus(); }
            if ($sendBtn) $sendBtn.prop('disabled', false);
        });
    }

    // ── Public API ───────────────────────────────────────────────────────────
    return {
        STORAGE_KEYS,
        resolveCurrentRole,
        loadHistory,
        saveHistory,
        clearHistory,
        renderMessage,
        renderProductGrid,
        showLoading,
        removeLoading,
        scrollToBottom,
        updateRoleBadge,
        sendMessage,
        ROLE_LABELS
    };

})();