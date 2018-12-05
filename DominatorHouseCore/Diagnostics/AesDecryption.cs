using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace DominatorHouseCore.Diagnostics
{
    public class AesDecryption
    {
        public static byte[] Decrypt(byte[] bytes, string keyStr)
        {
            var sha256 = new SHA256CryptoServiceProvider();
            var ivBytes = new byte[] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
            var keyBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(keyStr));
            return Decrypt(bytes, keyBytes, ivBytes);
        }

        static byte[] Decrypt(byte[] bytes, byte[] keyBytes, byte[] ivBytes)
        {
            var aes = new AesCryptoServiceProvider
            {
                IV = ivBytes,
                Key = keyBytes
            };

            var transform = aes.CreateDecryptor();
            return transform.TransformFinalBlock(bytes, 0, bytes.Length);
        }

        public static string DecryptAes(string decryptedString)
        {
            var aesKey = ConfigurationManager.AppSettings["AesKey"];
            var decryptedKey = decryptedString.Replace("\\/", "/").Replace("\"","");
            var bytes = Decrypt(Convert.FromBase64String(decryptedKey), aesKey);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}