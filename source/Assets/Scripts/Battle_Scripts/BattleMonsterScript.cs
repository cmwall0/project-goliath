using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

///<summary>
/// Used the manipulate the monster through movement, sprite changes,
/// hit detection, and actions
///</summary>
[RequireComponent(typeof(NavMeshAgent))]
public class BattleMonsterScript : MonoBehaviour, ParentPuppetScript {

    private const string MASTER_BATTLE_LOOP = "masterBattleLoop",
                         SELECTED_INDICATOR = "selectedIndicator",
                         DESTINATION_INDICATOR = "destinationObject";

    public int currentActionPointCount {
        get;
        private set;
    }

    public int meleeAttackCountModifier {
        get;
        private set;
    }

    public int actionPointCountModifier {
        get;
        private set;
    }

    public int actionRefillModifier {
        get;
        private set;
    }

    public int toughnessModifier {
        get;
        private set;
    }

    public int strengthModifier {
        get;
        private set;
    }

    public int movementModifier {
        get;
        private set;
    }

    public List<BattleAction> battleActions {
        get;
        private set;
    }

    private NavMeshAgent navMeshAgent;

    private NavMeshPath path;

    private Monster connectedMonster;

    private MasterBattleScript masterBattleScript;

    void Start() {

        // Find the master battle script component. Return an error if it is
        // not found.
        masterBattleScript = GameObject.Find(MASTER_BATTLE_LOOP).
            GetComponent<MasterBattleScript>();

        if(masterBattleScript == null) {

            Debug.LogError("Master Battle Script could not be found!");
        }

        // Find the nav mesh agent and remove updated rotation.
        navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshAgent.updateRotation = true;

        // Create a new mesh path for the passive path line to calcualte the
        // predicted movement of the puppet.
        path = new NavMeshPath();
    }

}
