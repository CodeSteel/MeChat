var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();
var chatbox = document.getElementById("chatbox");


connection.on("ReceiveMessage", function (username, userId, createdAt, message) {
    var item = document.createElement("p");
    item.className = "flex items-center space-x-1"
    
    var dateSpan = document.createElement("span");
    dateSpan.className = "text-true-gray-400 text-sm";
    dateSpan.textContent = createdAt;

    item.appendChild(dateSpan);

    var link = document.createElement("a");
    link.className = "flex items-center space-x-2";
    link.href = `/account/profile?userId=${userId}`
    
    var imgSpan = document.createElement("span");
    var img = document.createElement("img");
    img.src = "favicon.ico";
    img.className = "ml-2";
    img.width = "16";
    img.height = "16";
    imgSpan.appendChild(img);
    
    link.appendChild(imgSpan);
    
    var nameSpan = document.createElement("span");
    nameSpan.className = "text-lg font-bold";
    nameSpan.textContent = username + ": ";
    
    link.appendChild(nameSpan);
    item.appendChild(link);

    var bodySpan = document.createElement("span");
    bodySpan.className = "text-md";
    bodySpan.textContent = message;
    
    item.appendChild(bodySpan);
    
    chatbox.appendChild(item);
    chatbox.scrollTop = chatbox.scrollHeight;
});

connection.start();

document.getElementById("sendMessage").addEventListener("click", function (event) {
    var messageInput = document.getElementById("messageInput");
    
    connection.invoke("Send", messageInput.value);
    event.preventDefault();
    messageInput.value = "";
});

document.onkeydown = keyPress;

function keyPress(e){
    var x = e || window.event;
    var key = (x.keyCode || x.which);
    if(key == 13 && key != 16){
        document.getElementById("sendMessage").click();
    }
}

chatbox.scrollTop = chatbox.scrollHeight;

document.getElementById("messageInput").focus();

