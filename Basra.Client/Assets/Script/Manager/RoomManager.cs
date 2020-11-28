using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Basra.Client
{
    public class RoomManager : MonoBehaviour
    {
        private Hand[] Hands;
        public Ground Ground;

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
            AppManager.I.Room = this;
        }

        private void Start()
        {
            Ready();
            GenreText.text = Genre.ToString();

            InitHands();
            InitGround();
        }

        private void InitHands()
        {
            Hands = new Hand[PlayerCount];

            for (var i = 0; i < PlayerCount; i++)
            {
                Hands[i] = Instantiate(FrequentAssets.I.HandPrefab).GetComponent<Hand>();
                Hands[i].transform.position = HandPozes[i];
                Hands[i].transform.eulerAngles = HandRotations[i];
                Hands[i].Room = this;
            }
        }
        private void InitGround()
        {
            Ground = Instantiate(FrequentAssets.I.GroundPrefab, Vector3.zero, new Quaternion()).GetComponent<Ground>();
            Ground.Room = this;
        }

        [Rpc]
        public void InitialDistribute(int[] hand, int[] ground)
        {
            Ground.Set(ground);
            Distribute(hand);
        }

        [Rpc]
        public void Distribute(int[] hand)
        {
            Hands[0].Set(hand);

            for (var i = 1; i < PlayerCount; i++)
            {
                Hands[i].Set();
            }

            Debug.Log($"hand cards are {hand[0]} {hand[1]} {hand[2]} {hand[3]}");
        }

        public void Ready()
        {
            AppManager.I.HubConnection.SendAsync("Ready");
        }
    }
}