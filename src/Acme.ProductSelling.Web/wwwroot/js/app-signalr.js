document.addEventListener("DOMContentLoaded", () => {

    // 1. Hub Connection Configuration
    //----------------------------------
    const hubUrl = "/signalr-hubs/orders"; // Must match the server-side MapHub path

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl, {
            // accessTokenFactory is used to send the JWT token for authentication
            // SignalR will use this token to identify the user on the server.
            accessTokenFactory: () => {
                // Replace 'your_jwt_token_key' with the key you use to store the token
                // This function should return the actual JWT string.
                return localStorage.getItem("your_jwt_token_key");
            }
        })
        .withAutomaticReconnect([0, 2000, 10000, 30000]) // Optional: configure auto-reconnect delays
        .configureLogging(signalR.LogLevel.Information) // Optional: for debugging
        .build();

    // 2. Define what happens when a message is received from the server
    //------------------------------------------------------------------
    // "ReceiveOrderStatusUpdate" must match the method name called by the server
    // and defined in your IOrderClient interface.
    connection.on("ReceiveOrderStatusUpdate", (orderId, orderNumber, newStatusEnumValue, statusText) => {
        console.log(`SignalR: Order Status Update Received:
            Order ID: ${orderId},
            Order Number: ${orderNumber},
            New Status Enum Value: ${newStatusEnumValue}, // This is the integer value of the OrderStatus enum
            Status Text: ${statusText}`); // This is the string representation (e.g., "Confirmed")

        // Update the UI
        updateOrderStatusInUI(orderId, statusText);

        // Show a notification (you can use a more sophisticated library like Toastr)
        alert(`Order ${orderNumber} status has been updated to: ${statusText}`);
    });

    // 3. Function to update the UI
    //-------------------------------
    function updateOrderStatusInUI(orderId, newStatusText) {
        // Find the list item for the order using a data attribute
        const orderListItem = document.querySelector(`li[data-order-id="${orderId}"]`);

        if (orderListItem) {
            const statusSpan = orderListItem.querySelector(".order-status");
            if (statusSpan) {
                statusSpan.textContent = newStatusText;
                // You might want to add a class to highlight the change temporarily
                orderListItem.classList.add("status-updated");
                setTimeout(() => {
                    orderListItem.classList.remove("status-updated");
                }, 3000); // Remove highlight after 3 seconds
            } else {
                console.warn(`SignalR: Could not find status span for order ID ${orderId}`);
            }
        } else {
            console.warn(`SignalR: Could not find UI element for order ID ${orderId}. The order might not be on the current page.`);
        }
    }

    // 4. Start the connection and handle connection events
    //----------------------------------------------------
    async function startSignalRConnection() {
        try {
            await connection.start();
            console.log("SignalR: Connected to OrderHub successfully.");

            // Example: If you had admin groups or specific order subscriptions
            // (Requires corresponding methods on the server-side Hub like "SubscribeToOrderUpdates")
            // if (userIsAdmin) {
            //     await connection.invoke("JoinGroup", "OrderAdmins");
            // }
            // await connection.invoke("SubscribeToOrderUpdates", "someSpecificOrderId");

        } catch (err) {
            console.error("SignalR: Connection failed: ", err);
            // The withAutomaticReconnect setting will handle retries.
            // You might want to inform the user that real-time updates are down.
        }
    }

    // Handle connection closed event (especially if not using withAutomaticReconnect or if it fails permanently)
    connection.onclose(error => {
        console.warn("SignalR: Connection closed.", error);
        // You might want to inform the user or implement custom retry logic if
        // withAutomaticReconnect is not sufficient or disabled.
    });

    // Handle reconnected event
    connection.onreconnected(connectionId => {
        console.log(`SignalR: Reconnected with ID ${connectionId}.`);
        // You might need to re-join groups or re-subscribe if your application uses them
        // and state is lost on disconnect/reconnect.
    });

    // Handle reconnecting event
    connection.onreconnecting(error => {
        console.warn("SignalR: Attempting to reconnect...", error);
        // You could show a visual indicator to the user that the connection is trying to re-establish.
    });


    // 5. Initializing the connection
    //-------------------------------
    // Make sure the user is authenticated before trying to connect if you rely on the token
    if (localStorage.getItem("your_jwt_token_key")) {
        startSignalRConnection();
    } else {
        console.warn("SignalR: No auth token found. Real-time updates will not connect for user-specific notifications.");
        // You might still want to connect if there are public notifications,
        // but for user-specific ones, it won't work without a token.
    }

});