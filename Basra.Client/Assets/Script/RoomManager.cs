using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Current;

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
        GenreText.text = Genre.ToString();
    }
}
