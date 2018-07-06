using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DominatorHouseCore.Utility
{
    public static class SpinTexHelper
    {
        public static string GetSpinText(string content)
        {           
            var spintexCollection = GetSpinMessageCollection(content);
            var randomNumber = RandomUtilties.GetRandomNumber(spintexCollection.Count-1);
            return spintexCollection[randomNumber];
        }

        public static List<string> GetSpinMessageCollection(string content)
        {
            try
            {
                if (content.Length <= 150)
                    return GetSpinnedComments(content);

                var spinnedList = new List<string>();
                while (spinnedList.Count < 50)
                {
                    var spinnedText = SpinText(new Random(), content);
                    if (spinnedList.Contains(spinnedText))
                        continue;
                    spinnedList.Add(spinnedText);
                }
                return spinnedList;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return new List<string>();
        }

        private static string SpinText(Random random, string text)
        {
            const string pattern = @"\(([^)]*)\)";
            var match = Regex.Match(text, pattern);
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

        private static List<string> GetSpinnedComments(string text)
        {

            #region Properties

            var matchDictionary = new Dictionary<Match, string[]>();
            var possibleTexts = new List<string>();
            var splittedDataInsideBraces = new List<string[]>();
            var spinTextPattern = new Regex(@"\(([^)]*)\)", RegexOptions.Compiled);

            #endregion

            #region given Text Combinations

            foreach (Match match in spinTextPattern.Matches(text))
            {
                try
                {
                    var dataInsideBracesArray = match.Value.Replace("(", "").Replace(")", "").Split('|');
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

            IDictionaryEnumerator enumerator = matchDictionary.GetEnumerator();

            var possibleTextCollection = new List<string> { text };

            foreach (var splitArray in splittedDataInsideBraces)
            {
                enumerator.MoveNext();
                try
                {
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

            return possibleTextCollection.FindAll(s => !s.Contains("("));
        }

    }
}