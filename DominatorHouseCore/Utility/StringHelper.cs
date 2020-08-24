#region

using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace DominatorHouseCore.Utility
{
    [Localizable(false)]
    public static class StringHelper
    {
        public static string Base64Decode(this string base64EncodedData)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedData));
        }

        public static string Base64Encode(this string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }

        public static string GenerateGuid(bool keepDashes = true)
        {
            var str = Guid.NewGuid().ToString();
            if (!keepDashes)
                return str.Replace('-', char.MinValue);
            return str;
        }

        public static string GetRegexPatern(string patern, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            var match = Regex.Match(value, patern);
            if (!match.Success)
                return null;
            return match.Value;
        }

        public static byte[] GetSha256Raw(string randomString, byte[] key = null)
        {
            using (var hmacshA256 = key == null ? new HMACSHA256() : new HMACSHA256(key))
            {
                hmacshA256.ComputeHash(Encoding.UTF8.GetBytes(randomString));
                return hmacshA256.Hash;
            }
        }

        public static bool IsValidJson(this string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput))
                return false;
            strInput = strInput.Trim();
            if ((!strInput.StartsWith("{") || !strInput.EndsWith("}")) &&
                (!strInput.StartsWith("[") || !strInput.EndsWith("]")))
                return false;
            try
            {
                JToken.Parse(strInput);
                return true;
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        private static string ByteToString(byte[] buff)
        {
            return buff.Aggregate(string.Empty, (current, t) => current + t.ToString("X2"));
        }
    }
}