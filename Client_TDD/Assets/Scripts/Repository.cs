using System.ComponentModel;
using System.Runtime.CompilerServices;
using Basra.Models.Client;
using JetBrains.Annotations;

public interface IRepository
{
    PersonalFullUserInfo PersonalFullInfo { get; set; }
    MinUserInfo[] YesterdayChampions { get; set; }
    MinUserInfo[] TopFriends { get; set; }

    //move this to place where it can be synced from server
    int[] CardbackPrices { get; }
    // string[] Titles { get; }
}

public class Repository : IRepository
{
    public PersonalFullUserInfo PersonalFullInfo { get; set; }
    public MinUserInfo[] YesterdayChampions { get; set; }
    public MinUserInfo[] TopFriends { get; set; }
    public int[] CardbackPrices { get; } = {50, 65, 100, 450, 600, 700, 1800, 2000, 2600};
    public static string[] Titles =
    {
        "thw chosen one",
        "piece of skill",
        "holy hanaka",
        "basra grandmaster",
        "top eater"
    };
}