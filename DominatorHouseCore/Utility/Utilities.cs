using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
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
            return "android-" + (String.IsNullOrEmpty(Guid) ? RandomUtilties.GetRandomString(5).GetHexFromString().Substring(0, 16) : Guid);
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
            try
            {
                if (strSource.Contains(strStart) && strSource.Contains(strEnd))
                {
                    var start = strSource.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length;
                    var end = strSource.IndexOf(strEnd, start, StringComparison.Ordinal);

                    if (end < 0 || start < 0)
                        return string.Empty;

                    return strSource.Substring(start, end - start);
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
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
        {
            try
            {
                var lang = Application.Current?.FindResource(resourceDictionaryKey)?.ToString() ?? resourceDictionaryKey;
                return lang;
            }
            catch (Exception)
            {
                var keySubstring = resourceDictionaryKey.Substring("LangKey".Length);
                return Regex.Replace(keySubstring, "(\\B[A-Z])", " $1");
            }
        }


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

        /// <summary>
        /// Remove the url from given text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveUrls(string text) =>
            Regex.Replace(text, @"\b(?:https?://|www\.)\S+\b", string.Empty).Trim();

        /// <summary>
        /// Replace the url with their shorten url
        /// </summary>
        /// <param name="text">text</param>
        /// <returns></returns>
        public static string ReplaceWithShortenUrl(string text)
        {
            // pattern
            var linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            // call to replace evaluator
            return linkParser.Replace(text, ReplaceMatchEvaluator);
        }

        /// <summary>
        /// Apply the shorten url for all matches
        /// </summary>
        /// <param name="match"><see cref="Match"/> match objects</param>
        /// <returns></returns>
        public static string ReplaceMatchEvaluator(Match match) => Shorten(match.Value);

        /// <summary>
        /// Get a shorten url for give url
        /// </summary>
        /// <param name="longUrl"></param>
        /// <returns></returns>
        public static string Shorten(string longUrl)
        {
            if (string.IsNullOrEmpty(longUrl))
                return longUrl;

            // get the Bitly login and apikey value
            var login = ConstantVariable.BitlyLogin;
            var apikey = ConstantVariable.BitlyApiKey;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(apikey))
                return longUrl;

            // base url
            var url = $"http://api.bit.ly/shorten?format=json&version=2.0.1&longUrl={HttpUtility.UrlEncode(longUrl)}&login={login}&apiKey={apikey}";

            // make a get requests
            var request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                var response = request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        // Hitting the bitly url and fetching the shorten url for given long url
                        var reader = new StreamReader(responseStream, Encoding.UTF8);
                        dynamic jsonResponse = JsonConvert.DeserializeObject(reader.ReadToEnd());
                        // Fetch the short url from api
                        string shortUrl = jsonResponse["results"][longUrl]["shortUrl"];
                        return shortUrl;
                    }
                }
            }
            catch (WebException)
            {
                return longUrl;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return longUrl;
        }


        /// <summary>
        /// Extract integer only value from string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>

        public static string GetIntegerOnlyString(string data)
        {
            if (data.Contains("null"))
                return "0";

            return Regex.Replace(data, "[^0-9]+", string.Empty);
        }


        public static string FirstMatchExtractor(string decodedResponse, string pattern)
        {
            var match = Regex.Matches(decodedResponse, pattern, RegexOptions.Singleline);
            return match.Count > 0 ? match[0].Groups[1].ToString() : string.Empty;
        }

        public static bool DownloadNotFound()
        {
            var webclient = new WebClient();
            webclient.DownloadFile("https://cdn.browshot.com/static/images/not-found.png", $"{ConstantVariable.GetNotFoundImage()}");
            return true;
        }

        public static bool DownloadSocinatorIcon()
        {
            var webclient = new WebClient();
            webclient.DownloadFile("https://socinator.com/wp-content/uploads/2018/07/fav_64.png", $"{ConstantVariable.GetSocinatorIcon()}");
            return true;
        }
        public static string ReplaceUniCode(string messeges)
        {
            Regex _regex = new Regex(@"\\u(?<Value>[a-zA-Z0-9]{4})", RegexOptions.Compiled);
            messeges = _regex.Replace(messeges,
                m => ((char)int.Parse(m.Groups["Value"].Value, System.Globalization.NumberStyles.HexNumber)).ToString()
            );
            return messeges;
        }
    }
}
