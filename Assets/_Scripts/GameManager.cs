using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public WordMatchingGenerator generator;
    public GameObject winPanel, losePanel;


    public HistoryElementController historyPrefab;
    public OptionElementController optionPrefab;

    public Transform historyParent, optionParent;

    public int maxAttempts = 4;

    public int currentAttempt = 0;

    List<HistoryElementController> attempts;
    List<OptionElementController> options;

    public WordSet set;


    // Start is called before the first frame update
    void Start()
    {
        LoadNewGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectWord(string word)
    {
        var attempt = attempts[currentAttempt++];
        attempt.SelectWord(word);

        if (IsCorrectWord(word))
        {
            GameOver(true);
        }
        else if(currentAttempt >= maxAttempts)
        {
            GameOver(false);
        }
    }

    public void GameOver(bool result)
    {
        winPanel.SetActive(result);
        losePanel.SetActive(result == false);
        foreach(var o in options)
        {
            o.button.interactable = false;
        }
    }




    void LoadNewGame()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        attempts = new List<HistoryElementController>();
        for (int i = 0; i < maxAttempts; i++)
        {
            HistoryElementController history = Instantiate(historyPrefab, historyParent);
            history.Initialize(this);
            attempts.Add(history);
        }
        generator.GenerateWordSet(GenerateGame);
    }

    void GenerateGame(WordSet set)
    {
        options = new List<OptionElementController>();
        this.set = set;
        List<string> words = Shuffle(set.optionWords);
        foreach(var word in words)
        {
            OptionElementController option = Instantiate(optionPrefab, optionParent);
            option.Initialize(this, word);
            options.Add(option);
        }
        
    }



    private List<string> Shuffle(List<string> words)
    {
        List<string> result = new List<string>(words.Count);

        for (int i = 0; i < result.Capacity; i++)
        {
            int x = Random.Range(0, words.Count);
            result.Add(words[x]);
            words.RemoveAt(x);
        }

        return result;
    }

    bool IsCorrectWord(string word)
    {
        return set.winWord.Equals(word);
    }
}
