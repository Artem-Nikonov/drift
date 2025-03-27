mergeInto(LibraryManager.library, {

    connectToHub: function(){
        if (!window.connection) {
            console.error("SignalR connection is not initialized!");
            return;
        }
        window.connection.start().then(function () {
            console.log("Connected to game!");
            SendMessage('MainServer', 'ConnectionSuccessful')
        }).catch(function (err) {
            console.error("Error connecting to SignalR hub: ", err.toString());
        });
    },

    enterLobby: function (lobbyId, carColor) {
        if (!window.connection) {
            console.error("SignalR connection is not initialized!");
            return;
        }

        window.connection.invoke("EnterLobby", UTF8ToString(lobbyId), carColor)
            .then(function () {
                console.log("Successfully entered lobby: " + UTF8ToString(lobbyId));
            })
            .catch(function (err) {
                console.error("Error entering lobby: ", err.toString());
            });
    },

    sendCarTransform: function (lobbyId, carTransformJSON) {
        if (!window.connection) {
            console.error("SignalR connection is not initialized!");
            return;
        }

        window.connection.invoke("SendCarTransform", UTF8ToString(lobbyId), UTF8ToString(carTransformJSON));
    },

    sendGameResult: function (lobbyId, gameResultJSON) {
        if (!window.connection) {
            console.error("SignalR connection is not initialized!");
            return;
        }

        window.connection.invoke("SendGameResult", UTF8ToString(lobbyId), UTF8ToString(gameResultJSON));
    }
});