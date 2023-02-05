using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyroKame
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private LevelGenerator generator;
        [SerializeField] private PlayerBall ball;
        [SerializeField] private GameDirectory rootDir;

        [SerializeField] private TMPro.TextMeshProUGUI objectiveText;
        [SerializeField] private TMPro.TextMeshProUGUI scoreText;
        [SerializeField] private TMPro.TextMeshProUGUI livesText;

        [SerializeField] private GameObject startTip;

        [SerializeField] private GameObject directionalHint;

        [SerializeField] private CameraController camera;

        private GameEntry currentTarget;

        private List<GameEntry> entries;

        [SerializeField] private int lives = 3;
        [SerializeField] private int maxLives = 3;

        [SerializeField] private int score = 0;

        [SerializeField] private float answerTime = 1.5f;

        public void Start()
        {
            generator.OnLevelReady += Generator_OnLevelReady;
            ball.OnBallLost += Ball_OnBallLost;
            ball.OnBallDropped += Ball_OnBallDropped;

            livesText.text = lives.ToString();
            scoreText.text = score.ToString();
            //ball.Ready = true;
        }

        private void Ball_OnBallDropped()
        {
            startTip.SetActive(false);
        }

        private void ShowAnswer()
        {
            ball.Ready = false;
            Debug.Log("Showing answer");

            IEnumerator routine()
            {
                List<GameEntry> breadCrumbs = new List<GameEntry>();
                breadCrumbs.Add(currentTarget);
                GameEntry nextInHierarchy = currentTarget.Parent;
                while (nextInHierarchy != null)
                {
                    breadCrumbs.Add(nextInHierarchy);
                    nextInHierarchy = nextInHierarchy.Parent;
                }

                foreach (var item in breadCrumbs)
                {
                    GameDirectory dir = item.gameObject.GetComponent<GameDirectory>();
                    if (dir != null)
                    {
                        dir.ActivateChildren();
                    }
                }

                foreach (var item in breadCrumbs)
                {
                    camera.Target = item.transform;
                    yield return new WaitForSeconds(answerTime);
                }
                foreach (var item in entries)
                {
                    item.gameObject.SetActive(false);
                }
                camera.Target = ball.transform;
                Debug.Log("Finished answer");
                rootDir.MakeVisible();
                rootDir.ActivateChildren();
                startTip.SetActive(true);
                ball.Ready = true;
            }
            StartCoroutine(routine());

        }

        private void Ball_OnBallLost()
        {
            foreach (var item in entries)
            {
                item.gameObject.SetActive(false);
                item.HitsLeft = 3;
            }
            rootDir.HitsLeft = 3;
            lives--;
            livesText.text = lives.ToString();
            ShowAnswer();
            ball.ResetBall();
        }

        private void Generator_OnLevelReady(List<GameEntry> obj, GameDirectory root)
        {
            rootDir = root;
            entries = obj;
            PickTarget();
            rootDir.MakeVisible();
            rootDir.ActivateChildren();
        }

        private void Update()
        {
            if (currentTarget != null)
            {
                directionalHint.transform.position = Vector3.MoveTowards(ball.transform.position, currentTarget.transform.position, 3);
            }
        }

        private void PickTarget()
        {
            int randomIndex = Random.Range(0, entries.Count);
            currentTarget = entries[randomIndex];
            Debug.Log("Random target is " + currentTarget);

            List<GameEntry> breadCrumbs = new List<GameEntry>();
            GameEntry nextInHierarchy = currentTarget.Parent;
            while (nextInHierarchy != null)
            {
                breadCrumbs.Add(nextInHierarchy);
                nextInHierarchy = nextInHierarchy.Parent;
            }
            currentTarget.SetAsTarget(true);
            objectiveText.text = "Find " + currentTarget.Entry.name + " in " + breadCrumbs[breadCrumbs.Count - 2].name;
            ShowAnswer();
        }
    }
}