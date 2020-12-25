using UnityEngine;

namespace Basra.Client
{
    public class PlayerInput : IPlayerInput
    {
        public float Vertical => Input.GetAxis("Vertical");
    }
}