using UnityEngine;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    [SerializeField] private Image[] choiceIndicators;

    [SerializeField] private int startChoice;

    [HideInInspector] public int CurrentChoice;

    [SerializeField] private Color choiceColor = Color.red;
    private void Start()
    {
        CurrentChoice = startChoice;

        foreach (var ci in choiceIndicators)
            ci.color = Color.white;

        choiceIndicators[CurrentChoice].color = choiceColor;
    }

    public virtual void NextChoice()
    {
        choiceIndicators[CurrentChoice].color = Color.white;

        CurrentChoice = ++CurrentChoice % choiceIndicators.Length;

        choiceIndicators[CurrentChoice].color = choiceColor;
    }
}