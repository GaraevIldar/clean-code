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

function openProfile() {
    alert('Открытие профиля...');
}
