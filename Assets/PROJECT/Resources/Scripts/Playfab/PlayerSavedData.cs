using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSavedData 
{

    public float ATK = 1;
    public float DEF = 1;
    public float SPD = 1;
    public float LUCK = 1;

    public int Gold;

    public int Honor;

    public PlayerSavedData(float aTK, float dEF, float sPD, float lUCK, int gold, int honor)
    {
        ATK = aTK;
        DEF = dEF;
        SPD = sPD;
        LUCK = lUCK;
        Gold = gold;
        Honor = honor;
    }
}
