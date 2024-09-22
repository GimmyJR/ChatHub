document.addEventListener('DOMContentLoaded', function () {
    let selectedUserId = "";
    let selectedUser = {};
    let user = JSON.parse(localStorage.getItem("accessToken"));
    let hub;

    // Fetch users and populate user list
    fetch('http://localhost:50267/users')
        .then(response => response.json())
        .then(users => {
            const userList = document.getElementById('userList');
            userList.innerHTML = ''; // Clear list before adding
            users.forEach(u => {
                if (u.id !== user.id) {
                    const li = document.createElement('li');
                    li.className = 'clearfix';
                    li.innerHTML = `
                        <img src="http://localhost:50267/${u.avatar}" alt="avatar">
                        <div class="about">
                            <div class="name">${u.name}</div>
                            <div class="status">
                                <i class="fa fa-circle ${u.status === 'online' ? 'online' : 'offline'}"></i> ${u.status}
                            </div>
                        </div>`;
                    li.addEventListener('click', () => changeUser(u));
                    userList.appendChild(li);
                }
            });
        });

    // SignalR setup
    hub = new signalR.HubConnectionBuilder().withUrl("http://localhost:50267/chat-hub").build();

    hub.start().then(() => {
        console.log("Connection started...");
        hub.invoke("Connect", user.id);
    });

    hub.on("Messages", (chat) => {
        if (selectedUserId === chat.userId) {
            addMessageToChat(chat, chat.userId !== user.id);
        }
    });

    // Change user and fetch chat history
    function changeUser(newUser) {
        selectedUserId = newUser.id;
        selectedUser = newUser;
        document.getElementById('selectedUserAvatar').src = `http://localhost:50267/${newUser.avatar}`;
        document.getElementById('selectedUserName').textContent = newUser.name;
        document.getElementById('selectedUserStatus').textContent = newUser.status;
        document.getElementById('chatArea').style.display = 'block';

        fetchChatHistory(); // Fetch chat history for the selected user
    }

    // Fetch chat history for the current conversation
    function fetchChatHistory() {
        fetch(`http://localhost:50267/api/Chats/GetChats?userId=${user.id}&toUserId=${selectedUserId}`)
            .then(response => response.json())
            .then(chats => {
                const chatHistory = document.getElementById('chatHistory');
                chatHistory.innerHTML = ''; // Clear chat history before adding
                chats.forEach(chat => {
                    addMessageToChat(chat, chat.userId !== user.id);
                });
            });
    }

    // Add message to chat history
    function addMessageToChat(chat, isOther) {
        const chatHistory = document.getElementById('chatHistory');
        const li = document.createElement('li');
        li.className = isOther ? 'clearfix' : 'clearfix d-flex';
        li.style = isOther ? '' : 'flex-direction: column; width: 100%; align-items:flex-end;';
        li.innerHTML = `
            <div class="message-data">
                <span class="message-data-time">${chat.date}</span>
            </div>
            <div class="message ${isOther ? 'other-message' : 'my-message'}">${chat.message}</div>`;
        chatHistory.appendChild(li);
    }

    

    // Send message
    function sendMessage() {
        const messageInput = document.getElementById('messageInput');
        const data = {
            userId: user.id,
            toUserId: selectedUserId,
            message: messageInput.value // Get message from the input field
        };

        fetch("http://localhost:50267/api/Chats/SendMessage", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(data)
        })
        .then(response => response.json())
        .then(res => {
            // Assuming res is the chat message object
            addMessageToChat(res, false); // Add the message to chat history
             // Clear the input field after sending
        })
        .catch(error => {
            console.error("Error sending message:", error);
        });
    }
    document.getElementById('sendMessageBtn').addEventListener('click', sendMessage);
    document.getElementById('messageInput').addEventListener('keypress', function (e) {
        if (e.key === 'Enter') {
            sendMessage();
        }
    });

    document.getElementById('logout').addEventListener('click', async function () {
        let user = JSON.parse(localStorage.getItem('accessToken'));
    
        // Ensure user exists before making the request
        if (user && user.id) {
            try {
                // Send a request to update the user's status to "offline" using a GUID
                const response = await fetch(`http://localhost:50267/api/account/logout/${user.id}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });
    
                if (!response.ok) {
                    throw new Error('Failed to update user status');
                }
    
                // If the status update is successful, clear the local storage
                localStorage.removeItem('accessToken');
    
                // Redirect to the login page
                window.location.href = '/login.html'; // Adjust the path to your login page if necessary
            } catch (error) {
                console.error('Error logging out:', error);
                alert('An error occurred while logging out.');
            }
        } else {
            // If no user is found, proceed with the logout process
            localStorage.removeItem('accessToken');
            window.location.href = '/login.html';
        }
    });
    
    
    
});
