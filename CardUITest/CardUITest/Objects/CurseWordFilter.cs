using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CardUITest.Objects
{
    public class CurseWordFilter
    {
        //List<string> bannedWords = new List<string>();

        //public CurseWordFilter()
        //{
        //    bannedWords.Add("Asshole");
        //    bannedWords.Add("Asshole");
        //    bannedWords.Add("Poes");
        //}
        private static List<string> BannedWords()
        {
            List<string> bWord = new List<string>();
            bWord.Add("Poes");
            bWord.Add("Ass");
            bWord.Add("Dick");
            // Add more here!
            return bWord;
        }
        public static string FilterBannedWords(string noteBody)
        {
            foreach (var bWord in BannedWords())
            {
                // replacing the curse word with ***'s but keeping the same length
                string strReplace = "";

                for (int i = 0; i <= bWord.Length; i++)
                {
                    strReplace += "*";
                }

                noteBody = Regex.Replace(noteBody.ToString(), bWord, strReplace, RegexOptions.IgnoreCase);
            }
            return noteBody.ToString();
        }
    }
}
