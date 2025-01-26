function updateUIAfterAuth(isLoggedIn) {
    const profileButton = document.querySelector('.profile-button button:nth-child(1)');
    const registerButton = document.querySelector('.profile-button button:nth-child(2)');
    const loginButton = document.querySelector('.profile-button button:nth-child(3)');

    if (isLoggedIn) {
        profileButton.style.display = 'inline'; 
        registerButton.style.display = 'none';  
        loginButton.style.display = 'none';     
    } else {
        profileButton.style.display = 'none';   
        registerButton.style.display = 'inline'; 
        loginButton.style.display = 'inline';   
    }
}
function uploadToStorage() {
    const fileName = prompt("Введите имя файла для сохранения:", "input.txt"); 
    const text = document.getElementById("inputText").value;

    if (!fileName || !text) {
        alert("Имя файла и текст не могут быть пустыми!");
        return;
    }

    fetch("http://localhost:5000/api/storage/upload-text", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({
            fileName: fileName,
            text: text
        }),
    })
        .then(response => {
            if (response.ok) {
                alert(`Текст успешно загружен в файл: ${fileName}`);
            } else {
                return response.text().then(err => {
                    throw new Error(err);
                });
            }
        })
        .catch(error => {
            alert("Ошибка загрузки: " + error.message);
        });
}

function downloadFromStorage() {
    const fileName = prompt("Введите имя файла для загрузки:", "input.txt");

    if (!fileName) {
        alert("Имя файла не может быть пустым!");
        return;
    }

    fetch(`http://localhost:5000/api/storage/download-text/${fileName}`, {
        method: "GET",
    })
        .then(response => {
            if (response.ok) {
                return response.text();
            } else {
                return response.text().then(err => {
                    throw new Error(err);
                });
            }
        })
        .then(text => {
            document.getElementById("inputText").value = text;
            alert(`Текст из файла "${fileName}" успешно загружен в поле ввода.`);
        })
        .catch(error => {
            alert("Ошибка загрузки: " + error.message);
        });
}


window.onload = function () {
    const username = getCookie("username");

    if (username) {
        updateUIAfterAuth(true);
    } else {
        updateUIAfterAuth(false);
    }
};

function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
    return null;
}
function registerUser() {
    const userName = document.getElementById('registerUsername').value;
    const password = document.getElementById('registerPassword').value;

    fetch('http://localhost:5000/api/auth/register', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            UserName: userName,
            Password: password
        }),
    })
        .then(response => {
            if (response.ok) {
                alert('Регистрация успешна!');
                closeForm('registerForm');
            } else {
                response.text().then(err => alert(err));
            }
        })
        .catch(error => {
            alert('Ошибка при регистрации: ' + error.message);
        });
}

function loginUser() {
    const userName = document.getElementById('loginUsername').value;
    const password = document.getElementById('loginPassword').value;

    fetch('http://localhost:5000/api/auth/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            UserName: userName,
            Password: password
        }),
    })
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                return response.text().then(err => {
                    throw new Error(err);
                });
            }
        })
        .then(data => {
            document.cookie = `username=${data.username}; path=/;`;

            alert(`Добро пожаловать, ${data.username}!`);
            updateUIAfterAuth(true);
            closeForm('loginForm');
        })
        .catch(error => {
            alert('Ошибка при авторизации: ' + error.message);
        });
}

function openProfile() {
    fetch('http://localhost:5000/api/auth/check-auth', {
        method: 'GET',
        credentials: 'include', 
    })
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error('Пользователь не авторизован');
            }
        })
        .then(data => {
            document.getElementById('profileUsername').textContent = `Имя пользователя: ${data.userName}`;
            document.getElementById('profileModal').style.display = 'flex';
        })
        .catch(error => {
            alert(error.message);
        });
}

function closeProfile() {
    document.getElementById('profileModal').style.display = 'none';
}

function logoutUser() {
    fetch('http://localhost:5000/api/auth/logout', {
        method: 'POST',
        credentials: 'include', 
    })
        .then(response => {
            if (response.ok) {
                updateUIAfterAuth(false);
                closeProfile();
                alert('Вы успешно вышли');
            } else {
                throw new Error('Ошибка при выходе');
            }
        })
        .catch(error => {
            alert(error.message);
        });
}

function openRegister() {
    document.getElementById('registerForm').style.display = 'block';
    document.getElementById('loginForm').style.display = 'none';
}

function openLogin() {
    document.getElementById('loginForm').style.display = 'block';
    document.getElementById('registerForm').style.display = 'none';
}

function closeForm(formId) {
    document.getElementById(formId).style.display = 'none';
}

window.onload = function () {
    fetch('http://localhost:5000/api/auth/check-auth', {
        method: 'GET',
        credentials: 'include', 
    })
        .then(response => {
            if (response.ok) {
                updateUIAfterAuth(true);
            } else {
                updateUIAfterAuth(false); 
            }
        })
        .catch(error => {
            console.error('Ошибка при проверке аутентификации:', error);
        });
};

function transferText() {
    const inputText = document.getElementById('inputText').value;

    fetch('http://localhost:5000/api/Text/process', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ text: inputText }),
    })
        .then(response => {
            if (!response.ok) {
                return response.text().then(err => {
                    throw new Error(err);
                });
            }
            return response.json();
        })
        .then(data => {
            const outputText = document.getElementById('outputText');
            outputText.textContent = data.output;
        })
        .catch(error => {
            alert('Ошибка: ' + error.message);
        });
}

function loadFile() {
    const input = document.createElement('input');
    input.type = 'file';
    input.accept = 'text/plain';
    input.onchange = (event) => {
        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = (e) => {
                document.getElementById('inputText').value = e.target.result;
            };
            reader.readAsText(file);
        }
    };
    input.click();
}

function saveFile() {
    const text = document.getElementById('outputText').textContent;
    const blob = new Blob([text], { type: 'text/plain' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = 'output.txt';
    link.click();
}