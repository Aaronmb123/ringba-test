using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ringba_test
{

    public class Program
    {
        public static void Main(string[] args)
        {
            const string URL = "https://ringba-test-html.s3-us-west-1.amazonaws.com/TestQuestions/output.txt ";
            string fileContents = string.Empty;

            Console.WriteLine($"Downloading file");

            try
            {
                WebClient wc = new WebClient();
                fileContents = wc.DownloadString(URL);
            }
            catch (WebException)
            {
                Console.WriteLine($"Error downloading file. Exiting...");
                return;
            }

            Console.WriteLine($"Calculating Stats...");

            Dictionary<Char, int> letterCount = new Dictionary<char, int>();
            Dictionary<string, int> wordCount = new Dictionary<string, int>();
            Dictionary<string, int> prefixCount = new Dictionary<string, int>();
            int numCapitals = 0;
            int numChars = fileContents.Length;
            string currentWord = string.Empty; 

            bool isCapital;

            for (int i = 0; i < numChars;)
            {
                int currentWordLength = 1;
                char c = fileContents[i];
                isCapital = Char.IsUpper(c);

                if (isCapital)
                {
                    //How many letters are capitalized in the file
                    numCapitals++;
                    c = Char.ToLower(c);
                    //How many of each letter are in the file
                    if (letterCount.ContainsKey(c))
                    {
                        letterCount[c]++;
                    }
                    else
                    {
                        letterCount.Add(c, 1);
                    }
                    // beginning of downloaded string: AbabdehAbadiaAbakasAbampsAbandAbashed
                    // find current word length
                    int index = i + 1;
                    while (index < numChars)
                    {
                        c = fileContents[index];
                        if (Char.IsLower(c))
                        {
                            //How many of each letter are in the file
                            //duplicate code, but this counts the lower case letters
                            // in between the capitals
                            if (letterCount.ContainsKey(c))
                            {
                                letterCount[c]++;
                            }
                            else
                            {
                                letterCount.Add(c, 1);
                            }
                            currentWordLength++;
                        }
                        else
                        {
                            break;
                        }
                        index++;
                    }

                    currentWord = fileContents.Substring(i, currentWordLength);

                    //The most common word and the number of times it has been seen.
                    if (wordCount.ContainsKey(currentWord))
                    {
                        wordCount[currentWord]++;
                    }
                    else
                    {
                        wordCount.Add(currentWord, 1);
                    }

                    //The most common 2 character prefix and the number of occurrences in the text file.
                    if (currentWordLength > 2)
                    {
                        StringBuilder prefixBuilder = new StringBuilder();
                        prefixBuilder.Append(Char.ToLower(currentWord[0]));
                        prefixBuilder.Append(currentWord[1]);

                        string currentPrefix = prefixBuilder.ToString();
                        if (prefixCount.ContainsKey(currentPrefix))
                        {
                            prefixCount[currentPrefix]++;
                        }
                        else
                        {
                            prefixCount.Add(currentPrefix, 1);
                        }
                    }

                }
                //skip to the beginning of the next word, i.e. the next capital letter
                i += currentWordLength;
            }

            //How many letters are capitalized in the file
            Console.WriteLine($"Number of Capitals: {numCapitals}");
            
            //The most common word and the number of times it has been seen.
            string mostCommonWord = wordCount.OrderByDescending(keyValuePair => keyValuePair.Value).First().Key;
            int mostCommonWordOccurances = wordCount[mostCommonWord];
            Console.WriteLine($"Most common word: {mostCommonWord}, Number of occurances: {mostCommonWordOccurances}");

            //How many of each letter are in the file
            Console.WriteLine($"Number of Occurances of each letter");
            foreach (KeyValuePair<char, int> entry in letterCount)
            {
                Console.WriteLine($"{entry.Key} : {entry.Value}");
            }

            //The most common 2 letter prefix and the number of times it has been seen.
            string mostCommonPrefix = prefixCount.OrderByDescending(keyValuePair => keyValuePair.Value).First().Key;
            int mostCommonPfOccurances = prefixCount[mostCommonPrefix];
            Console.WriteLine($"Most common prefix: {mostCommonPrefix}, Number of occurances: {mostCommonPfOccurances}");
        }
    }
}