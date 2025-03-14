using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaitingController : MonoBehaviour
{
    [SerializeField] private Sprite[] Design;
    [SerializeField] private Sprite[] Panels;
    [Header("Tabs")]
    [SerializeField] private Transform tabContent;
    [SerializeField] private PlayerNote tabPrefab;


    public void ShowLobbyPlayers(List<PlayerInfo> lobbyPlayers)
    {
        if (lobbyPlayers == null) return;

        ClearTop();

        for (int i = 0; i < lobbyPlayers.Count; i++)
        {
            var player = lobbyPlayers[i];
            var newTab = Instantiate(tabPrefab, tabContent);
            var place = $"{i + 1}{GetPlacePrefix(i + 1)}";

            var panel = i == 0 ? Panels[0] : i == lobbyPlayers.Count - 1 ? Panels[2] : Panels[1];
            var design = i < 3 ? Design[i] : Design[3];

            newTab.InitNote(player.userName, player.score.ToString(), place, panel, design);
        }
    }

    private void ClearTop()
    {
        foreach (Transform tab in tabContent)
        {
            Destroy(tab.gameObject);
        }
    }

    private string GetPlacePrefix(int place) => place switch
    {
        1 => "st",
        2 => "nd",
        3 => "rd",
        _ => "th"
    };


}
