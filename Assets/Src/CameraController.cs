using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyroKame
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float maxZoom = 20f;
        [SerializeField] private float minZoom = 1f;

        public Transform Target { get => target; set => target = value; }

        private void Start()
        {
            
        }
        // Update is called once per frame
        void Update()
        {
            /*
            float velocityZoom = Mathf.Lerp(minZoom, maxZoom, target.velocity.magnitude / 50f);
            transform.position = new Vector3(target.transform.position.x, target.transform.position.y, velocityZoom);
            */
            Vector3 targetVec = new Vector3(Target.transform.position.x, Target.transform.position.y, maxZoom);
            // speed up a bit with longer distances
            float distance = Vector3.Distance(targetVec, transform.position);
            float distanceFactor = distance / 10f;
            float step = Time.deltaTime * distanceFactor;
            
            transform.position = Vector3.Lerp(transform.position, targetVec, step);
        }

    }
}