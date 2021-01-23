using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///
///</summary>
[CreateAssetMenu()]
public class Monster : ScriptableObject{

    public List<MonsterBodyPart> bodyParts;

    public List<MonsterPerk> perkList;

    public string monsterName;

    public string monsterDesc;

    public int meleeAttackCount;

    public int strength;

    public int movement;
}

[System.Serializable]
public class MonsterBodyPart {

    public string name;

    public int toughness;

    public int hp;

    public void damageBodyPart(int damage) {
        hp -= damage;
    }
}