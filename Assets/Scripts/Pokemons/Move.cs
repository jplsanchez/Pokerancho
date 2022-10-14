using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MoveBase
{

    public int CurrentPP { get; set; }

    public Move() : base()
    {
        CurrentPP = base.PP;
    }

    public Move(MoveBase moveBase) : base()
    {
        name = moveBase.Name;
        description = moveBase.Description;
        type = moveBase.Type;
        power = moveBase.Power;
        accuracy = moveBase.Accuracy;
        priotity = moveBase.Priotity;
        pp = moveBase.PP;
        statType = moveBase.StatType;
        statStage = moveBase.StatStage;
        isSpecial = moveBase.IsSpecial;

        CurrentPP = base.PP;
    }
}
