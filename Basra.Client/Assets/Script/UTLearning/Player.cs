using UnityEngine;

namespace Basra.Client
{
    public class Player : MonoBehaviour
    {
        private Rigidbody2D _rigidbody;
        public IPlayerInput PlayerInput;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.gravityScale = 0;
        }

        private void Update()
        {
            var vertical = PlayerInput.Vertical;

            var speed = .1f;

            _rigidbody.AddForce(Vector2.up * vertical * speed);
        }
    }
}