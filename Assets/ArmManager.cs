using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArmManager : MonoBehaviour
{
    public GameObject ArmPrefab;
    public float TimeDownForSpawn = 10.0f;
    public float MaxLightHeight = 2.0f;
    public float DistanceSpawnFromPlayer = 10.0f;

    private float timeDownTimer = 0f;
    
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.LightMovementScript.Height <= MaxLightHeight) {
            timeDownTimer += Time.deltaTime;
            if(timeDownTimer >= TimeDownForSpawn) {
                Vector3 rndPosition = GetRandomPositionNearPlayer();
                if (rndPosition != Vector3.zero) {
                    GameObject arm = Instantiate(ArmPrefab, rndPosition, Quaternion.identity);
                    arm.transform.parent = this.transform;
                    timeDownTimer = 0f;
                }
            }
        }
        else
            timeDownTimer = 0f;
    }

    private Vector3 GetRandomPositionNearPlayer() {
        for (int i = 0; i < 30; i++) {
            Vector3 rndPoint = GameManager.Instance.Player.transform.position + Random.insideUnitSphere * DistanceSpawnFromPlayer;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(rndPoint, out hit, 1.0f, NavMesh.AllAreas)){
                return hit.position;
            }
        }

        return Vector3.zero;
    }
}
