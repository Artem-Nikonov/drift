using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UserProfile : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI BalanceText;
    [SerializeField] private Image UserAvatar;

    private static int balance;
    public static int Balance
    {
        get => balance;
        set => balance = value;
    }

    public void SetBalance(int balance)
    {
        UserProfile.balance = balance;
        BalanceText.text = FormatNumber(balance);
    }

    public void AddBalance(int sum)
    {
        balance += sum;
        BalanceText.text = FormatNumber(balance);
    }

    public void SetAvatar(Texture2D texture)
    {
        Sprite avatarSprite = Sprite.Create(
            texture,                         
            new Rect(0, 0, texture.width, texture.height), 
            new Vector2(0.5f, 0.5f)          
        );

        if (UserAvatar != null)
        {
            UserAvatar.sprite = avatarSprite;
        }
    }

    public void ShowBalance() => BalanceText.text = FormatNumber(balance);

    public static string FormatNumber(double number)
    {
        double absNumber = Math.Abs(number);
        string result;

        if (absNumber >= 1000000000) 
        {
            result = (absNumber / 1000000000).ToString("0.#") + "B";
        }
        else if (absNumber >= 1000000)
        {
            result = (absNumber / 1000000).ToString("0.#") + "M";
        }
        else if (absNumber >= 1000)
        {
            result = (absNumber / 1000).ToString("0.#") + "K";
        }
        else
        {
            result = absNumber.ToString("0");
        }

        return number < 0 ? "-" + result : result;
    }
}
