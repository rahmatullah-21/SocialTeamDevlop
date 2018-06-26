using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Utility
{
    public class PublisherHelper
    {
        /// <summary>
        /// This method can be used to convert text in Post Description into Hashtags based on Keywords and Minimum Length
        /// </summary>
        /// <param name="postDescription"></param>
        /// <param name="maxHashCount"></param>
        /// <param name="hashKeywords"></param>
        /// <param name="minimumStringLength"></param>
        /// <returns></returns>
        public static string HashByKeywordsAndMinimumLength(string postDescription,int maxHashCount,string hashKeywords,int minimumStringLength)
        {
            string[] discriptionArray = postDescription.Split(' ');
            string[] hashTags = hashKeywords.Split(',');
            int hashCount = 0;
            hashCount = HashByKeyword(maxHashCount, discriptionArray, hashTags, hashCount);
            HashByWordLength(maxHashCount, minimumStringLength, discriptionArray, hashCount);
            postDescription = string.Empty;
            foreach (string item in discriptionArray)
            {
                postDescription += item + " ";
            }
            return postDescription;
        }

        /// <summary>
        /// This method can be used to convert text in Post Description into Hashtags based on string minimum length
        /// </summary>
        /// <param name="maxHashCount"></param>
        /// <param name="postDescription"></param>
        /// <param name="minimumStringLength"></param>
        /// <returns></returns>
        public static string HashByWordLength(int maxHashCount, string postDescription, int minimumStringLength)
        {
            int hashCount = 0;
            string[] discriptionArray = postDescription.Split(' ');
            for (int hash = 0; hash < discriptionArray.Length; hash++)
            {
                if (discriptionArray[hash].Length > minimumStringLength && !discriptionArray[hash].Contains("#"))
                {
                    discriptionArray[hash] = "#" + discriptionArray[hash];
                    hashCount++;
                }
                if (hashCount >= maxHashCount)
                    break;
            }
            postDescription = string.Empty;
            foreach (string item in discriptionArray)
            {
                postDescription += item + " ";
            }
            return postDescription;
        }
        /// <summary>
        /// This method can be used to convert text in Post Description into Hashtags based on keywords
        /// </summary>
        /// <param name="maxHashCount"></param>
        /// <param name="postDescription"></param>
        /// <param name="hashKeywords"></param>
        /// <returns></returns>
        public static string HashByKeyword(int maxHashCount, string postDescription, string hashKeywords)
        {
            int hashCount = 0;
            string[] hashTags = hashKeywords.Split(',');
            string[] discriptionArray = postDescription.Split(' ');
            for (int hash = 0; hash < discriptionArray.Length; hash++)
            {
                if (hashTags.Any(x => x == discriptionArray[hash]))
                {
                    discriptionArray[hash] = "#" + discriptionArray[hash];
                    hashCount++;
                }
                if (hashCount >= maxHashCount)
                    break;
            }
            postDescription = string.Empty;
            foreach (string item in discriptionArray)
            {
                postDescription += item + " ";
            }
            return postDescription;
        }

        private static int HashByWordLength(int maxHashCount, int minimumStringLength, string[] discriptionArray, int hashCount)
        {
            for (int hash = 0; hash < discriptionArray.Length; hash++)
            {
                if (discriptionArray[hash].Length > minimumStringLength && !discriptionArray[hash].Contains("#"))
                {
                    discriptionArray[hash] = "#" + discriptionArray[hash];
                    hashCount++;
                }
                if (hashCount >= maxHashCount)
                    break;
            }

            return hashCount;
        }

        private static int HashByKeyword(int maxHashCount, string[] discriptionArray, string[] hashTags, int hashCount)
        {
            for (int hash = 0; hash < discriptionArray.Length; hash++)
            {
                if (hashTags.Any(x => x == discriptionArray[hash]))
                {
                    discriptionArray[hash] = "#" + discriptionArray[hash];
                    hashCount++;
                }
                if (hashCount >= maxHashCount)
                    break;
            }

            return hashCount;
        }
    }
}
