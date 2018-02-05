using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public static bool CheckRegexPatern(string patern, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return Regex.Match(value, patern).Success;
            return false;
        }

        public static string ConvertThreeLetterCountryCodeToTwoLetterCountryCode(this string threeLetterCoutryCode)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
      {
        {
          "AFG",
          "AF"
        },
        {
          "ALB",
          "AL"
        },
        {
          "ARE",
          "AE"
        },
        {
          "ARG",
          "AR"
        },
        {
          "ARM",
          "AM"
        },
        {
          "AUS",
          "AU"
        },
        {
          "AUT",
          "AT"
        },
        {
          "AZE",
          "AZ"
        },
        {
          "BEL",
          "BE"
        },
        {
          "BGD",
          "BD"
        },
        {
          "BGR",
          "BG"
        },
        {
          "BHR",
          "BH"
        },
        {
          "BIH",
          "BA"
        },
        {
          "BLR",
          "BY"
        },
        {
          "BLZ",
          "BZ"
        },
        {
          "BOL",
          "BO"
        },
        {
          "BRA",
          "BR"
        },
        {
          "BRN",
          "BN"
        },
        {
          "CAN",
          "CA"
        },
        {
          "CHE",
          "CH"
        },
        {
          "CHL",
          "CL"
        },
        {
          "CHN",
          "CN"
        },
        {
          "COL",
          "CO"
        },
        {
          "CRI",
          "CR"
        },
        {
          "CZE",
          "CZ"
        },
        {
          "DEU",
          "DE"
        },
        {
          "DNK",
          "DK"
        },
        {
          "DOM",
          "DO"
        },
        {
          "DZA",
          "DZ"
        },
        {
          "ECU",
          "EC"
        },
        {
          "EGY",
          "EG"
        },
        {
          "ESP",
          "ES"
        },
        {
          "EST",
          "EE"
        },
        {
          "ETH",
          "ET"
        },
        {
          "FIN",
          "FI"
        },
        {
          "FRA",
          "FR"
        },
        {
          "FRO",
          "FO"
        },
        {
          "GBR",
          "GB"
        },
        {
          "GEO",
          "GE"
        },
        {
          "GRC",
          "GR"
        },
        {
          "GRL",
          "GL"
        },
        {
          "GTM",
          "GT"
        },
        {
          "HKG",
          "HK"
        },
        {
          "HND",
          "HN"
        },
        {
          "HRV",
          "HR"
        },
        {
          "HUN",
          "HU"
        },
        {
          "IDN",
          "ID"
        },
        {
          "IND",
          "IN"
        },
        {
          "IRL",
          "IE"
        },
        {
          "IRN",
          "IR"
        },
        {
          "IRQ",
          "IQ"
        },
        {
          "ISL",
          "IS"
        },
        {
          "ISR",
          "IL"
        },
        {
          "ITA",
          "IT"
        },
        {
          "JAM",
          "JM"
        },
        {
          "JOR",
          "JO"
        },
        {
          "JPN",
          "JP"
        },
        {
          "KAZ",
          "KZ"
        },
        {
          "KEN",
          "KE"
        },
        {
          "KGZ",
          "KG"
        },
        {
          "KHM",
          "KH"
        },
        {
          "KOR",
          "KR"
        },
        {
          "KWT",
          "KW"
        },
        {
          "LAO",
          "LA"
        },
        {
          "LBN",
          "LB"
        },
        {
          "LBY",
          "LY"
        },
        {
          "LIE",
          "LI"
        },
        {
          "LKA",
          "LK"
        },
        {
          "LTU",
          "LT"
        },
        {
          "LUX",
          "LU"
        },
        {
          "LVA",
          "LV"
        },
        {
          "MAC",
          "MO"
        },
        {
          "MAR",
          "MA"
        },
        {
          "MCO",
          "MC"
        },
        {
          "MDV",
          "MV"
        },
        {
          "MEX",
          "MX"
        },
        {
          "MKD",
          "MK"
        },
        {
          "MLT",
          "MT"
        },
        {
          "MNE",
          "ME"
        },
        {
          "MNG",
          "MN"
        },
        {
          "MYS",
          "MY"
        },
        {
          "NGA",
          "NG"
        },
        {
          "NIC",
          "NI"
        },
        {
          "NLD",
          "NL"
        },
        {
          "NOR",
          "NO"
        },
        {
          "NPL",
          "NP"
        },
        {
          "NZL",
          "NZ"
        },
        {
          "OMN",
          "OM"
        },
        {
          "PAK",
          "PK"
        },
        {
          "PAN",
          "PA"
        },
        {
          "PER",
          "PE"
        },
        {
          "PHL",
          "PH"
        },
        {
          "POL",
          "PL"
        },
        {
          "PRI",
          "PR"
        },
        {
          "PRT",
          "PT"
        },
        {
          "PRY",
          "PY"
        },
        {
          "QAT",
          "QA"
        },
        {
          "ROU",
          "RO"
        },
        {
          "RUS",
          "RU"
        },
        {
          "RWA",
          "RW"
        },
        {
          "SAU",
          "SA"
        },
        {
          "SCG",
          "CS"
        },
        {
          "SEN",
          "SN"
        },
        {
          "SGP",
          "SG"
        },
        {
          "SLV",
          "SV"
        },
        {
          "SRB",
          "RS"
        },
        {
          "SVK",
          "SK"
        },
        {
          "SVN",
          "SI"
        },
        {
          "SWE",
          "SE"
        },
        {
          "SYR",
          "SY"
        },
        {
          "TAJ",
          "TJ"
        },
        {
          "THA",
          "TH"
        },
        {
          "TKM",
          "TM"
        },
        {
          "TTO",
          "TT"
        },
        {
          "TUN",
          "TN"
        },
        {
          "TUR",
          "TR"
        },
        {
          "TWN",
          "TW"
        },
        {
          "UKR",
          "UA"
        },
        {
          "URY",
          "UY"
        },
        {
          "USA",
          "US"
        },
        {
          "UZB",
          "UZ"
        },
        {
          "VEN",
          "VE"
        },
        {
          "VNM",
          "VN"
        },
        {
          "YEM",
          "YE"
        },
        {
          "ZAF",
          "ZA"
        },
        {
          "ZWE",
          "ZW"
        }
      };
            if (!dictionary.ContainsKey(threeLetterCoutryCode))
                return (string)null;
            return dictionary[threeLetterCoutryCode];
        }

        public static string GenerateGuid(bool keepDashes = true)
        {
            string str = Guid.NewGuid().ToString();
            if (!keepDashes)
                return str.Replace('-', char.MinValue);
            return str;
        }

        public static string GetCoutryFromtwoLetterCountryCode(this string countryCode)
        {
            return new RegionInfo(countryCode).EnglishName;
        }

        public static string GetMd5(this Stream stream)
        {
            using (MD5 md5 = MD5.Create())
                return Encoding.Default.GetString(md5.ComputeHash(stream)).Replace("-", string.Empty).ToLower();
        }

        public static string GetMd5(this string @string)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(@string);
                return BitConverter.ToString(md5.ComputeHash(bytes)).Replace("-", string.Empty).ToLower();
            }
        }

        public static string GetRegexPatern(string patern, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return (string)null;
            Match match = Regex.Match(value, patern);
            if (!match.Success)
                return (string)null;
            return match.Value;
        }

        public static MatchCollection GetRegexPaternGroups(string patern, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return Regex.Matches(value, patern);
            return (MatchCollection)null;
        }

        public static string GetSha256(string randomString, byte[] key = null)
        {
            return StringHelper.ByteToString(StringHelper.GetSha256Raw(randomString, key)).ToLower();
        }

        public static byte[] GetSha256Raw(string randomString, byte[] key = null)
        {
            using (HMACSHA256 hmacshA256 = key == null ? new HMACSHA256() : new HMACSHA256(key))
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
            if ((!strInput.StartsWith("{") || !strInput.EndsWith("}")) && (!strInput.StartsWith("[") || !strInput.EndsWith("]")))
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

        public static string RemoveLineEndings(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            char ch = '\x2028';
            string oldValue1 = ch.ToString();
            ch = '\x2029';
            string oldValue2 = ch.ToString();
            return value.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(oldValue1, string.Empty).Replace(oldValue2, string.Empty);
        }

        public static string ReplaceFirstOccurrence(this string source, string find, string replace)
        {
            int startIndex = source.IndexOf(find, StringComparison.Ordinal);
            return source.Remove(startIndex, find.Length).Insert(startIndex, replace);
        }

        public static string ReplacePaternBlank(string patern, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;
            return new Regex(patern).Replace(value, string.Empty);
        }

        //public static string SpinIt(this string str)
        //{
        //    for (Match match = Regex.Match(str, "{[^{}]*}"); match.Success; match = Regex.Match(str, "{[^{}]*}"))
        //    {
        //        string[] strArray = str.Substring(match.Index + 1, match.Length - 2).Split('|');
        //        str = str.Substring(0, match.Index) + strArray[RandomUtilties.GetRandomNumber(strArray.Length - 1, 0)] + str.Substring(match.Index + match.Length);
        //    }
        //    return str;
        //}

        private static string ByteToString(byte[] buff)
        {
            return ((IEnumerable<byte>)buff).Aggregate<byte, string>(string.Empty, (Func<string, byte, string>)((current, t) => current + t.ToString("X2")));
        }
    }
}
