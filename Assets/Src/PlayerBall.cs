using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GyroKame
{
    public class PlayerBall : MonoBehaviour
    {
        [SerializeField] private Rigidbody body;
        [SerializeField] private float minDepth = -100f;
        private bool ready = true;
        Vector3 startGrav;

        public event Action OnBallLost, OnBallDropped;

        private Vector3 startPos;

        public bool Ready { get => ready; set => ready = value; }

        // Start is called before the first frame update
        void Start()
        {
            startGrav = Physics.gravity;
            startPos = transform.position;
            ResetBall();
        }

        public void Drop()
        {
            Physics.gravity = startGrav;
            Ready = false;
            body.isKinematic = false;
            OnBallDropped?.Invoke();
        }

        public void ResetBall()
        {
            Debug.Log("Reset ball");
            Physics.gravity = Vector3.zero;
            transform.position = startPos;
            body.isKinematic = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.Space) && Ready)
            {
                Drop();
            }
            // for dev
            if (Input.GetKey(KeyCode.A))
            {
                Vector3 newGrav = new Vector3(Physics.gravity.x - 0.15f, Physics.gravity.y, Physics.gravity.z).normalized * 10f;
                Physics.gravity = newGrav;
            } else if (Input.GetKey(KeyCode.D))
            {
                Vector3 newGrav = new Vector3(Physics.gravity.x + 0.15f, Physics.gravity.y, Physics.gravity.z).normalized * 10f;
                Physics.gravity = newGrav;
            }
            if (transform.position.y < minDepth)
            {
                OnBallLost?.Invoke();
                //ResetBall();
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