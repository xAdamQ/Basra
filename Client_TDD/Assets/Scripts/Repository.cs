using System;
using System.Collections.Generic;


public interface IRepository
{
    PersonalFullUserInfo PersonalFullInfo { get; set; }
    MinUserInfo[] YesterdayChampions { get; set; }
    MinUserInfo[] TopFriends { get; set; }
}

public class Repository : IRepository
{
    public PersonalFullUserInfo PersonalFullInfo { get; set; }
    public MinUserInfo[] YesterdayChampions { get; set; }
    public MinUserInfo[] TopFriends { get; set; }


    public static string[] Titles =
    {
        "thw chosen one",
        "piece of skill",
        "holy hanaka",
        "basra grandmaster",
        "top eater",
        "top eater",
        "top eater",
        "top eater",
        "top eater",
        "top eater",
        "top eater",
    };


    //since controller module group doesn't die 
    public static IRepository I;

    public Repository()
    {
        if (I != null) throw new Exception("reinitializing singleton that already exists");
        I = this;
    }
}