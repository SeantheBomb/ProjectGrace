using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HistoryElementController : MonoBehaviour
{

    public TMP_Text text;

    GameManager game;


    public void Initialize(GameManager game)
    {
        this.game = game;
    }

    public void SelectWord(string word)
    {
        text.text = $"{word} - {GetCorrectPlaces(word)}/{GetCorrectLetters(word)}";
    }


    int GetCorrectPlaces(string word)
    {
        int result = 0;
        for (int i = 0; i < word.Length; i++)
        {
            if (word[i].Equals(game.set.winWord[i]))
                result++;
        }
        return result;
    }

    int GetCorrectLetters(string word)
    {
        return word.Intersect(game.set.winWord).Count();
    }
}
