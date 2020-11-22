using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Current;

    private List<int> Hand;

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

    private void Awake()
    {
        Current = this;
    }

    //this will work because room follows "Current" patterns
    public static int Genre;
    public static int PlayerCount;

    [SerializeField]
    private Text GenreText;

    private void Start()
    {
        Ready();
        GenreText.text = Genre.ToString();
    }

    public void SetHand(List<int> hand)
    {
        Hand = hand;
        //do hand anim
        Debug.Log($"hand cards are {hand[0]} {hand[1]} {hand[2]} {hand[3]}");
    }

    public void Ready()
    {
        AppManager.I.HubConnection.SendAsync("Ready");
    }
}
