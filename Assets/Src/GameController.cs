using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

        [SerializeField] private CameraController cameraController;

        private GameEntry currentTarget;
        private List<GameEntry> currentPath;
        private List<GameEntry> currentPathLeft = new List<GameEntry>();

        List<float> levels = new List<float>();

        private List<GameEntry> entries;

        [SerializeField] private int lives = 3;
        [SerializeField] private int maxLives = 3;

        [SerializeField] private int score = 0;

        [SerializeField] private float answerTime = 1.5f;

        [SerializeField] private AudioSource cleared, success;


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

                for (int i = 0; i < currentPath.Count; i++)
                {
                    var item = currentPath[i];
                    GameDirectory dir = item.gameObject.GetComponent<GameDirectory>();
                    if (dir != null)
                    {
                        dir.ActivateChildren();
                    }
                }

                for (int i = 0; i < currentPath.Count; i++)
                {
                    var item = currentPath[i];
                    cameraController.Target = item.transform;
                    yield return new WaitForSeconds(answerTime / 2f);
                    item.HighlightFlashBlock();
                    yield return new WaitForSeconds(answerTime / 2f);
                }
                foreach (var item in entries)
                {
                    item.ResetEntry();
                    item.gameObject.SetActive(false);
                }
                cameraController.Target = ball.transform;
                Debug.Log("Finished answer");
                rootDir.MakeVisible();
                rootDir.ActivateChildren();
                startTip.SetActive(true);
                currentPathLeft.Clear();
                currentPathLeft.AddRange(currentPath);
                ball.Ready = true;
            }
            StartCoroutine(routine());

        }

        private void Ball_OnBallLost()
        {
            foreach (var item in entries)
            {
                item.gameObject.SetActive(false);
            }
            lives--;
            livesText.text = lives.ToString();
            ShowAnswer();
            ball.ResetBall();
        }

        private void Generator_OnLevelReady(List<GameEntry> obj, GameDirectory root)
        {
            rootDir = root;
            entries = obj;
            foreach (var item in entries)
            {
                item.OnBlockDestroyed += Item_OnBlockDestroyed;
            }
            Debug.Log("Created " + obj.Count + " nodes");
            PickTarget();
            rootDir.MakeVisible();
            rootDir.ActivateChildren();
        }

        private void Item_OnBlockDestroyed(GameEntry obj)
        {
            if (currentPathLeft.Contains(obj))
            {
                currentPathLeft.Remove(obj);
            }
            foreach (var item in obj.Siblings)
            {
                item.LockBlock();
            }
            if (obj == currentTarget)
            {
                score += obj.GetDepth();
                Debug.Log("Target found!");
                success.Play();
                ball.VictoryAnimation();
            } else
            {
                cleared.Play();
            }
            score += 1;
            scoreText.text = score.ToString();
        }

        private void Update()
        {
            if (currentTarget != null && currentPathLeft.Count > 0)
            {
                var target = currentPathLeft.Last();
                directionalHint.transform.position = Vector3.MoveTowards(ball.transform.position, target.transform.position, 3);
                directionalHint.transform.LookAt(target.transform);
            }
        }

        private void PickTarget()
        {
            int randomIndex = Random.Range(0, entries.Count);
            currentTarget = entries[randomIndex];
            Debug.Log("Random target is " + currentTarget + " of " + entries.Count);

            List<GameEntry> breadCrumbs = new List<GameEntry>();
            GameEntry nextInHierarchy = currentTarget.Parent;
            breadCrumbs.Add(currentTarget);
            while (nextInHierarchy != null)
            {
                breadCrumbs.Add(nextInHierarchy);
                nextInHierarchy = nextInHierarchy.Parent;
            }
            currentTarget.SetAsTarget(true);
            currentPath = breadCrumbs;
            currentPathLeft.AddRange(breadCrumbs);
            objectiveText.text = "Find " + currentTarget.Entry.name + " in " + breadCrumbs[breadCrumbs.Count - 2].name;
            ShowAnswer();
        }
    }
}