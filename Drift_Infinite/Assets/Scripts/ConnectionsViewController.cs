using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionsViewController : MonoBehaviour
{

    [SerializeField] private Sprite[] Avatars;
    [Header("Tabs")]
    [SerializeField] private Transform tabContent;
    [SerializeField] private ConnectedPlayerNode tabPrefab;

    private Dictionary<long, GameObject> Nodes = new();

    public void ShowPlayers(List<DrifterInfo> drifters, bool clearContent)
    {
        if (drifters == null || Avatars.Length == 0) return;

        if(clearContent) ClearContent();

        for (int i = 0; i < drifters.Count; i++)
        {
            AddPlayer(drifters[i]);
        }
    }

    public void AddPlayer(DrifterInfo drifter)
    {
        if (Nodes.ContainsKey(drifter.userId)) return;
        var tab = Instantiate(tabPrefab, tabContent);

        var avatar = drifter.carColor < Avatars.Length ? Avatars[drifter.carColor] : Avatars[0];

        tab.InitNote(drifter.userName, avatar);

        Nodes.TryAdd(drifter.userId, tab.gameObject);
    }

    public void RemovePlayer(long userId)
    {
        if(Nodes.TryGetValue(userId, out var node) && node != null)
        {
            Destroy(node);
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
