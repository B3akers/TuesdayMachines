using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace TuesdayMachines.Utils
{
    public static class StringExtensions
    {
        public static byte[] HMAC(this string value, string key)
        {
            key = key ?? "";

            using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                return hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(value));
            }
        }

        public static string HashSHA256(this string value)
        {
            var inputBytes = Encoding.UTF8.GetBytes(value);
            var inputHash = SHA256.HashData(inputBytes);
            return Convert.ToHexString(inputHash).ToLower();
        }

        public static string Decrypt(this string value, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                var data = Convert.FromBase64String(value);

                byte[] iv = new byte[16];
                Buffer.BlockCopy(data, 0, iv, 0, iv.Length);

                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(data, iv.Length, data.Length - iv.Length))
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

        public static string Encrypt(this string value, string key)
        {
            byte[] final;
            using (Aes aesAlg = Aes.Create())
            {
                byte[] encrypted;

                aesAlg.Key = Encoding.UTF8.GetBytes(key);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(value);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
                var iv = aesAlg.IV;

                final = new byte[encrypted.Length + iv.Length];
                Buffer.BlockCopy(iv, 0, final, 0, iv.Length);
                Buffer.BlockCopy(encrypted, 0, final, iv.Length, encrypted.Length);
            }

            return Convert.ToBase64String(final);
        }
    }
}
