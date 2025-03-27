using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ConnectedPlayerNode : MonoBehaviour
{
    public TextMeshProUGUI UserName;
    public Image Avatar;

    public void InitNote(string userName, Sprite avatarSprite)
    {
        UserName.text = userName;
        Avatar.sprite = avatarSprite;

    }
}
