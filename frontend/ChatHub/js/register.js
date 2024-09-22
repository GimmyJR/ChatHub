document.getElementById('registerForm').addEventListener('submit', async function (event) {
    event.preventDefault(); 

    const formData = new FormData();
    const name = document.getElementById('name').value;
    const file = document.getElementById('avatar').files[0]; 

    formData.append('Name', name); // Match the case with your RegisterDto
    formData.append('Avatar', file); // Match the case with your RegisterDto

    console.log([...formData.entries()]); 

    try {
        const response = await fetch('http://localhost:50267/api/account/register', {
            method: 'POST',
            body: formData
        });

        if (!response.ok) {
            const errorData = await response.json();
            alert(`Error: ${errorData.Nessage}`);
        } else {
            alert('Registration successful!');
            window.location.href = '/login.html';
        }
    } catch (error) {
        console.error('Error during registration:', error);
        alert('An error occurred during registration');
    }
});
