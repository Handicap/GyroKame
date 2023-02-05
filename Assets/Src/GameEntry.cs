using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyroKame
{
    public class GameEntry : MonoBehaviour
    {

        [SerializeField] private GameDirectory parent;
        [SerializeField] private FileEntry entry;
        [SerializeField] private TMPro.TextMeshPro text;

        [SerializeField] private Vector3 originalPosition;

        [SerializeField] private float depthStep = 3f;

        private float perlinSeed = 0;

        private float perlinSpeed = 0.5f;
        private float jitterAmount = 0.75f;

        [SerializeField] private GameObject graphic;
        [SerializeField] private Material material;

        private int hitsLeft = 3;

        public virtual void Initialize(FileEntry entry, GameDirectory parent, float horizontalPosition)
        {
            this.parent = parent;
            this.entry = entry;
            text.text = entry.name;
            name = entry.ToString();
            if (parent != null)
            {
                //transform.parent = parent.transform;
                parent.AddToChildren(this);
            }
            transform.position = new Vector3(horizontalPosition, 4f * -GetDepth(0), 0);
            originalPosition = transform.position;
            perlinSeed = Random.Range(-1f, 1f);
            gameObject.SetActive(false);
            material = graphic.GetComponent<Renderer>().material;
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
            graphic.transform.position = new Vector3(originalPosition.x + perlin, originalPosition.y + perlin2, 0f);
        }

        public void FlashBlock()
        {
            IEnumerator flash()
            {
                float phase = 0f;
                float speed = 10f;
                Vector3 maxFlash = Vector3.one * 1.5f;
                Color basicColor = material.color;
                Color flashColor = Color.white;
                while (phase < 1f)
                {
                    transform.localScale = Vector3.Lerp(Vector3.one, maxFlash, phase);
                    phase += Time.deltaTime * speed;
                    material.color = Color.Lerp(basicColor, flashColor, phase);
                    yield return null;
                }
                phase = 0f;
                while (phase < 1f)
                {
                    transform.localScale = Vector3.Lerp(maxFlash, Vector3.one, phase);
                    phase += Time.deltaTime * speed;
                    material.color = Color.Lerp(flashColor, basicColor, phase);
                    yield return null;
                }
            }
            StartCoroutine(flash());
        }

        public void OnHit()
        {
            hitsLeft = hitsLeft - 1;
            if (hitsLeft < 1)
            {
                this.DestroyBlock();
            } else
            {
                FlashBlock();
            }
        }

        public virtual void DestroyBlock()
        {
            IEnumerator animate()
            {
                float phase = 0f;
                float speed = 10f;
                Vector3 maxFlash = new Vector3(3f, 0f, 0f);
                Color basicColor = material.color;
                Color flashColor = Color.white;
                while (phase < 1f)
                {
                    transform.localScale = Vector3.Lerp(Vector3.one, maxFlash, phase);
                    phase += Time.deltaTime * speed;
                    material.color = Color.Lerp(basicColor, flashColor, phase);
                    yield return null;
                }
                Destroy(this.gameObject);
            }
            StartCoroutine(animate());
        }
    }
}