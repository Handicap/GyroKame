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
        [SerializeField] private float victoryTime = 4f;
        [SerializeField] private AudioSource fail, start;
        private bool ready = true;
        Vector3 startGrav;

        public event Action OnBallLost, OnBallDropped, OnBallReady;

        private Vector3 startPos;

        public bool Ready { get => ready; set => ready = value; }

        // Start is called before the first frame update
        void Start()
        {
            startGrav = Physics.gravity;
            startPos = transform.position;
            body.maxLinearVelocity = 100f;
            ResetBall();
        }

        public void Drop()
        {
            Physics.gravity = startGrav;
            Ready = false;
            body.isKinematic = false;
            OnBallDropped?.Invoke();
            body.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            start.Play();
        }

        public void ResetBall()
        {
            Debug.Log("Reset ball");
            Physics.gravity = Vector3.zero;
            transform.position = startPos;
            body.isKinematic = true;
            if (!ready)
            {
                fail.Play();
            }
            OnBallReady?.Invoke();
        }

        public void VictoryAnimation()
        {
            IEnumerator animation()
            {
                float phase = 0f;
                Vector3 beginPos = transform.position;
                body.isKinematic = true;
                while (phase < 1f)
                {
                    phase += Time.deltaTime / victoryTime;
                    Vector3 intermediate = Vector3.Lerp(beginPos, startPos, phase);
                    intermediate.z = Mathf.Lerp(0f, 25f, Mathf.Abs(phase - 0.5f) + 0.5f);
                    transform.position = intermediate;
                    yield return null;
                }
                transform.position = startPos;
                ready = true;
                ResetBall();
            }
            StartCoroutine(animation());
        }

        // Update is called once per frame
        void Update()
        {
            if ((Input.GetKey(KeyCode.Space) || Input.touchCount > 0) && Ready)
            {
                Drop();
            }

            if (Input.GetKey(KeyCode.A))
            {
                body.AddForce(Vector3.left * 15f, ForceMode.Force);
            } else if (Input.GetKey(KeyCode.D))
            {
                body.AddForce(Vector3.right * 15f, ForceMode.Force);
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