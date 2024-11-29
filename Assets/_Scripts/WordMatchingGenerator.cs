using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class WordMatchingGenerator : MonoBehaviour
{
    private const string WordListUrl = "https://raw.githubusercontent.com/dwyl/english-words/master/words_alpha.txt";

    private List<string> wordList = new List<string>();
    private int targetWordLength = 0;
    private System.Random random;

    public WordSet set;


    public void GenerateWordSet(Action<WordSet> OnSet = null)
    {
        // Seed the random generator with the current date
        random = new System.Random(DateTime.Now.Date.GetHashCode());

        // Randomly select a target word length between 5 and 7
        targetWordLength = random.Next(5, 8);
        Debug.Log("Target Word Length: " + targetWordLength);

        // Fetch word list
        StartCoroutine(FetchWordsFromDwyl(OnSet));
    }

    private IEnumerator FetchWordsFromDwyl(Action<WordSet> OnSet)
    {
        UnityWebRequest request = UnityWebRequest.Get(WordListUrl);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            try
            {
                // Parse the word list from the response
                string[] words = request.downloadHandler.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                // Filter words by the target length
                wordList = words.Where(word => word.Length == targetWordLength).ToList();

                if (wordList.Count > 0)
                {
                    // Generate the set of matching words
                    set = GenerateMatchingWords();
                    OnSet?.Invoke(set);
                    //Debug.Log("Generated Words: " + string.Join(", ", resultWords));
                }
                else
                {
                    Debug.LogWarning("No valid words of the specified length were found.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to process word list: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("Failed to fetch word list: " + request.error);
        }
    }

    private WordSet GenerateMatchingWords()
    {
        // Select the first word as the model
        string modelWord = wordList[random.Next(wordList.Count)];
        Debug.Log("Model Word: " + modelWord);

        wordList = Shuffle(wordList);

        // Ensure one word matches more than half of the letters
        string highMatchWord = wordList.FirstOrDefault(word =>
            CountMatchingLetters(modelWord, word) > modelWord.Length / 2 && word != modelWord);

        if (string.IsNullOrEmpty(highMatchWord))
        {
            Debug.LogWarning("No word found that matches more than half of the letters.");
            highMatchWord = modelWord; // Fallback to using the model word itself
        }

        // Ensure other words match at least one letter
        List<string> partialMatchWords = wordList.Where(word =>
            CountMatchingLetters(modelWord, word) >= 1 &&
            word != modelWord &&
            word != highMatchWord)
            .Take(10) // We need 10 words excluding the model and high-match word
            .ToList();

        // Combine all the words into a single list
        List<string> wordOptions = new List<string> { modelWord, highMatchWord };
        wordOptions.AddRange(partialMatchWords);


        // Ensure exactly 12 words are included

        wordOptions = wordOptions.Take(12).ToList();

        WordSet set = new WordSet()
        {
            winWord = modelWord,
            optionWords = wordOptions
        };

        return set;
    }

    private int CountMatchingLetters(string word1, string word2)
    {
        int matches = 0;
        for (int i = 0; i < word1.Length; i++)
        {
            if (word1[i] == word2[i])
                matches++;
        }
        return matches;
    }


    private List<string> Shuffle(List<string> words)
    {
        List<string> result = new List<string>(words.Count);

        for (int i = 0; i < result.Capacity; i++)
        {
            int x = random.Next(words.Count);
            result.Add(words[x]);
            words.RemoveAt(x);
        }

        return result;
    }
}

[System.Serializable]
public class WordSet
{
    public string winWord;
    public List<string> optionWords;
}
