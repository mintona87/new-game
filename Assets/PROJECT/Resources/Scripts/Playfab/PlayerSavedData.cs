using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSavedData 
{

    public int ATK = 1;
    public int DEF = 1;
    public int SPD = 1;
    public int LUCK = 1;

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
