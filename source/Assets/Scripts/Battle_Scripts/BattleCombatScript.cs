using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Handler for the battle actions done by the hunters and the monsters.
///</summary>
public class BattleCombatScript : MonoBehaviour {

    List<QueuedAction> queue;

    ///<summary>
    /// 
    ///</summary>
    void Start() {
        
        queue = new List<QueuedAction>();
    }

    /// <summary>
    /// Function called to process the given action with the given targets by 
    /// the given user.
    /// </summary>
    /// <param name="processingAction"> The given action being processed
    ///   </param>
    /// <param name="givenTargets"> The list of lists of targets; each list 
    ///   is for a single action's targets; there will be more than one list if
    ///   there are follow up actions; each list correlates to an action in the
    ///   queue of follow up actions i.e. the first list will go to the first
    ///   action, the second list will go to the second action until there are
    ///   no more actions.</param>
    /// <param name="user">The user of the action Hopefully we will create a
    ///   a parent class to house hunter and monster scripts but for now this
    ///   function only accepts hunter battle puppet scripts.</param>
    public void processAction (BattleAction processingAction, 
                               List<List<System.Object>> givenTargets, 
                               ParentPuppetScript user) {

        user.setCurrentActionPointCount(user.getCurrentActionPointCount()
            - ((BattleAction)processingAction).apCost);

        // Based on the type of action being processed, relay the given
        //   parameters to a function to handle that specific type of action.
        switch(processingAction) {

            case MeleeAction meleeAction:
                processMeleeAction(meleeAction, givenTargets[0], 
                    user);
                break;
        }


        // If the processing action is a followup then there must be a 2nd list;
        //   recursively call this function with the followup action and
        //   the list of list of targets with the front most list popped out.
        if(processingAction is FollowupAction) {

            givenTargets.Remove(givenTargets[0]);

            processAction(((FollowupAction)processingAction).getFollowup(), 
                givenTargets, user);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="processingAction"></param>
    /// <param name="targets"></param>
    /// <param name="user"></param>
    private void processMeleeAction(MeleeAction processingAction,
                                    List<System.Object> targets, 
                                    ParentPuppetScript user) {

        Debug.LogError("The Hunter " + user.getPuppetName() + 
            " is using the action " + ((BattleAction)processingAction).name +
            " on the following targets:");

        foreach(System.Object target in targets) {
            Debug.LogError(((MonsterBodyPart)target).name);
        }

        int strengthModifier = processingAction.
            getStrengthModifierValue();

        int strength = modifyValue(user.getModifiedStrength(), strengthModifier,
                                   processingAction.getStrengthModifierType());

        float chanceToHit = processingAction.getChanceToHit();

        int piercing = processingAction.getPiercing();

        int damage = processingAction.getDamagae();

        int meleeHitCount = user.getModifiedMeleeAttackCount();

        for(int i = 0; i < targets.Count; i++) {
            for(int hits = 0; hits < meleeHitCount; hits ++){
                switch(targets[i]) {

                    case MonsterBodyPart monsterBodyPart:
                        
                        Debug.LogError("The target is a monster bodypart");

                        Debug.LogError("Chance to hit is " + chanceToHit);

                        // If the chance to hit succes, proceed, otherwise end this
                        if(Random.Range(0,99) < chanceToHit) {

                            Debug.LogError("HIT!");

                            // Remove some toughness from the monster through
                            //   the peircing value
                            float monsterToughness = monsterBodyPart.toughness - piercing;

                            // Clamp the monster toughness so that it is always
                            // greater than 0 (such that we do not divide by zero)
                            monsterToughness = Mathf.Clamp(monsterToughness, 1.0f, 
                                float.PositiveInfinity);
                            
                            // Get the chance to penetrate 
                            float chanceToPierce = user.getModifiedStrength() / 
                                (monsterToughness * 2);

                            Debug.LogError("The chance to pierce is: " + user.getModifiedStrength() + " / ((" + monsterBodyPart.toughness + " - " + piercing + ") * 2) = " + chanceToPierce);

                            // If it pens, then deal damag based on the given amount
                            if(Random.Range(0,99) < (float)(chanceToPierce * 100f)) {

                                Debug.LogError("PIERCE!");

                                Debug.LogError("Bodypart HP is " + monsterBodyPart.hp + " and damage is " + damage);

                                monsterBodyPart.damageBodyPart(damage);

                                Debug.LogError("Bodypart HP after damage is " + monsterBodyPart.hp);

                            } else {
                                Debug.LogError("DEFLECTED!");
                                // Tell user the hit reflected
                            }
                        } else {
                            Debug.LogError("MISS!");
                            // Tell user they missed
                        }
                        break;

                    default: 
                        Debug.LogError("Error! unknown action target from the" + 
                                    "current processing target: " + 
                                    targets[i].ToString());
                        break;

                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="initial"></param>
    /// <param name="modifier"></param>
    /// <param name="modificationType"></param>
    /// <returns></returns>
    public int modifyValue(int initial, int modifier,
                           ModificationType modificationType) {

        switch(modificationType) {

            case ModificationType.ADD:
                return initial + modifier;

            case ModificationType.SUBTRACT:
                return initial - modifier;

            case ModificationType.MULTIPLY:
                return initial * modifier;

            case ModificationType.DIVIDE:
                return initial / modifier;

            case ModificationType.REPLACE:
                return modifier;

            case ModificationType.ADD_DIVISION:
                int strengthAddition = initial / modifier;
                return initial + strengthAddition;

            default:
                Debug.LogError("Error! Unrecognized ModificationType enum given" 
                    + "in modifyValue function: " 
                    + modificationType.ToString());
                return 0;
        }
    }
}

public class QueuedAction {

    public BattleAction queuedAction {
        get;
        private set;
    }

    public int roundCount {
        get;
        private set;
    }

    public QueueType[] queueTypes {
        get;
        private set;
    }

    public QueuedAction (BattleAction givenQueuedAction, int givenRoundCount,
                         QueueType[] givenQueueTypes) {
    
        queuedAction = givenQueuedAction;
        
        roundCount = givenRoundCount;

        queueTypes = givenQueueTypes;
    }

    public enum QueueType {
        HUNTER_START, HUNTER_END, MONSTER_START, MONSTER_END,
        ROUND_START, ROUND_END, EVERY_ROUND
    }
}