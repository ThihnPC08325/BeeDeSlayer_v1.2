using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class EncryptionHelper
{
    private const string Key = "qwertyuiopasdfgh"; // AES key (16, 24, or 32 bytes)
    private const string Iv = "dadeadlineslayer"; // AES IV (16 bytes)

    // Encrypt the password
    public static string EncryptPassword(string plainText)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = Encoding.UTF8.GetBytes(Key);
        aesAlg.IV = Encoding.UTF8.GetBytes(Iv);

        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msEncrypt = new MemoryStream();
        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        {
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }
        }
        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    // Decrypt the password
    public static string DecryptPassword(string cipherText)
    {
        // Kiểm tra nếu dữ liệu có phải là Base64 hợp lệ không
        byte[] cipherBytes;
        try
        {
            cipherBytes = Convert.FromBase64String(cipherText); // Chuyển đổi chuỗi thành mảng byte
        }
        catch (FormatException)
        {
            throw new ArgumentException("Dữ liệu mã hóa không hợp lệ.");
        }

        using Aes aesAlg = Aes.Create();
        byte[] keyBytes = Encoding.UTF8.GetBytes(Key);
        byte[] ivBytes = Encoding.UTF8.GetBytes(Iv);

        // Kiểm tra độ dài của key và iv
        if (keyBytes.Length != 16)
            throw new ArgumentException("Key length must be 24 bytes for AES-192.");
        if (ivBytes.Length != 16)
            throw new ArgumentException("IV length must be 16 bytes.");

        aesAlg.Key = keyBytes;
        aesAlg.IV = ivBytes;

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
        {
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}
