mergeInto(LibraryManager.library, {

    enterLobby: function (lobbyId) {
        if (!window.connection) {
            console.error("SignalR connection is not initialized!");
            return;
        }

        window.connection.invoke("EnterLobby", UTF8ToString(lobbyId))
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