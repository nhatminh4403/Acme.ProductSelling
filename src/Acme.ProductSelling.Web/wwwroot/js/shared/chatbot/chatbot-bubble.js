$(document).ready(function () {

    const $adminBubble = $('#chatbotAdminBubble');
    const $customerBubble = $('#chatbotCustomerBubble');

    if ($adminBubble.length) initBubble($adminBubble, 'admin');
    if ($customerBubble.length) initBubble($customerBubble, 'customer');

    // ─────────────────────────────────────────────────────────────────────────
    function initBubble($root, mode) {

        const l = abp.localization.getResource('ProductSelling');
        const $toggle = $root.find('.chatbot-bubble-toggle');
        const $panel = $root.find('.chatbot-bubble-panel');
        const $messages = $root.find('.chatbot-bubble-messages');
        const $input = $root.find('.chatbot-bubble-input');
        const $sendBtn = $root.find('.chatbot-bubble-send');
        const $form = $root.find('.chatbot-bubble-form');
        const $badge = $root.find('.chatbot-bubble-role-badge');
        const $clearBtn = $root.find('.chatbot-bubble-clear');
        const $closeBtn = $root.find('.chatbot-bubble-close');
        const $unread = $root.find('.chatbot-bubble-unread');

        const storageKey = ChatbotCore.STORAGE_KEYS[mode] || ('chatbot_' + mode);

        // ── 1. Resolve role synchronously BEFORE reading storage ─────────────
        const currentRole = ChatbotCore.resolveCurrentRole();

        // ── 2. Load history — stale entries are wiped inside loadHistory ─────
        let history = ChatbotCore.loadHistory(storageKey, currentRole);

        // ── 3. Set badge immediately — no waiting for a server round-trip ────
        ChatbotCore.updateRoleBadge($badge, currentRole);

        let isOpen = false;
        let unreadCount = 0;

        $input.attr('placeholder', l('Chatbot:InputPlaceholder'));

        // ── Toggle ───────────────────────────────────────────────────────────
        $toggle.on('click', function () {
            if (isOpen) closePanel(); else openPanel();
        });
        $closeBtn.on('click', closePanel);

        function openPanel() {
            isOpen = true;
            $panel.removeClass('d-none').addClass('chatbot-bubble-panel--open');
            $toggle.find('.chatbot-bubble-icon-open').addClass('d-none');
            $toggle.find('.chatbot-bubble-icon-close').removeClass('d-none');

            unreadCount = 0;
            $unread.text('').addClass('d-none');

            // Populate on first open
            if ($messages.children().length === 0) {
                if (history.length > 0) {
                    history.forEach(h => ChatbotCore.renderMessage($messages, h.role, h.content));
                } else {
                    ChatbotCore.renderMessage($messages, 'system', l('Chatbot:WelcomeMessage'));
                }
            }

            $input.focus();
        }

        function closePanel() {
            isOpen = false;
            $panel.addClass('d-none').removeClass('chatbot-bubble-panel--open');
            $toggle.find('.chatbot-bubble-icon-open').removeClass('d-none');
            $toggle.find('.chatbot-bubble-icon-close').addClass('d-none');
        }

        // ── Send ─────────────────────────────────────────────────────────────
        $form.on('submit', function (e) {
            e.preventDefault();
            const msg = $input.val().trim();
            if (!msg) return;
            $input.val('');
            dispatch(msg);
        });

        $root.on('click', '.quick-question', function () {
            const q = $(this).data('question') || $(this).text().trim();
            if (!isOpen) openPanel();
            dispatch(q);
        });

        // ── Clear ────────────────────────────────────────────────────────────
        $clearBtn.on('click', function () {
            ChatbotCore.clearHistory(storageKey);
            history = [];
            $messages.empty();
            ChatbotCore.renderMessage($messages, 'system', l('Chatbot:WelcomeMessage'));
        });

        // ── Dispatch ─────────────────────────────────────────────────────────
        function dispatch(message) {
            if (!isOpen) openPanel();

            ChatbotCore.sendMessage(message, {
                $container: $messages,
                $input: $input,
                $sendBtn: $sendBtn,
                $badge: $badge,
                l: l,
                history: history,
                storageKey: storageKey,
                currentRole: currentRole,
                onHistoryUpdate: (updated, serverRole) => {
                    history = updated;
                    // Server role is the ground truth — update badge if it differs
                    if (serverRole && serverRole !== currentRole) {
                        ChatbotCore.updateRoleBadge($badge, serverRole);
                    }
                    if (!isOpen) {
                        unreadCount++;
                        $unread.text(unreadCount > 9 ? '9+' : unreadCount).removeClass('d-none');
                    }
                }
            });
        }
    }
});


















