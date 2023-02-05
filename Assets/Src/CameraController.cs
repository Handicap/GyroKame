using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyroKame
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Rigidbody target;
        [SerializeField] private float maxZoom = 20f;
        [SerializeField] private float minZoom = 1f;
        private void Start()
        {
            
        }
        // Update is called once per frame
        void Update()
        {
            float velocityZoom = Mathf.Lerp(minZoom, maxZoom, target.velocity.magnitude / 50f);
            transform.position = new Vector3(target.transform.position.x, target.transform.position.y, velocityZoom);
        }

    }
}