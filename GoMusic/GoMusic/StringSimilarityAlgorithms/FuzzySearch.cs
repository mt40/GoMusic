using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoMusic.StringSimilarityAlgorithms
{
    //#define LINQ
    //-----------------------------------------------------------------------------
    /// <summary>
    /// Provides methods for fuzzy string searching.
    /// </summary>
    public class LevenshteinDistance
    {
        /// <summary>
        /// Calculates the Levenshtein-distance of two strings.
        /// </summary>
        /// <param name="src">
        /// 1. string
        /// </param>
        /// <param name="dest">
        /// 2. string
        /// </param>
        /// <returns>
        /// Levenshstein-distance
        /// </returns>
        /// <remarks>
        /// See 
        /// <a href='http://en.wikipedia.org/wiki/Levenshtein_distance'>
        /// http://en.wikipedia.org/wiki/Levenshtein_distance
        /// </a>
        /// </remarks>
        private static int CalculateLevenshteinDistance(string src, string dest)
        {
            int[,] d = new int[src.Length + 1, dest.Length + 1];
            int i, j, cost;
            char[] str1 = src.ToCharArray();
            char[] str2 = dest.ToCharArray();

            for (i = 0; i <= str1.Length; i++)
            {
                d[i, 0] = i;
            }
            for (j = 0; j <= str2.Length; j++)
            {
                d[0, j] = j;
            }
            for (i = 1; i <= str1.Length; i++)
            {
                for (j = 1; j <= str2.Length; j++)
                {

                    if (str1[i - 1] == str2[j - 1])
                        cost = 0;
                    else
                        cost = 1;

                    d[i, j] =
                        Math.Min(
                            d[i - 1, j] + 1,					// Deletion
                            Math.Min(
                                d[i, j - 1] + 1,				// Insertion
                                d[i - 1, j - 1] + cost));		// Substitution

                    if ((i > 1) && (j > 1) && (str1[i - 1] == str2[j - 2]) && (str1[i - 2] == str2[j - 1]))
                    {
                        d[i, j] = Math.Min(d[i, j], d[i - 2, j - 2] + cost);
                    }
                }
            }

            return d[str1.Length, str2.Length];
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Fuzzy searches a list of strings.
        /// </summary>
        /// <param name="word">
        /// String you need to search.
        /// </param>
        /// <param name="text">
        /// String to be searched.
        /// </param>
        /// <param name="fuzzyness">
        /// Ration of the fuzzyness. A value of 0.8 means that the 
        /// difference between the word to find and the found words
        /// is less than 20%.
        /// </param>
        /// <returns>
        /// True if match (depend on fuzzyness), otherwise false.
        /// </returns>
        /// <example>
        /// 
        /// </example>
        public static bool Search(string pattern, string text, double fuzzyness)
        {

            // Calculate the Levenshtein-distance:
            int levenshteinDistance =
                CalculateLevenshteinDistance(pattern, text);

            // Length of the longer string:
            int length = Math.Max(pattern.Length, text.Length);

            // Calculate the score:
            double score = 1.0 - (double)levenshteinDistance / length;

            // Match?
            if (score > fuzzyness)
                return true;
            return false;
        }
    }
}

