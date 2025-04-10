using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    [Header("Round Settings")]
    public float timeBetweenRounds = 10f;
    public int currentRound = 1;

    [Header("Events")]
    public UnityEvent OnRoundStart = new UnityEvent();
    public UnityEvent OnRoundEnd = new UnityEvent();
    public DialogueLoader dl;

    private bool roundActive = false;
    [Header("UI Components")]
    public TMP_Text currentWave;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(RoundLoop());
    }

    private IEnumerator RoundLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenRounds);
            Debug.Log($"Round {currentRound} starting!");

            StartRound();
            dl.StartConversation("Echo");

            // Wait until spawning starts to avoid skipping rounds
            yield return new WaitUntil(() => EnemySpawner.Instance.IsSpawning());

            // Wait until all enemies are defeated and spawning has stopped
            yield return new WaitUntil(() => !EnemySpawner.Instance.IsThereEnemy() && !EnemySpawner.Instance.IsSpawning());
            EndRound();
        }
    }



    private void StartRound()
    {
        roundActive = true;
        currentRound++;
        OnRoundStart.Invoke();
        EnemySpawner.Instance.StartRound();
    }

    private void EndRound()
    {
        roundActive = false;
        OnRoundEnd.Invoke();
        EnemySpawner.Instance.StopSpawning();
        ResetAllTowers(); // Call the reset method on all towers
    }

    private void ResetAllTowers()
    {
        foreach (BaseTower tower in FindObjectsOfType<BaseTower>())
        {
            tower.Reset(); 
        }
    }

}
