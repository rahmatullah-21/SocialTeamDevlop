using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows;
using DominatorHouseCore.LogHelper;

namespace DominatorHouseCore.Utility
{
    public static class Utilities
    {

        /// <summary>
        /// GetMobileDeviceId is used to get the mobile device id with 16 Digits
        /// </summary>
        /// <returns>return the 16 digit unique mobile device ID</returns>
        public static string GetMobileDeviceId(string Guid = "")
        {
            // Collect the random inputString with five character, convert those character to byte array with help of the MD5
            return "android-" + (String.IsNullOrEmpty(Guid)?RandomUtilties.GetRandomString(5).GetHexFromString().Substring(0, 16):Guid);
        }


        /// <summary>
        /// GetHexFromString is used to get the hexadecimal value of the given input inputString
        /// </summary>
        /// <param name="inputString">inputString which is convert into hexadecimal</param>
        /// <returns>Required Hexa decimal value from the inputString</returns>
        public static string GetHexFromString(this string inputString)
        {
            //Validate the input whether is null or not 
            if (inputString == null)
                throw new ArgumentNullException(nameof(inputString));

            // Convert the input values to hexa
            using (var md5 = MD5.Create())
            {
                // Read the bytes values form the input string
                var bytes = Encoding.UTF8.GetBytes(inputString);

                //Compute the hash values of bytes with the help of MD5 then convert those to base datatype(here string),
                //finally convert the string to lower
                return BitConverter.ToString(md5.ComputeHash(bytes)).Replace("-", String.Empty).ToLower();
            }
        }


        /// <summary>
        /// GetGuid is used to get the GUID values
        /// </summary>
        /// <param name="isDashesNeed">This parameter is used to decide whether keep the dashes in the GUID or not</param>
        /// <returns>Return the GUID</returns>
        public static string GetGuid(bool isDashesNeed = true)
        {
            // Generate the GUID 
            var getGuid = Guid.NewGuid().ToString();
            // return the GUID without dashes if isDashesNeed is true 
            return !isDashesNeed ? getGuid.Replace('-', Char.MinValue) : getGuid;
        }


        /// <summary>
        /// ValidJsonChecker is used to verify the inputstring is valid json or not
        /// </summary>
        /// <param name="inputJsonValue">Pass the string which is going to check valid json</param>
        /// <returns>return true if the given input is parsed successfully, else return false</returns>
        public static bool ValidJsonChecker(this string inputJsonValue)
        {
            // Validate the input contains null or whitespace
            if (String.IsNullOrWhiteSpace(inputJsonValue))
                return false;

            // Remove the leading and trailing character from the input
            inputJsonValue = inputJsonValue.Trim();

            // If the given input is not started with {,},[,] then it will return false
            if ((!inputJsonValue.StartsWith("{") || !inputJsonValue.EndsWith("}"))
                && (!inputJsonValue.StartsWith("[") || !inputJsonValue.EndsWith("]")))
                return false;

            try
            {
                // try to parse the inputstring if successfully parsed it will return true. else false
                JToken.Parse(inputJsonValue);
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

        /// <summary>
        /// Get the text from source string Between two pattern of characters
        /// </summary>
        /// <param name="strSource"></param>
        /// <param name="strStart"></param>
        /// <param name="strEnd"></param>
        /// <returns></returns>
        public static string GetBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }


      /// <summary>
      /// Calculates percentage
      /// </summary>
      /// <param name="value"></param>
      /// <param name="percentage"></param>
      /// <returns></returns>
        public static int PercentageCalculator(int value, int percentage)
        {
          return (value * percentage) / 100;
        }

        // Returns string from resource dictionary
        public static string FromResourceDictionary(this string resourceDictionaryKey)
            => Application.Current?.FindResource(resourceDictionaryKey)?.ToString() ?? resourceDictionaryKey;



        public static void ExportReports(string fileName, string csvHeader, List<string> csvData)
        {
            using (var streamWriter = new StreamWriter(fileName, true))
                streamWriter.WriteLine(csvHeader);
            try
            {
                foreach (var item in csvData)
                {
                    using (var streamWriter = new StreamWriter(fileName, true))
                    {
                        streamWriter.WriteLine(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            GlobusLogHelper.log.Info("Data has been exported successfully");
        }


        public static string GetUrlFormPostData(object obj)
        {
            string urlFormData = String.Empty;
            string serializedPostData = JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(serializedPostData);

            foreach (var dictKey in dict.Keys)
            {
                urlFormData += (urlFormData == String.Empty ? String.Empty : "&") + dictKey + "=" + dict[dictKey];
            }
            return urlFormData;
        }
    }
}
