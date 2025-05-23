using System;
using TMPro;
using UnityEngine;

public class SelectableCharacter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI CharDisplay;
    public char Character { get; private set; } = 'A';

    public void NextChar()
    {
        Character = NextLetter(Character);
        CharDisplay.text = Character.ToString();
    }

    public void PreviousChar()
    {
        Character = PreviousLetter(Character);
        CharDisplay.text = Character.ToString();
    }

    public void SetChar(char c)
    {
        Character = c;
        CharDisplay.text = Character.ToString();
    }

    private char NextLetter(char c)
    {
        if (c == 'Z') return 'A';
        else return (Char)(Convert.ToUInt16(c) + 1);
    }

    private char PreviousLetter(char c)
    {
        if (c == 'A') return 'Z';
        else return (Char)(Convert.ToUInt16(c) - 1);
    }
}
