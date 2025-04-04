﻿using System;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

using UnityEngine.SceneManagement;

public class AllGamesServer : MonoBehaviour
{
    public static AllGamesServer Instance;

#if UNITY_EDITOR
    public bool connectOnMain = false;
#endif

#if UNITY_EDITOR
    public ReceiveData startData { get; private set; } = new ReceiveData
    {
        chatId = "12345",
        initData = "_",
        startParam = "test",
        userId = 100000
    };
#else
    public ReceiveData startData { get; private set; }
#endif


    public string connectionString =>
#if UNITY_EDITOR
        connectOnMain ? "allgamesapp.ru" : "localhost:7055";
#else
        "allgamesapp.ru";
#endif

    private string cookie;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            return;
        }

        Destroy(gameObject);


    }

    public void Start()
    {
#if UNITY_EDITOR
SceneManager.LoadScene(1);
#endif
    }

    public void Auth(string data)
    {
        startData = JsonUtility.FromJson<ReceiveData>(data);

        StartCoroutine(SendPostRequest("api/v1/Auth", startData.initData, (req) =>
        {
            MultiplayerController.connectToHub();
        }));
    }

    public void ConnectionSuccessful()
    {
        SceneManager.LoadScene(1);
    }

    private IEnumerator SendGetRequest<T>(string url, Action<T> onLoad, Action onError)
    {
        using UnityWebRequest request = UnityWebRequest.Get($"https://{connectionString}/{url}");
        Debug.Log($"https://{connectionString}/{url}");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError ||
        request.result == UnityWebRequest.Result.ProtocolError)

        {
            Debug.LogError(request.error);
            onError?.Invoke();
        }
        else
        {
            onLoad?.Invoke(JsonUtility.FromJson<T>(request.downloadHandler.text));
        }
    }

    private IEnumerator SendPostRequest(string url, string body, Action<UnityWebRequest> onLoad)
    {
        using UnityWebRequest request = new UnityWebRequest($"https://{connectionString}/{url}", "POST");
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "text/plain;charset=UTF-8");
        request.SetRequestHeader("Accept", "*/*");

        //using UnityWebRequest request = UnityWebRequest.Post($"https://{connectionString}/{url}", body);
        //request.SetRequestHeader("Content-Type", "text/plain;charset=UTF-8");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
        request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }

        onLoad?.Invoke(request);
    }


    private IEnumerator SendPostRequest(string url, string body, Action onLoad)
    {
        using UnityWebRequest request = new UnityWebRequest($"https://{connectionString}/{url}", "POST");
        Console.WriteLine(body);
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "*/*");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
        request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }

        onLoad?.Invoke();
    }

    private IEnumerator SendPostRequest(string url, Action onLoad)
    {
        using UnityWebRequest request = UnityWebRequest.Post($"https://{connectionString}/{url}", "");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
        request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }

        onLoad?.Invoke();
    }

    public void GetTopPlayersInfo(string gameName, Action<GameTop> onLoad, Action onError) => StartCoroutine(SendGetRequest($"api/v1/Games/topPlayersInfo/{gameName}", onLoad, onError));
    public void GetLobby(string gameName, string chatId, Action<Lobby> onLoad, Action onError) => StartCoroutine(SendGetRequest($"api/v1/Games/multiplayerLobby?gameName={gameName}&chatId={chatId}", onLoad, onError));
    //public void SendLobbyGameResult(string lobbyId, int score, string messageId, Action onLoad)
    //{
    //    var body = new GameResult
    //    {
    //        score = score,
    //        messageId = messageId,
    //        key = EncryptionService.GenerateGameResultKey(GameManager.LobbyGuid, score)
    //    };
    //    StartCoroutine(SendPostRequest($"api/v1/Games/lobbyGameResult/{lobbyId}", JsonUtility.ToJson(body), onLoad));
    //}
    public void Auth(Action onLoad) => StartCoroutine(SendPostRequest($"api/v1/Auth", onLoad));
}

[Serializable]
public class ReceiveData
{
    public string initData;
    public string startParam;
    public string chatId;
    public long userId;
}