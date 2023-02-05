using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyroKame
{
    public class Sway : MonoBehaviour
    {
        [SerializeField] private float startOffset = 0f;
        [SerializeField] private float maxAngle, minAngle;
        [SerializeField] private float speed;
        // Update is called once per frame
        void Update()
        {
            float sine = (1.0f + Mathf.Sin(startOffset + Time.time * speed)) / 2.0f;
            transform.localRotation = Quaternion.Lerp(Quaternion.AngleAxis(minAngle, Vector3.forward), Quaternion.AngleAxis(maxAngle, Vector3.forward), sine);
        }
    }
}