document.getElementById('loginForm').addEventListener('submit', async function (event) {
    event.preventDefault(); 

    const name = document.getElementById('name').value;

    try {
        const response = await fetch('http://localhost:50267/api/account/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ name: name }) 
        });

        if (!response.ok) {
            const errorData = await response.json();
            console.error('Server Error:', errorData);
            alert(`Error: ${errorData.Message}`);
        } else {
            const user = await response.json();
            alert(`Welcome, ${user.name}!`);
            
            // Store user details in localStorage
            localStorage.setItem("accessToken", JSON.stringify(user));

            // Redirect to the home page after login
            window.location.href = '/home.html'; // Adjust the path if necessary
        }
    } catch (error) {
        console.error('Fetch Error:', error);
        alert('An error occurred during login');
    }
});
