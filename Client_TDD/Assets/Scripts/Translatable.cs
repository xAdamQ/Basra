using System;
using TMPro;
using UnityEngine;

public enum Language
{
    Arabic,
    English
}

public class Translatable : MonoBehaviour
{
    #region kvp attempt

    //[ShowInInspector] private KeyValuePair<Language, string>[] languages = languagesInit();
    ////[ShowInInspector] private ListDictionary<Language, string> languagesDic = new Dictionary<Language, string> { { Language.English, "" } };
    //[ShowInInspector] private ListDictionary languagesDic = new ListDictionary { { Language.English, "" } };

    //private static KeyValuePair<Language, string>[] languagesInit()
    //{
    //    var langs = new KeyValuePair<Language, string>[languagesEnumValues.Length];

    //    for (int i = 0; i < LanguagesEnumValues.Length; i++)
    //    {
    //        langs[i] = new KeyValuePair<Language, string>(LanguagesEnumValues[i], "");
    //    }

    //    return new KeyValuePair<Language, string>[2];
    //}

    #endregion

    [Header("Arabic is default, start with English, another...")]
    [SerializeField]
    private string[] translations = new string[languagesEnumValues.Length - 1];

    //first lang is english, matches index 1 in enum

    //we get current lang from presisitant storage like instantGames 1mb
    private static Language currentLanguage = Language.Arabic;
    public static Language CurrentLanguage
    {
        get { return currentLanguage; }
        set
        {
            currentLanguage = value;
            //RefreshLanguageForActiveTranslatables();
        }
    }

    private static Language[] languagesEnumValues = (Language[])Enum.GetValues(typeof(Language));
    public static Language[] LanguagesEnumValues => languagesEnumValues;

    public void Awake()
    {
        var langindex = (int)CurrentLanguage - 1;

        if (langindex >= 0)
            GetComponent<TMP_Text>().text = translations[langindex];

#if UNITY_EDITOR
        //if (translations.Any(t => string.IsNullOrWhiteSpace(t)))
        //Debug.LogWarning($"translation expected at {name}");
#endif
    }


    //you can't change language for scene text because arabic is not stored anywhere when it's overriden
    //private static void RefreshLanguageForActiveTranslatables()
    //{
    //    foreach (var t in FindObjectsOfType<Translatable>()) t.Translate();
    //}

    //private void Translate()
    //{

    //}
}


/*

the ultimate localization design:

    - text should't be stored in memory, but should be written in component, achieve this by editor window where the
        entered translation is stored in a file (json or xml), where the translation linked to the compenent by comp uid.
        this point is very hard and auto localization solution could be better

    - make excel sheet for translations
 
 */