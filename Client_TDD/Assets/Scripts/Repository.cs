using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

public interface IRepository
{
    PersonalFullUserInfo PersonalFullInfo { get; set; }
    MinUserInfo[] YesterdayChampions { get; set; }
    MinUserInfo[] TopFriends { get; set; }
    int[] CardbackPrices { get; }
    int ChosenCardback { get; set; }
}

public class Repository : IRepository
{
    public PersonalFullUserInfo PersonalFullInfo { get; set; }
    public MinUserInfo[] YesterdayChampions { get; set; }
    public MinUserInfo[] TopFriends { get; set; }
    public int[] CardbackPrices { get; } = {50, 65, 100, 450, 600, 700, 1800, 2000, 2600};
    public int ChosenCardback { get; set; }
}