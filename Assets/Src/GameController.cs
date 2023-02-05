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

        [SerializeField] private GameObject startTip;

        [SerializeField] private GameObject directionalHint;

        private GameEntry currentTarget;

        private List<GameEntry> entries;

        [SerializeField] private int lives = 3;
        public void Start()
        {
            generator.OnLevelReady += Generator_OnLevelReady;
        }

        private void Generator_OnLevelReady(List<GameEntry> obj)
        {
            entries = obj;
            PickTarget();
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
        }
    }
}