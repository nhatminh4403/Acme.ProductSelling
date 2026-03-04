$(document).ready(function () {

    if (!$('#chatbotPageWidget').length) return;

    const l = abp.localization.getResource('ProductSelling');
    const $messages = $('#chatbotPageMessages');
    const $input = $('#chatbotPageInput');
    const $sendBtn = $('#chatbotPageSendBtn');
    const $form = $('#chatbotPageForm');
    const $badge = $('#chatbotPageRoleBadge');
    const $clearBtn = $('#chatbotPageClearBtn');

    const storageKey = ChatbotCore.STORAGE_KEYS.admin;

    // ── 1. Resolve role synchronously BEFORE reading storage ─────────────────
    const currentRole = ChatbotCore.resolveCurrentRole();

    // ── 2. Load history — stale guest entries are auto-wiped ─────────────────
    let history = ChatbotCore.loadHistory(storageKey, currentRole);

    // ── 3. Set badge immediately ──────────────────────────────────────────────
    ChatbotCore.updateRoleBadge($badge, currentRole);

    $input.attr('placeholder', l('Chatbot:InputPlaceholder'));

    // ── Replay history or show welcome ────────────────────────────────────────
    if (history.length > 0) {
        history.forEach(h => ChatbotCore.renderMessage($messages, h.role, h.content));
    } else {
        ChatbotCore.renderMessage($messages, 'system', l('Chatbot:WelcomeMessage'));
    }

    // ── Send ──────────────────────────────────────────────────────────────────
    $form.on('submit', function (e) {
        e.preventDefault();
        const msg = $input.val().trim();
        if (!msg) return;
        $input.val('');
        dispatch(msg);
    });

    $(document).on('click', '#chatbotPageWidget .quick-question', function () {
        dispatch($(this).data('question') || $(this).text().trim());
    });

    // ── Clear ─────────────────────────────────────────────────────────────────
    $clearBtn.on('click', function () {
        ChatbotCore.clearHistory(storageKey);
        history = [];
        $messages.empty();
        ChatbotCore.renderMessage($messages, 'system', l('Chatbot:WelcomeMessage'));
    });

    // ── Dispatch ──────────────────────────────────────────────────────────────
    function dispatch(message) {
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
                if (serverRole && serverRole !== currentRole) {
                    ChatbotCore.updateRoleBadge($badge, serverRole);
                }
            }
        });
    }
});


















