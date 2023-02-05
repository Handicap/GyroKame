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
        [SerializeField] private GameObject gameoverPanel;

        [SerializeField] private GameObject startTip;
        [SerializeField] private GameObject startTipPc;

        [SerializeField] private GameObject directionalHint;

        [SerializeField] private CameraController cameraController;

        private GameEntry currentTarget;
        private List<GameEntry> currentPath = new List<GameEntry>();
        private List<GameEntry> currentPathLeft = new List<GameEntry>();

        [SerializeField] private GameEntry pointerTarget;
        [SerializeField] private GameObject gravityHelper;

        List<float> levels = new List<float>();

        private List<GameEntry> entries;

        [SerializeField] private int lives = 3;
        [SerializeField] private int maxLives = 3;

        [SerializeField] private int score = 0;

        [SerializeField] private float answerTime = 1.5f;

        [SerializeField] private AudioSource cleared, success, wrongBlock;


        public void Start()
        {
            generator.OnLevelReady += Generator_OnLevelReady;
            ball.OnBallLost += Ball_OnBallLost;
            ball.OnBallReady += ShowAnswer;
            ball.OnBallDropped += Ball_OnBallDropped;

            livesText.text = lives.ToString();
            scoreText.text = score.ToString();
            gameoverPanel.SetActive(false);
            //ball.Ready = true;
            if (Application.platform == RuntimePlatform.Android)
            {
                startTip.SetActive(true);
                startTipPc.SetActive(false);
            }
            else
            {
                startTip.SetActive(false);
                startTipPc.SetActive(true);
            }
        }

        private void Ball_OnBallDropped()
        {
            startTip.SetActive(false);
            startTipPc.SetActive(false);
        }

        private void ShowAnswer()
        {
            if (currentPath.Count < 1) return;
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

                rootDir.gameObject.SetActive(true);
                rootDir.ResetEntry();

                foreach (var item in entries)
                {
                    item.ResetEntry();
                }

                for (int i = 0; i < currentPath.Count; i++)
                {
                    var item = currentPath[i];
                    cameraController.Target = item.transform;
                    yield return new WaitForSeconds(answerTime / 3f);
                    yield return new WaitForSeconds(answerTime / 3f);
                    item.HighlightFlashBlock();
                    yield return new WaitForSeconds(answerTime / 3f);
                }
                foreach (var item in entries)
                {
                    //item.ResetEntry();
                    item.gameObject.SetActive(false);
                }
                cameraController.Target = ball.transform;
                Debug.Log("Finished answer");
                rootDir.MakeVisible();
                //rootDir.ActivateChildren();
                if (Application.platform == RuntimePlatform.Android)
                {
                    startTip.SetActive(true);
                } else
                {
                    startTipPc.SetActive(true);
                }
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
            if (lives > 0)
            {
                lives--;
                livesText.text = lives.ToString();
            }
            if (lives < 1)
            {
                Debug.Log("Ran out of lives");
                gameoverPanel.SetActive(true);
            } else
            {
                //ShowAnswer();
                ball.ResetBall();
            }
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
            //rootDir.ActivateChildren();
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
                score += obj.GetDepth() * 2;
                Debug.Log("Target found!");
                success.Play();
                ball.VictoryAnimation();
                PickTarget();
            } else if (currentPath.Contains(obj))
            {
                cleared.Play();
                score += 1 * obj.GetDepth();
            } else
            {
                wrongBlock.Play();
            }
            scoreText.text = score.ToString();
        }

        private void Update()
        {
            if (currentTarget != null && currentPathLeft.Count > 0)
            {
                pointerTarget = currentPathLeft.Last();
                Vector3 midWay = Vector3.Lerp(ball.transform.position, pointerTarget.transform.position, 0.5f);
                if (Vector3.Distance(midWay, ball.transform.position) > 3f)
                {
                    directionalHint.transform.position = Vector3.MoveTowards(ball.transform.position, pointerTarget.transform.position, 3f);
                } else
                {
                    directionalHint.transform.position = midWay;
                }
                directionalHint.transform.LookAt(pointerTarget.transform, Vector3.forward);
                
            }
            gravityHelper.transform.LookAt(Physics.gravity * 10000f, Vector3.forward);
            /*
            if (currentPathLeft.Count > 0 && currentPathLeft.Last().transform.position.y > ball.transform.position.y)
            {
                currentPathLeft.RemoveAt(currentPathLeft.Count - 1);
            }
            */
            if ((Input.GetKey(KeyCode.Space) || Input.touches.Length > 0) && lives < 1)
            {
                lives = maxLives;
                score = 0;
                scoreText.text = "0";
                livesText.text = lives.ToString();
                ball.ResetBall();
                PickTarget();
                ShowAnswer();
                gameoverPanel.SetActive(false);
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