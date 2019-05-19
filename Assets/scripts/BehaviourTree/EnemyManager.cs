using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    public static SoundUnityEvent soundUnityEvent;

    private List<BehaviourTree> enemies;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null) {
            Instance = this;
            if (soundUnityEvent == null)
                soundUnityEvent = new SoundUnityEvent();
        }

        soundUnityEvent.AddListener(OnSound);
        enemies = new List<BehaviourTree>();
    } 

    public static void AddEnemy(BehaviourTree enemy) {
        Instance.enemies.Add(enemy);
    }

    private void OnSound(float range, float loudness, Vector3 pos) {
        foreach (BehaviourTree enemy in enemies) {
            if ((pos - enemy.transform.position).sqrMagnitude <= range * range) {
                enemy.IncreaseAnger(loudness);
            }
        }
    }
}
