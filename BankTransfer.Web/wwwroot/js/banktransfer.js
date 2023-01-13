"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/banktransferhub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

let bankTransferUrl = "https://localhost:7121/api/v1/core-banking/bankTransfer";

let makeRequest = document.getElementById("makeRequest");

let initialResponse = document.getElementById("initialResponse");

let asyncResponse = document.getElementById("asyncResponse");

connection.on("ReceiveMessage", function (valueToSend) {
    console.log("Hello World");
    console.log(`Json Parsed::: ${valueToSend}`);

    var textAreaContent = document.getElementById("messages");

    textAreaContent.textContent = valueToSend;
});


//Make api Request to get first Response
//Get Value for the various fields

function MakeBankTransferRequest(endpoint) {
    fetch(endpoint, {
        method: 'POST',
        headers: {
            'ApiKey': '98f3618cf3msh703c3a788391b78p11cf19jsn5786aa86a64c',
            'Accept': 'application/json',
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            Provider: document.getElementById("providerInput").value,
            BeneficiaryBankCode: document.getElementById("beneficiaryBankCodeInput").value,
            BeneficiaryAccountNumber: document.getElementById("beneficiaryAccountNumber").value,
            Amount: document.getElementById("amountInput").value
        })
    })
    .then(res => res.json())
        .then(data => {
            console.log(data);
        })
        .catch(err => {
            console.log(err);
        })

}

function handleBankTransfer(e) {
    e.preventDefault();
    MakeBankTransferRequest(bankTransferUrl);
}

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

makeRequest.addEventListener("click", handleBankTransfer);
