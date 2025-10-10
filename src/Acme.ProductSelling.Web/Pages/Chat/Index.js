let sessionId = 'session_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);

async function sendMessage(text) {
    const input = document.getElementById('messageInput');
    const message = text || input.value.trim();

    if (!message) return;

    addUserMessage(message);
    input.value = '';
    showTypingIndicator();

    try {
        const response = await fetch('/api/app/chatbot/message', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                message: message,
                sessionId: sessionId,
                isAdminChat: false
            })
        });

        const data = await response.json();
        removeTypingIndicator();
        addBotMessage(data.message);
        showSuggestions(data.suggestions);

    } catch (error) {
        removeTypingIndicator();
        addBotMessage('Sorry, something went wrong. Please try again.');
        console.error('Error:', error);
    }
}

function addUserMessage(text) {
    const container = document.getElementById('chat-container');
    const messageDiv = document.createElement('div');
    messageDiv.className = 'user-message mb-3';
    messageDiv.innerHTML = `
        <div class="d-flex align-items-start justify-content-end">
            <div class="bg-primary text-white rounded-3 shadow-sm p-3" style="max-width: 80%;">
                <strong>You:</strong> ${escapeHtml(text)}
            </div>
        </div>
    `;
    container.appendChild(messageDiv);
    container.scrollTop = container.scrollHeight;
}

function addBotMessage(text) {
    const container = document.getElementById('chat-container');
    const messageDiv = document.createElement('div');
    messageDiv.className = 'bot-message mb-3';
    messageDiv.innerHTML = `
        <div class="d-flex align-items-start">
            <div class="bg-white rounded-3 shadow-sm p-3" style="max-width: 80%;">
                <strong>Bot:</strong> ${escapeHtml(text)}
            </div>
        </div>
    `;
    container.appendChild(messageDiv);
    container.scrollTop = container.scrollHeight;
}

function showTypingIndicator() {
    const container = document.getElementById('chat-container');
    const typingDiv = document.createElement('div');
    typingDiv.id = 'typing-indicator';
    typingDiv.className = 'bot-message mb-3';
    typingDiv.innerHTML = `
        <div class="d-flex align-items-start">
            <div class="bg-white rounded-3 shadow-sm p-3">
                <span class="typing-dots">
                    <span>.</span><span>.</span><span>.</span>
                </span>
            </div>
        </div>
    `;
    container.appendChild(typingDiv);
    container.scrollTop = container.scrollHeight;
}

function removeTypingIndicator() {
    const indicator = document.getElementById('typing-indicator');
    if (indicator) indicator.remove();
}

function showSuggestions(suggestions) {
    const container = document.getElementById('suggestions-container');
    container.innerHTML = '';

    if (suggestions && suggestions.length > 0) {
        suggestions.forEach(suggestion => {
            const btn = document.createElement('button');
            btn.className = 'btn btn-sm btn-outline-primary me-2 mb-2';
            btn.textContent = suggestion;
            btn.onclick = () => sendMessage(suggestion);
            container.appendChild(btn);
        });
    }
}

function handleKeyPress(event) {
    if (event.key === 'Enter') {
        sendMessage();
    }
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

// Add CSS for typing animation
const style = document.createElement('style');
style.textContent = `
    .typing-dots span {
        animation: blink 1.4s infinite;
        animation-fill-mode: both;
    }
    .typing-dots span:nth-child(2) {
        animation-delay: 0.2s;
    }
    .typing-dots span:nth-child(3) {
        animation-delay: 0.4s;
    }
    @keyframes blink {
        0%, 80%, 100% { opacity: 0; }
        40% { opacity: 1; }
    }
`;
document.head.appendChild(style);