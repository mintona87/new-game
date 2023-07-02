using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSavedData 
{

    public int ATK;
    public int DEF;
    public int SPD;
    public int LUCK;

    public int Gold;

    public int Honor;

    public PlayerSavedData(int aTK, int dEF, int sPD, int lUCK, int gold, int honor)
    {
        ATK = aTK;
        DEF = dEF;
        SPD = sPD;
        LUCK = lUCK;
        Gold = gold;
        Honor = honor;
    }
}
