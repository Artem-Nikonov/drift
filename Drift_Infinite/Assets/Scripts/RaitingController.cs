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
    public static RaitingController Instance;
    // Start is called before the first frame update

    public void Awake()
    {
        Instance = this;
    }

    public void ShowLobbyPlayers(List<PlayerInfo> lobbyPlayers)
    {
        if (lobbyPlayers == null) return;

        ClearTop();
        for (int i = 0; i < lobbyPlayers.Count; i++)
        {
            var player = lobbyPlayers[i];
            var newTab = Instantiate(tabPrefab, tabContent);
            var place = $"{i + 1}{GetPlacePrefix(i + 1)}";
            var panel = Panels[1];
            var design = Design[3];

            if (i == 0)
            {
                panel = Panels[0];
                design = Design[0];
            }
            else if (i == 1)
            {
                design = Design[1];
            }
            else if (i == 2)
            {
                design = Design[2];
            }
            else if (i == lobbyPlayers.Count)
            {
                panel = Panels[2];
            }


            newTab.InitNote(player.userName, player.score.ToString(), place, panel, design);
        }
    }

    private void ClearTop()
    {
        Debug.Log(tabContent.transform == null);
        //foreach (Transform tab in tabContent.transform)
        //    Destroy(tab.gameObject);
    }


    private string GetPlacePrefix(int place) => place switch
    {
        1 => "st",
        2 => "nd",
        3 => "rd",
        _ => "th"
    };


}
