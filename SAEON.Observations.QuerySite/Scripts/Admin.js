function disableStart() {
    var btn = document.getElementById('btnStart');
    btn.disabled = true;
    btn.innerText = 'Running...';
    var ul = document.getElementById("ulStatus");
    ul.innerText = "";
}

function enableStart() {
    var btn = document.getElementById('btnStart');
    btn.disabled = false;
    btn.innerText = 'Start';
}

function setupHub(hubUrl, messageId) {
    var connection = new signalR.HubConnectionBuilder().withUrl(hubUrl).build();

    connection.on(messageId, function (message) {
        var li = document.createElement("li");
        li.textContent = message;
        document.getElementById("ulStatus").appendChild(li);
        li.scrollIntoView();
        if (message === "Done") {
            enableStart();
        }
    });

    connection
        .start({ withCredentials: false })
        .then(() => console.log('Connection started'))
        .catch(err => console.log('Error while starting connection: ' + err));
}

