using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class DwylWordGenerator : MonoBehaviour
{
    private const string WordListUrl = "https://raw.githubusercontent.com/dwyl/english-words/master/words_alpha.txt";

    private List<string> wordList = new List<string>();
    private int targetWordLength = 0;

    void Start()
    {
        // Seed word length using the date
        System.Random random = new System.Random(DateTime.Now.Date.GetHashCode());
        targetWordLength = random.Next(5, 8); // Pick a length between 5 and 7
        Debug.Log("Target Word Length: " + targetWordLength);

        // Fetch word list
        StartCoroutine(FetchWordsFromDwyl());
    }

    private IEnumerator FetchWordsFromDwyl()
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

                // Shuffle and select 6 words
                System.Random random = new System.Random(DateTime.Now.Date.GetHashCode());
                wordList = ShuffleAndSelect(wordList, 6, random);

                if (wordList.Count > 0)
                {
                    Debug.Log("Random Words of the Day: " + string.Join(", ", wordList));
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

    private List<string> ShuffleAndSelect(List<string> words, int count, System.Random random)
    {
        // Shuffle the list
        for (int i = words.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            var temp = words[i];
            words[i] = words[j];
            words[j] = temp;
        }

        // Take the first 'count' words
        return words.Take(count).ToList();
    }
}
