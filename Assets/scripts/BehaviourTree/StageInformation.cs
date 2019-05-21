using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageInformation", menuName = "Enemy", order = 1)]
public class StageInformation : ScriptableObject
{
    public float AngerToStage;
    public float Size;
    public float ViewRange;
    public float AttackRange;
    public int Damage;
    public float AttackTime;
    public float Speed;
}
