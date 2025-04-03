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
                console.log("Successfully entered lobby");
            })
            .catch(function (err) {
                console.error("Error entering lobby: ", err.toString());
            });
    },

    leaveLobby: function (lobbyId) {
        if (!window.connection) {
            console.error("SignalR connection is not initialized!");
            return;
        }

        window.connection.invoke("LeaveLobby", UTF8ToString(lobbyId))
            .then(function () {
                console.log("Successful exit from the game");
            })
            .catch(function (err) {
                console.error("Error entering lobby: ", err.toString());
            });
    },

    startGame: function (lobbyId) {
        if (!window.connection) {
            console.error("SignalR connection is not initialized!");
            return;
        }

        window.connection.invoke("StartGame", UTF8ToString(lobbyId))
            .then(function () {
                console.log("Game started");
            })
            .catch(function (err) {
                console.error("Error: ", err.toString());
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