using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DominatorHouseCore.Utility
{
    public static class SpinTexHelper
    {
        /// <summary>
        /// Get a single text from collection spin text
        /// </summary>
        /// <param name="content">text sources</param>
        /// <returns></returns>
        public static string GetSpinText(string content)
        {
            // pattern for spin text
            const string pattern = @"\(([^)]*)\)";

            // Check whether pattern are present in given content
            var match = Regex.Match(content, pattern);
            if (match.Success)
            {
                // call the spin text generator to fetch all posts
                var spintexCollection = GetSpinMessageCollection(content);

                // select a random spin text and return an item
                var randomNumber = RandomUtilties.GetRandomNumber(spintexCollection.Count - 1);
                return spintexCollection[randomNumber];
            }
            return content;
        }

        /// <summary>
        /// Get the spin text collections from text 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static List<string> GetSpinMessageCollection(string content)
        {
            try
            {
                // if given content length is lesser than 150 character
                if (content.Length <= 150)
                    return GetSpinnedTexts(content);

                var spinnedList = new List<string>();

                // Iterate until getting 30 unique texts
                while (spinnedList.Count < 30)
                {
                    var spinnedText = SpinText(new Random(), content);
                    if (spinnedList.Contains(spinnedText))
                        continue;
                    spinnedList.Add(spinnedText);
                }
                // return 30 unique spin text
                return spinnedList;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return new List<string>();
        }

        /// <summary>
        /// Generate a spin for vast amount of text as input
        /// </summary>
        /// <param name="random">random objects</param>
        /// <param name="text">input text</param>
        /// <returns></returns>
        private static string SpinText(Random random, string text)
        {
            // pattern
            const string pattern = @"\(([^)]*)\)";
            var match = Regex.Match(text, pattern);
            // Iterate untill match success
            while (match.Success)
            {
                // Get random choice and replace pattern match.
                var segments = text.Substring(match.Index + 1, match.Length - 2);
                var choices = segments.Split('|');
                text = text.Substring(0, match.Index) + choices[random.Next(choices.Length)] + text.Substring(match.Index + match.Length);
                match = Regex.Match(text, pattern);
            }

            // Return the modified string.
            return text;
        }

        /// <summary>
        /// Generate spin text from small amount of text
        /// </summary>
        /// <param name="text">input text</param>
        /// <returns></returns>
        private static List<string> GetSpinnedTexts(string text)
        {

            #region Properties

            var matchDictionary = new Dictionary<Match, string[]>();
            var possibleTexts = new List<string>();
            var splittedDataInsideBraces = new List<string[]>();
            var spinTextPattern = new Regex(@"\(([^)]*)\)", RegexOptions.Compiled);

            #endregion

            #region given Text Combinations

            // Iterate all matches
            foreach (Match match in spinTextPattern.Matches(text))
            {
                try
                {
                    // Get all spin texts
                    var dataInsideBracesArray = match.Value.Replace("(", "").Replace(")", "").Split('|');
                    // Add into text list
                    matchDictionary.Add(match, dataInsideBracesArray);
                    splittedDataInsideBraces.Add(dataInsideBracesArray);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            #endregion

            #region Generate the proper combinations

            // get the enumerators for the dictionary
            IDictionaryEnumerator enumerator = matchDictionary.GetEnumerator();

            var possibleTextCollection = new List<string> { text };

            // iterate splitted datas inside spin pattern
            foreach (var splitArray in splittedDataInsideBraces)
            {
                enumerator.MoveNext();
                try
                {
                    // Iterate all possible text combinations
                    foreach (var possibleText in possibleTextCollection)
                    {
                        try
                        {
                            foreach (var individualValue in splitArray)
                            {
                                try
                                {
                                    if (enumerator.Key == null)
                                        continue;

                                    var modComment = possibleText.Replace(enumerator.Key.ToString(), individualValue);
                                    // Add the possible text combinations
                                    possibleTexts.Add(modComment);
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                possibleTextCollection.AddRange(possibleTexts);
            }

            #endregion
            // return proper spin texts
            return possibleTextCollection.FindAll(s => !s.Contains("("));
        }

    }
}