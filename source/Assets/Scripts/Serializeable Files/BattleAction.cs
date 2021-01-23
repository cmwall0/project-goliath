using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Houses the basic info for an action the hunter can do. Usually an action
/// will inherit the BattleAction class, a single action type 
/// (i.e. MeleeAction), and, if it has a follow up action, the FollowupAction 
/// intereface.
/// </summary>
public abstract class BattleAction {

    /// <summary>
    /// The list of target infos needed for the action to run. Each TargetInfo
    /// object in the list must be fufilled in the sense that the count for
    /// each TargetInfo must be accounted for before action execution. IE
    /// targetInfos = {ti0.targetTypes = {MonsterBodyPart, Ground} 
    ///                ti0.targetCount = 1, ti1.targetTypes = {Hunter, Monster}
    ///                ti1.targetCount = 2} is the same as saying:
    /// Targets = (MonsterBodyPart OR Ground) AND (Hunter OR Monster) AND
    ///                  (Hunter OR Monster).
    /// </summary>
    public List<TargetInfo> targetInfos {
        get;
        protected set;
    }

    private int totalTargetCountCache = -1;

    public int totalTargetCount {

        get {
            if(totalTargetCountCache == -1) {
                            
                totalTargetCountCache = processCache();
            }

            return totalTargetCountCache;
        }

        private set {

            totalTargetCountCache = value;
        }
    }

    public string name {
        get;
        protected set;
    }

    public string desc {
        get;
        protected set;
    }

    public int range {
        get;
        protected set;
    }

    public int apCost {
        get;
        protected set;
    }

    // -1 uses implies infinite uses
    public int uses {
        get;
        protected set;
    }

    /// <summary>
    /// USed to store the max amount of targets selected
    /// </summary>
    /// <returns></returns>
    private int processCache() {
        if(targetInfos == null) {

            Debug.LogError("List of targets for the action " + name + 
            " is null");

            return -1;
        }

        int cache = 0;

        foreach(TargetInfo target in targetInfos) {

            cache += target.targetCount;
        }

        return cache;
    }
}

/// <summary>
/// 
/// </summary>
public interface FollowupAction {
    BattleAction getFollowup();
}

/// <summary>
/// 
/// </summary>
public interface MeleeAction {
    ModificationType getStrengthModifierType();

    int getStrengthModifierValue();
    
    float getChanceToHit();

    int getPiercing();

    int getDamagae();

}

/// <summary>
/// 
/// </summary>
public class TargetInfo {
    public List<ActionTarget> targetTypes {
        get;
        private set;
    }

    public int targetCount {
        get;
        private set;
    }

    public bool containsMonsterBodyPart;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="givenTargets"></param>
    /// <param name="givenTargetCount"></param>
    public TargetInfo(List<ActionTarget> givenTargets, int givenTargetCount, 
        bool givenContainsMonsterBodyPart) {

        targetTypes = givenTargets;

        targetCount = givenTargetCount;

        containsMonsterBodyPart = givenContainsMonsterBodyPart;
    }
}

/// <summary>
/// 
/// </summary>
public enum ActionTarget {
    MONSTER_PART, MONSTER, SELF
}

/// <summary>
/// ADD:          STAT + MODIFIER
/// SUBTRACT:     STAT - MODIFIER
/// ADD_DIVISION: STAT + (STAT/MODIFIER)
/// </summary>
public enum ModificationType {

    ADD, SUBTRACT, MULTIPLY, DIVIDE, REPLACE, ADD_DIVISION
}