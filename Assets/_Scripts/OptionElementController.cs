using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionElementController : MonoBehaviour
{

    public string word;

    public TMP_Text text;
    public Button button;

    GameManager game;


    public void Initialize(GameManager game, string word)
    {
        this.game = game;
        this.word = word;
        text.text = word;
        button.onClick.AddListener(() =>SelectWord());
    }

    public void SelectWord()
    {
        game.SelectWord(word);
        gameObject.SetActive(false);
    }

}
