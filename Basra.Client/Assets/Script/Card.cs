using System.Collections;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UI;

public enum CardType { Opponent, Mine, Ground }

public class Card : MonoBehaviour
{
    public static Vector2 Bounds = new Vector2(.75f, 1f);

    public CardType Type;

    //works on my, ground
    public Front Front;

    //works on my, oppo
    public Hand Hand;

    protected void Awake()
    {
        Hand = transform.parent.GetComponent<Hand>();

        switch (Type)
        {
            case CardType.Opponent:
                break;
            case CardType.Mine:
                Front = transform.GetChild(0).GetComponent<Front>();
                break;
            case CardType.Ground:
                Front = transform.GetChild(0).GetComponent<Front>();
                break;
        }
    }

    private void OnMouseDown()
    {
        VisualThrow();
    }

    #region time machine testing
    //server client sync ways
    //1- (instant, awaited)
    //2- (perfect data, perfect no user logic)
    //awaited can work with any perfect, but instant works with perfect data only

    /////the instant isuue
    //don't think of malicous users in client, the server handles all possibilites
    //the last action should be certificated by server because of the connection delays
    //so every action should be reversed and every client action the call server, server must call it

    // //works on my, oppo
    // public async Task<int[]> Throw()
    // {
    //     if (Type != CardType.Mine)
    //         throw new System.Exception("play your card dump ass");

    //     var indexInHand = Hand.Cards.IndexOf(this);//clients logic
    //     var eaten = await AppManager.I.HubConnection.InvokeAsync<int[]>("Throw", indexInHand);
    //     //what are the possible server responses?
    //     //1- value, empty array not null, null is faulty thing
    //     //2- server doesn't die, so it may send an exception or a message or null
    //     //return fun(hand, ground, other things)
    //     //so to make the logic on the client you need to pass the logic code

    //     if (eaten != null/*and other error messages*/)
    //     {
    //         Hand.Room.Ground.Add(this);
    //         Hand.Cards.Remove(this);
    //     }

    //     //makes the actions in the client itself

    //     return eaten;
    //     //I don't calc eaten locally
    //     //can I even sync the ground list and this things!, so you just send to the server and don't duplicate stuff around in both
    //     //there are 2 ways of this, perefect data and perfect no user logic, means the user should't process server data if if he can

    //     //server feedback implementation
    //     //deos complete happens in faluire anf cancel? I don't think so
    // }

    // //my code can be mixture of awaited and instant
    // //the instant concept is a single action must be done without server certification, the next action can be awaited if needed
    // public void ThrowIF()
    // {
    //     if (Type != CardType.Mine)
    //         throw new System.Exception("play your card dump ass");

    //     Hand.Room.Ground.Add(this);
    //     var indexInHand = Hand.Cards.IndexOf(this);
    //     Hand.Cards.Remove(this);

    //     AppManager.I.MySend("Throw", indexInHand);
    //     // AppManager.I.HubConnection.Send("Throw", indexInHand);
    // }

    // public void ShadowThrow()
    // {
    //     Hand.Room.Ground.ShadowAdd(this);
    // }
    // public void ReverseThrow()
    // {

    // }
    // public void ConfirmThrow()
    // {
    //     Hand.Room.Ground.ConfirmAdd();
    // }
    // public void ServerThrow()
    // {

    // }
    #endregion

    //server action initiator, who init the PrevAction
    public void VisualThrow()
    {
        if (Type != CardType.Mine)
            throw new System.Exception("play your card dump ass");

        var indexInHand = Hand.Cards.IndexOf(this);

        Hand.Room.Ground.VisualAdd(this);

        AppManager.I.LastAction = new ActionRecord(this, "Throw", indexInHand);
        AppManager.I.LastAction.RemoteCall();
    }
    public void InternalThrow()
    {
        Hand.Room.Ground.ActualAdd(this);
        Hand.Cards.Remove(this);
    }
    public void ServerThrow(int indexInHand)
    {
        var card = Hand.Cards[indexInHand];
        Hand.Room.Ground.VisualAdd(card);
        Hand.Room.Ground.ActualAdd(card);
        Hand.Cards.Remove(card);
    }



    /// <summary>
    /// for oppo cards only, 
    /// </summary>
    public void Throw(int id)
    {
        if (Type != CardType.Opponent)
            throw new System.Exception("play opponent card dump ass");

        AddFront(id);

        Hand.Cards.Remove(this);
        Hand.Room.Ground.Add(this);
    }

    public void AddFront(int id)
    {
        Front = Instantiate(FrequentAssets.I.FrontPrefab, transform).GetComponent<Front>();
        Front.transform.localPosition = Vector3.back * .01f;
        Front.Set(id);
    }

    //2 types of design, instant feedback and server feedback
    //server feedback is easier
    //in the instant feedback you are blocked in some actions
    //in server feedback you're blocked by default

    //server feedback design
    //return types are awaited with coroutines


}
