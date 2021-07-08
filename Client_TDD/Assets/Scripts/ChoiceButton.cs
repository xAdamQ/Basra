using UnityEngine;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    [SerializeField] private Image[] choiceIndicators;

    [SerializeField] private int startChoice;

    [HideInInspector] public int CurrentChoice;

    private void Start()
    {
        CurrentChoice = startChoice;

        choiceIndicators[CurrentChoice].color = Color.red;
    }

    public void NextChoice()
    {
        choiceIndicators[CurrentChoice].color = Color.white;

        CurrentChoice = ++CurrentChoice % choiceIndicators.Length;

        choiceIndicators[CurrentChoice].color = Color.red;
    }
}