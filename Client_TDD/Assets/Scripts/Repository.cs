using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

public interface IRepository
{
    PersonalFullUserInfo PersonalFullInfo { get; set; }
    PublicMinUserInfo[] YesterdayChampions { get; set; }
    PublicMinUserInfo[] TopFriends { get; set; }
}

public class Repository : IRepository
{
    public PersonalFullUserInfo PersonalFullInfo { get; set; }
    public PublicMinUserInfo[] YesterdayChampions { get; set; }
    public PublicMinUserInfo[] TopFriends { get; set; }
}