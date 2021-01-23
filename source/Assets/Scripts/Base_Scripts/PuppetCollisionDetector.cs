using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetCollisionDetector : MonoBehaviour {

    private const string MASTER_GAME_LOOP = "masterGameLoop";

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other) {

        if (other.gameObject.tag == "Hunt Entrance") {

            if(MasterGameScript.instance.gameSettings.hunterParty.Count > 0) {

                FindObjectOfType<TransitionController>().sceneTransition(1);

            } else {
                
                Debug.LogError("Not enough hunters in party!");
            }
        }
    }
}
