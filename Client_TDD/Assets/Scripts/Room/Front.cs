using UnityEngine;
using Zenject;

public class Front : MonoBehaviour
{
    public int Index;

    public class Factory
    {
        private readonly IInstantiator _instantiator;
        private readonly GameObject _prefab;
        private readonly Sprite[] _frontSprites;

        public Factory(IInstantiator instantiator, GameObject prefab, Sprite[] frontSprites)
        {
            _instantiator = instantiator;
            _prefab = prefab;
            _frontSprites = frontSprites;
        }

        public Front Create(int index, Transform parent)
        {
            var front = _instantiator.InstantiatePrefab(_prefab, parent)
                .GetComponent<Front>();

            //init, no init method because this class can access private members
            front.Index = index;
            front.GetComponent<SpriteRenderer>().sprite = _frontSprites[index];
            front.transform.localPosition = Vector3.forward * .01f;

            return front;
        }
    }
}