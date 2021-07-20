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

    private static int[] cardbackPrices = {50, 65, 100, 450, 600, 700, 1800, 2000, 2600};
    private static int[] backgroundPrices = {50, 65, 100, 450, 600,};
    public static int[][] ItemPrices => new[] {cardbackPrices, backgroundPrices};

    public static string[] Titles =
    {
        "thw chosen one",
        "piece of skill",
        "holy hanaka",
        "basra grandmaster",
        "top eater"
    };

    //since controller module group doesn't die 
    public static IRepository I;

    public Repository()
    {
        if (I != null) throw new Exception("reinitializing singleton that already exists");
        I = this;
    }
}