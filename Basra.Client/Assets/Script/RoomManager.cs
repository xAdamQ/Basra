using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Current;

    private MyHand MyHand;
    private OtherHand[] OtherHands;

    static float UpperPadding = .5f, ButtomPadding = 1f;

    static Vector2[] HandPozes = new Vector2[]
    {
        new Vector2(0, -5 + ButtomPadding),
        new Vector2(0, 5 - UpperPadding),
        new Vector2(2.5f, 0),
        new Vector2(-2.5f, 0),
    };
    static Vector3[] HandRotations = new Vector3[]
    {
        new Vector3(),
        new Vector3(0, 0, 180),
        new Vector3(0, 0, 90),
        new Vector3(0, 0, -90),
    };

    //this will work because room follows "Current" patterns
    //any static you have to reinit
    public static int Genre;
    public static int PlayerCount;

    [SerializeField] Text GenreText;

    private void Awake()
    {
        Current = this;
    }

    private void Start()
    {
        Ready();
        GenreText.text = Genre.ToString();

        CreateHands();
    }

    private void CreateHands()
    {
        MyHand = Instantiate(FrequentAssets.I.MyHandPrefab).GetComponent<MyHand>();
        MyHand.transform.position = HandPozes[0];
        MyHand.transform.eulerAngles = HandRotations[0];

        OtherHands = new OtherHand[PlayerCount];
        for (var i = 0; i < PlayerCount - 1; i++)
        {
            OtherHands[i] = Instantiate(FrequentAssets.I.OtherHandPrefab).GetComponent<OtherHand>();
            OtherHands[i].transform.position = HandPozes[i + 1];
            OtherHands[i].transform.eulerAngles = HandRotations[i + 1];
        }
    }

    public void Distribute(int[] hand)
    {
        MyHand.Set(hand);

        for (var i = 0; i < PlayerCount - 1; i++)
        {
            OtherHands[i].Set();
        }

        Debug.Log($"hand cards are {hand[0]} {hand[1]} {hand[2]} {hand[3]}");
    }

    public void Ready()
    {
        AppManager.I.HubConnection.SendAsync("Ready");
    }
}
