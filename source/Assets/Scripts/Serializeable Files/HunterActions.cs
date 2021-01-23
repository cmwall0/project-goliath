using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Punch : BattleAction, MeleeAction {

    private static Punch instance;

    /// <summary>
    /// 
    /// </summary>
    private Punch () {



        List<ActionTarget> actionTargetList = new List<ActionTarget>();

        actionTargetList.Add(ActionTarget.MONSTER_PART);

        targetInfos = new List<TargetInfo>() {

            new TargetInfo(actionTargetList, 1, true)
        };

        name = this.GetType().Name;

        desc = "Punch a monster.";

        range = 2;

        apCost = 1;

        uses = -1;
    }

    public static BattleAction getAction() {

        if(instance == null) {

            instance = new Punch();
        }

        return instance;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public ModificationType getStrengthModifierType() {

        return ModificationType.ADD;
    }

    public int getStrengthModifierValue() {

        return 0;
    }
    

    public float getChanceToHit() {

        return 50;
    }

    public int getPiercing() {

        return 0;
    }

    public int getDamagae() {

        return 1;
    }

}

public class DoublePunch : BattleAction, MeleeAction, FollowupAction {

    private static DoublePunch instance;

    /// <summary>
    /// 
    /// </summary>
    private DoublePunch () {
        
        List<ActionTarget> actionTargetList = new List<ActionTarget>();

        actionTargetList.Add(ActionTarget.MONSTER_PART);

        targetInfos = new List<TargetInfo>() {

            new TargetInfo(actionTargetList, 1, true)
        };

        name = this.GetType().Name;

        desc = "Double punch a monster.";

        range = 2;

        apCost = 2;

        uses = -1;
    }

    public static BattleAction getAction() {

        if(instance == null) {

            instance = new DoublePunch();
        }

        return instance;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public ModificationType getStrengthModifierType() {

        return ModificationType.ADD;
    }

    public int getStrengthModifierValue() {

        return 0;
    }
    

    public float getChanceToHit() {

        return 25;
    }

    public int getPiercing() {

        return 0;
    }

    public int getDamagae() {

        return 1;
    }

    public BattleAction getFollowup() {

        return Punch.getAction();
    }
}
