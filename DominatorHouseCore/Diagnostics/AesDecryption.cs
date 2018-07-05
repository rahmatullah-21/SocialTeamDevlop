using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace DominatorHouseCore.Diagnostics
{
    public class AesDecryption
    {
        public static byte[] Decrypt(byte[] bytes, string keyStr)
        {
            var sha256 = new SHA256CryptoServiceProvider();
            var ivBytes = new byte[16] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
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
            var decryptedKey = decryptedString.Replace("\\/", "/").Replace("\"","");
            byte[] bytes = Decrypt(Convert.FromBase64String(decryptedKey), "3sc3RLrpd17");
            return Encoding.UTF8.GetString(bytes);
        }
    }
}