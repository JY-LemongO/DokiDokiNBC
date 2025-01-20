using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData",menuName = "Monster/MonsterData",order = 0) ]
public class EnemySO : ScriptableObject
{
    public RegularEnemy.MonsterSpecies monsterSpecies; 
    [Header("Attck Info")]
    public float power;
    public float delay;

    [Header("Skill Info")]
    public float skillPower;
    public float skillDelay;
    public GameObject skill;
}
