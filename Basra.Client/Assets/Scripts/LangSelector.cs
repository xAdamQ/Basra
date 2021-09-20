using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class LangSelector : MonoModule<LangSelector>
{
    public static void Create()
    {
        Create("langSelector", ProjectReferences.I.Canvas).Forget(e=>throw e);
    }
        
    [SerializeField] private ChoiceButton langChoice;
    [SerializeField] private TMP_Text langText;

    protected override void Awake()
    {
        langChoice.ChoiceChanged += OnChoiceChanged;

        var lang = PlayerPrefs.GetInt("lang");
        langChoice.SetChoice(lang);
    }

    private void OnChoiceChanged(int choice)
    {
        langText.text = choice == 0 ? "عربي" : "Enlgish";
        
        PlayerPrefs.SetInt("lang", choice);
        
        Translatable.CurrentLanguage = (Language)choice;
    }
    
    

}