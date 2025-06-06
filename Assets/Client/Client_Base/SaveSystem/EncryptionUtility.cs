﻿using System;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// 加解密工具
/// </summary>
public static class EncryptionUtility
{
    private static readonly string key = "aP9s8d7f6g5h4j3k2l1m0n9b8v7c6x5z";

    public static string Encrypt(string plainText)
    {
        try
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.GenerateIV();
                byte[] iv = aes.IV;
                using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    byte[] result = new byte[iv.Length + encryptedBytes.Length];
                    Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                    Buffer.BlockCopy(encryptedBytes, 0, result, iv.Length, encryptedBytes.Length);
                    string base64Str = Convert.ToBase64String(result);
                    LogSystem.Log($"加密结果: {base64Str}");
                    return base64Str;
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"加密失败: {e.Message}");
            return string.Empty;
        }
    }

    public static string Decrypt(string encryptedText)
    {
        if (string.IsNullOrEmpty(encryptedText))
        {
            throw new ArgumentException("密文为空");
        }
        LogSystem.Log($"待解密内容: {encryptedText}");
        try
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                byte[] iv = new byte[aes.BlockSize / 8];
                Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);
                using (var decryptor = aes.CreateDecryptor(aes.Key, iv))
                {
                    byte[] cipherText = new byte[encryptedBytes.Length - iv.Length];
                    Buffer.BlockCopy(encryptedBytes, iv.Length, cipherText, 0, cipherText.Length);
                    byte[] plainBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
                    return Encoding.UTF8.GetString(plainBytes);
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.Error($"解密失败: {e.Message}");
            return string.Empty;
        }
    }
}