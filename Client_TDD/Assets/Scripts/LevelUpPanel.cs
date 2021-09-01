using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LevelUpPanel : MonoModule<LevelUpPanel>
{
    [SerializeField] private TMP_Text levelText, moneyRewardText;

    public static async UniTaskVoid Create(int newLevel, int moneyReward)
    {
        await Create("levelUpPanel", ProjectReferences.I.Canvas);

        I.levelText.text = newLevel.ToString();
        I.moneyRewardText.text = moneyReward.ToString();
    }

    public void Destroy() => Destroy(gameObject);
}

public abstract class MonoModule<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T I;

    protected static async UniTask Create(string address, Transform parent)
    {
        I = (await Addressables.InstantiateAsync(address, parent)).GetComponent<T>();
    }

    // public static void Destroy()
    // {
    //     Object.Destroy(I.gameObject);
    //     I = null;
    // }
}