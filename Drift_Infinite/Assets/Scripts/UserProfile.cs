using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class UserProfile : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI BalanceText;
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
