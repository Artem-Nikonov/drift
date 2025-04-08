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

    public void SetAvatar(Texture2D texture)
    {
        Sprite avatarSprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );

        if(Avatar != null)
            Avatar.sprite = avatarSprite;

    }
}
