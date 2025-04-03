using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class EncryptionService
{
    private const string Salt = "f7k9p2m4x8q1";
    public static string GenerateGameResultKey(long userId,string lobbyGuid, int score) => ComputeSha256Hash(userId + lobbyGuid + score + Salt);
    private static string ComputeSha256Hash(string data)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(data);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}