using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyroKame
{
    public class GameEntry : MonoBehaviour
    {

        [SerializeField] private GameDirectory parent;
        [SerializeField] private FileEntry entry;

        [SerializeField] private Vector3 originalPosition;

        [SerializeField] private float depthStep = 3f;

        private float perlinSeed = 0;

        private float perlinSpeed = 0.5f;
        private float jitterAmount = 0.75f;

        public void Initialize(FileEntry entry, GameDirectory parent, float horizontalPosition)
        {
            this.parent = parent;
            this.entry = entry;
            name = entry.ToString();
            if (parent != null)
            {
                transform.parent = parent.transform;
            }
            transform.position = new Vector3(horizontalPosition, 4f * -GetDepth(0), 0);
            originalPosition = transform.position;
            perlinSeed = Random.Range(-1f, 1f);
        }

        private int GetDepth(int currentDepth)
        {
            int depth = currentDepth;
            if (parent != null)
            {
                 depth = parent.GetDepth(currentDepth + 1);
            }
            return depth;
        }

        private void Update()
        {
            float perlin = (Mathf.PerlinNoise(Time.time * perlinSpeed + perlinSeed, 0) - 0.5f) * jitterAmount;
            float perlin2 = (Mathf.PerlinNoise(0, Time.time * perlinSpeed + perlinSeed) - 0.5f) * jitterAmount;
            transform.position = new Vector3(originalPosition.x + perlin, originalPosition.y + perlin2, 0f);
        }
    }
}