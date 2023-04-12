using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats 
{
    public int HP = 100;
    public int MaxHP = 100;
    public bool HasCharged = false;
    public bool isDefending = false;
    public bool isStunned = false;

    public int ATK = 1;
    public int DEF = 1;
    public int SPD = 1;
    public int LUCK = 1;

    public int Gold;
    public PlayerStats(int hP,int maxHP,bool hasCharged,bool IsDefending,bool IsStunned,int aTK,int dEF,int sPD,int lUCK, int gold)
    {
        HP = hP;
        MaxHP = maxHP;
        HasCharged = hasCharged;
        isDefending = IsDefending;
        isStunned = IsStunned;
        ATK = aTK;
        DEF = dEF;
        SPD = sPD;
        LUCK = lUCK;
        Gold = gold;
    }
}
