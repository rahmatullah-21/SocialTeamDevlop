using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.Utility
{
    public static class RandomUtilties
    {
        public static Random ObjRandom { get; } = new Random(Guid.NewGuid().GetHashCode());

        /// <summary>
        /// This Medthod is used to get the random item from the list
        /// </summary>
        /// <typeparam name="T">The Type of the element of the inputList</typeparam>
        /// <param name="inputList">List of value for get the random number from it</param>
        /// <returns>Returns the random item from input list</returns>
        public static T GetRandomItem<T>(this IList<T> inputList)
        {
            // Get the random index from list max count and min count 
            var index = RandomUtilties.GetRandomNumber(inputList.Count - 1, 0);

            // return the random item from the input list based on index value
            return inputList[index];
        }


        /// <summary>
        /// GetRandomNumber method is used to get the random value which lies from maxValue and MinValue
        /// </summary>
        /// <param name="maxValue">Maxvalue is define the upper bound</param>
        /// <param name="minValue">Maxvalue is define the lower bound</param>
        /// <returns>Return the random value lies from upper and lower</returns>
        public static int GetRandomNumber(int maxValue, int minValue = 0)
        {
            // increase the maxValue for getting a chance for select a last item as random number
            ++maxValue;
            // Collect the random value from min and max value
            return RandomUtilties.ObjRandom.Next(minValue, maxValue);
        }



        /// <summary>
        /// GetRandomString method is used to get the random string from alphanumeric character with required length
        /// </summary>
        /// <param name="outputStringLength">The required string count from alphanumeric character </param>
        /// <returns>returns random string from alphanumeric character</returns>
        public static string GetRandomString(int outputStringLength)
        {
            // returns random string from alphanumeric character
            return new string
                (Enumerable.Repeat<string>("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", outputStringLength).
                Select<string, char>((Func<string, char>)(s => s[RandomUtilties.ObjRandom.Next(s.Length)])).
                ToArray<char>());
        }

        /// <summary>
        /// GetRandomTexts method is used to get the random text from alphabets with required length
        /// </summary>
        /// <param name="outputTextLength">The required text characters count from alpha character </param>
        /// <returns>returns random text from alphabets</returns>
        public static string GetRandomTexts(int outputTextLength)
        {
            // returns random string from alphanumeric character
            return new string
            (Enumerable.Repeat<string>("ABCDEFGHIJKLMNOPQRSTUVWXYZ", outputTextLength).
                Select<string, char>((Func<string, char>)(s => s[RandomUtilties.ObjRandom.Next(s.Length)])).
                ToArray<char>());
        }


        /// <summary>
        /// Get Random string 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomString(int length)
        {
            return new string(Enumerable.Repeat<string>("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length).Select<string, char>((Func<string, char>)(s => s[RandomUtilties.ObjRandom.Next(s.Length)])).ToArray<char>());
        }
       
    }
}
