//using Basra.Server.Extensions;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Basra.Server.Structure.Room
//{
//    public class Ground
//    {
//        public List<int> Cards { get; set; }

//        public Ground(List<int> cards)
//        {
//            Cards = cards;
//        }

//        public List<int> Eat(int card)
//        {
//            Cards.Add(card);

//            var eaten = Cards.CutRange(1, fromEnd: false);

//            return eaten;
//        }


//        //create card list with used = false
//        //remove shapes from this list
//        //

//        /*

//public List<GameObject> RefreshSimilar()
//{
//Similar.Clear();
//var WBC = new List<GameObject>();
//WBC.AddRange(main.Ground);
//foreach (var card in WBC) card.GetComponent<card0>().used = false;

//var C = new List<//As
//                 List< //Bs
//                      List< //Es
//                           List< //Fs
//                                GameObject>>>>();

//if (GetComponent<CardInfo>().Id <= 10)
//{
//    int NoShapsNM = 0;
//    while (NoShapsNM < WBC.Count)
//    {
//        if (WBC[NoShapsNM].GetComponent<CardInfo>().getiD() > 10)
//            WBC.RemoveAt(NoShapsNM);
//        else
//            NoShapsNM++;
//    }
//}
////Remove shapes

//C.Add(new List<List<List<GameObject>>>()); // new A
//foreach (var item in WBC)
//{
//    var B = new List<List<GameObject>>();
//    var E = new List<GameObject>();
//    E.Add(item);
//    B.Add(E);
//    C.Last().Add(B);
//}
////Making Level 0


//if (GetComponent<CardInfo>().Id <= 10)
//{
//    for (int A = 1; A < WBC.Count; A++) // A maker
//    {
//        C.Add(new List<List<List<GameObject>>>());
//        for (int B = 0; B < WBC.Count - A; B++)// B maker
//        {
//            //Es E maker (shortcut)
//            C.Last().Add(new List<List<GameObject>>());
//            for (int PrevB = B + 1; PrevB < C[A - 1].Count; PrevB++)
//            {
//                foreach (var E_IN_PrevB in C[A - 1][PrevB])
//                {
//                    // An F shortcut
//                    C.Last().Last().Add(new List<GameObject>());

//                    C.Last().Last().Last().Add(WBC[B]);
//                    C.Last().Last().Last().AddRange(E_IN_PrevB);
//                }

//            }
//        }
//    }
//}
////Make cards groups pyramid 

//for (int A = C.Count - 1; A >= 0; A--)
//{
//    for (int B = C[A].Count - 1; B >= 0; B--)
//    {
//        for (int E = C[A][B].Count - 1; E >= 0; E--)
//        {
//            var usedCardExist = false;
//            foreach (var card in C[A][B][E])
//                if (card.GetComponent<card0>().used == true)
//                {
//                    //Debug.Log(card);
//                    usedCardExist = true;
//                    break;
//                }
//            if (usedCardExist) continue;
//            // ignore the group that has a used card

//            if (IdsSum(C[A][B][E]) == GetComponent<CardInfo>().Id)
//                foreach (var card in C[A][B][E])
//                {
//                    Similar.Add(card);
//                    card.GetComponent<card0>().used = true;
//                }


//        }
//    }
//}

//return Similar;
//}
//        */
//    }
//}
