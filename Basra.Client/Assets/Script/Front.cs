using System.Collections;
using UnityEngine;
namespace Basra.Client
{
    public class Front : MonoBehaviour
    {
        public int Id;

        public void Set(int id)
        {
            Id = id;
            GetComponent<SpriteRenderer>().sprite = FrequentAssets.I.NumberSprites[id];
        }
    }
}