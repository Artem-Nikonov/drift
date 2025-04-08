using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;

public class ConnectionsViewController : MonoBehaviour
{

    [SerializeField] private Sprite[] Avatars;
    [Header("Tabs")]
    [SerializeField] private Transform tabContent;
    [SerializeField] private ConnectedPlayerNode tabPrefab;

    private Dictionary<long, ConnectedPlayerNode> Nodes = new();

    public void ShowPlayers(List<DrifterInfo> drifters, bool clearContent)
    {
        if (drifters == null || Avatars.Length == 0) return;

        if(clearContent) ClearContent();

        for (int i = 0; i < drifters.Count; i++)
        {
            AddPlayer(drifters[i]);
        }

        foreach(var (id, node) in Nodes)
        {
            LoadAvatar(id, node);
        }

    }

    public void AddPlayer(DrifterInfo drifter, bool shopwAvatar = false)
    {
        if (Nodes.ContainsKey(drifter.userId)) return;
        var node = Instantiate(tabPrefab, tabContent);

        var avatar = drifter.carColor < Avatars.Length ? Avatars[drifter.carColor] : Avatars[0];

        node.InitNote(drifter.userName, avatar);

        if (Nodes.TryAdd(drifter.userId, node) && shopwAvatar)
        {
            LoadAvatar(drifter.userId, node);
        }
    }


    public void LoadAvatar(long userId,ConnectedPlayerNode node)
    {
        AllGamesServer.Instance.GetAvatar(userId, (texture) =>
        {
            node.SetAvatar(texture);
        });
    }

    public void RemovePlayer(long userId)
    {
        if(Nodes.TryGetValue(userId, out var node) && node != null)
        {
            Destroy(node.gameObject);
            Nodes.Remove(userId);
        }
    }

    private void ClearContent()
    {
        foreach (Transform tab in tabContent)
        {
            Destroy(tab.gameObject);
        }

        Nodes.Clear();
    }
}
