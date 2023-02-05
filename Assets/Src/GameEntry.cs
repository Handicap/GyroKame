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

        private Color targetColor = Color.cyan;
        private Color originalColor;

        private int hitsLeft = 3;

        public FileEntry Entry { get => entry; }
        public GameDirectory Parent { get => parent; set => parent = value; }
        public int HitsLeft { get => hitsLeft; set => hitsLeft = value; }

        public virtual void Initialize(FileEntry entry, GameDirectory parent, float horizontalPosition)
        {
            this.Parent = parent;
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
            originalColor = material.color;
        }

        private int GetDepth(int currentDepth)
        {
            int depth = currentDepth;
            if (Parent != null)
            {
                 depth = Parent.GetDepth(currentDepth + 1);
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
                material.color = basicColor;
            }
            StartCoroutine(flash());
        }

        public void SetAsTarget(bool target)
        {
            material.color = target ? targetColor : originalColor;

        }

        public void OnHit()
        {
            HitsLeft = HitsLeft - 1;
            if (HitsLeft < 1)
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
                transform.localScale = maxFlash;
                material.color = basicColor;
                GetComponent<Collider>().enabled = true;
                this.gameObject.SetActive(false);
            }
            StartCoroutine(animate());
            GetComponent<Collider>().enabled = false;
        }

        public virtual void MakeVisible()
        {
            this.gameObject.SetActive(true);
            IEnumerator animate()
            {
                float phase = 0f;
                float speed = 15f;
                Vector3 startScale = new Vector3(0.25f, 1f, 0f);
                while (phase < 1f)
                {
                    transform.localScale = Vector3.Lerp(startScale, Vector3.one, phase);
                    phase += Time.deltaTime * speed;
                    yield return null;
                }
                transform.localScale = Vector3.one;
            }
            StartCoroutine(animate());
        }
        public virtual void MakeInvisible()
        {
            IEnumerator animate()
            {
                float phase = 0f;
                float speed = 15f;
                Vector3 startScale = new Vector3(0.25f, 1f, 0f);
                while (phase < 1f)
                {
                    transform.localScale = Vector3.Lerp(Vector3.one, startScale, phase);
                    phase += Time.deltaTime * speed;
                    yield return null;
                }
                transform.localScale = Vector3.one;
                this.gameObject.SetActive(false);
            }
            StartCoroutine(animate());
        }

    }
}