using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNote : MonoBehaviour
{
    public TextMeshProUGUI UserName;
    public TextMeshProUGUI Score;
    public TextMeshProUGUI Place;
    public Image Panel;
    public Image Design;

    //public void InitNote(string userName, string score, Sprite sprite)
    //{
    //    UserName.text = userName;
    //    Score.text = score;
    //    Place.sprite = sprite;
    //}
    public void InitNote(string userName, string score, string place, Sprite panel, Sprite design)
    {
        UserName.text = userName;
        Score.text = score;
        Place.text = place;
        Panel.sprite = panel;
        Design.sprite = design;

    }

    
}
