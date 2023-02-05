using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

        [SerializeField] Color flashTargetColor;

        [SerializeField] private int maxHealth = 3;
        private int health = 3;

        public FileEntry Entry { get => entry; }
        public GameDirectory Parent { get => parent; set => parent = value; }
        public int Health { get => health; set => health = value; }

        public bool Locked { get; set; }

        [SerializeField] private AudioSource bounce, cleared, highlight;

        [SerializeField] private Coroutine flashRoutine;

        public List<GameEntry> Siblings { get
            {
                if (parent)
                {
                    List<GameEntry> siblings = new List<GameEntry>(parent.Children);
                    siblings.Remove(this);
                    return siblings;
                }
                return new List<GameEntry>();
            }
        }

        public event Action<GameEntry> OnBlockDestroyed;

        public void ResetEntry()
        {
            Locked = false;
            material.color = originalColor;
            graphic.transform.localScale = Vector3.one;
            transform.localScale = Vector3.one;
            Health = 3;
        }

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
            perlinSeed = UnityEngine.Random.Range(-1f, 1f);
            gameObject.SetActive(false);
            material = graphic.GetComponent<Renderer>().material;
            originalColor = material.color;
            health = maxHealth;
        }

        public int GetDepth(int currentDepth = 0)
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
                float hpLeft = (float) health / (float) maxHealth;
                Color basicColor = Color.Lerp(flashTargetColor, originalColor, hpLeft);
                Color flashColor = Color.white;
                bounce.Play();
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
                flashRoutine = null;
            }
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }
            flashRoutine = StartCoroutine(flash());
        }
        public void HighlightFlashBlock()
        {
            IEnumerator flash()
            {
                float phase = 0f;
                float speed = 5f;
                Vector3 maxFlash = Vector3.one * 2.0f;
                Color basicColor = originalColor;
                Color flashColor = Color.green;
                highlight.Play();
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
                flashRoutine = null;
            }
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }
            flashRoutine = StartCoroutine(flash());
        }

        public void SetAsTarget(bool target)
        {
            material.color = target ? targetColor : originalColor;

        }

        public void OnHit()
        {
            if (Locked) return;
            Health = Health - 1;
            if (Health < 1)
            {
                DestroyBlock();
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
                Color flashColor = Color.white;
                while (phase < 1f)
                {
                    transform.localScale = Vector3.Lerp(Vector3.one, maxFlash, phase);
                    phase += Time.deltaTime * speed;
                    material.color = Color.Lerp(originalColor, flashColor, phase);
                    yield return null;
                }
                transform.localScale = maxFlash;
                material.color = originalColor;
                GetComponent<Collider>().enabled = true;
                this.gameObject.SetActive(false);
                OnBlockDestroyed?.Invoke(this);
                //cleared.Play();
            }
            StartCoroutine(animate());
            GetComponent<Collider>().enabled = false;
        }
        public virtual void LockBlock()
        {
            IEnumerator animate()
            {
                float phase = 0f;
                float speed = 10f;
                Vector3 maxFlash = new Vector3(1.5f, 1f, 1f);
                Color flashColor = Color.grey;
                while (phase < 1f)
                {
                    graphic.transform.localScale = Vector3.Lerp(Vector3.one, maxFlash, phase);
                    phase += Time.deltaTime * speed;
                    material.color = Color.Lerp(originalColor, flashColor, phase);
                    yield return null;
                }
                Locked = true;
                transform.localScale = maxFlash;
                material.color = flashColor;
                
            }
            StartCoroutine(animate());
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
                    graphic.transform.localScale = Vector3.Lerp(startScale, Vector3.one, phase);
                    phase += Time.deltaTime * speed;
                    yield return null;
                }
                graphic.transform.transform.localScale = Vector3.one;
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
                    graphic.transform.localScale = Vector3.Lerp(Vector3.one, startScale, phase);
                    phase += Time.deltaTime * speed;
                    yield return null;
                }
                graphic.transform.localScale = Vector3.one;
                this.gameObject.SetActive(false);
            }
            StartCoroutine(animate());
        }

    }
}