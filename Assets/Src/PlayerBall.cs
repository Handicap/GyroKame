using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GyroKame
{
    public class PlayerBall : MonoBehaviour
    {
        [SerializeField] private Rigidbody body;
        private bool ready = true;
        Vector3 startGrav;
        // Start is called before the first frame update
        void Start()
        {
            startGrav = Physics.gravity;
            Physics.gravity = Vector3.zero;
        }

        public void Drop()
        {
            Physics.gravity = startGrav;
            ready = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.Space) && ready)
            {
                Drop();
            }
            // for dev
            if (Input.GetKey(KeyCode.A))
            {
                Vector3 newGrav = new Vector3(Physics.gravity.x - 0.05f, Physics.gravity.y, Physics.gravity.z).normalized * 10f;
                Physics.gravity = newGrav;
            } else if (Input.GetKey(KeyCode.D))
            {
                Vector3 newGrav = new Vector3(Physics.gravity.x + 0.05f, Physics.gravity.y, Physics.gravity.z).normalized * 10f;
                Physics.gravity = newGrav;
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            GameEntry entry = collision.collider.gameObject.GetComponent<GameEntry>();
            if (entry != null)
            {
                entry.OnHit();
            }
        }
    }
}