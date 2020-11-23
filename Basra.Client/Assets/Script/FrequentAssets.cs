using UnityEngine;

//my address system
public class FrequentAssets : MonoBehaviour
{
    public static FrequentAssets I;
    private void Awake()
    {
        I = this;
    }

    public GameObject EmptyCardPrefab;
    public GameObject RealCardPrefab;
    public GameObject MyHandPrefab;
    public GameObject OtherHandPrefab;
    public Sprite[] NumberSprites;//thier order matters

}