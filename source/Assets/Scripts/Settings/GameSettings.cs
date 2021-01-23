using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

///<summary>
/// Used to house the game data.
///</summary>
public class GameSettings {

    // All hunters in and out of the party.
    public List<Hunter> hunterCollection {
        get;
        private set;
    }

    // Current Group of hunters in the party
    public List<Hunter> hunterParty {
        get;
        private set;
    }

    /*Base Hunter Stats*/
    public const string BASE_NAME = "Un-named Hunter";

    // public List<Perk> basePerks {
    //     get;
    //     private set;
    // }

    public const int BASE_MELEE_ATTACK_COUNT = 1,
                     BASE_ACTION_POINTS = 5,
                     BASE_ACTION_REFILL = 4,
                     BASE_TOUGHNESS = 5,
                     BASE_STRENGTH = 5,
                     BASE_MOVEMENT = 5,
                     BASE_PRICE = 1,
                     BASE_SANITY = 5,
                     BASE_LEVEL = 0;


    public GameSettings() {

        hunterParty = new List<Hunter>();

        hunterCollection = new List<Hunter>();
    }

    /// <summary>
    /// Gets a hunter in the hunter group based on their ID
    /// </summary>
    /// <param name="id">the id of the hunter to be found</param>
    /// <returns>The person in the group; Returns null if the person could not 
    ///     be found</returns>
    public Hunter findHunter(int id) {
        foreach (Hunter hunter in hunterParty) {
            
            if (id == hunter.hunterID) return hunter;
        }

        return null;
    }

    /// <summary>
    /// Adds a new hunter to the person group and then return them.
    /// </summary>
    /// <param name="hunter">The hunter to be added to the game 
    ///     settings.</param>
    /// <returns>The new hunter in the group.</returns>
    public Hunter addHunter(Hunter hunter) {

        hunterCollection.Add(hunter);

        return hunter;
    }

    public Hunter addHunter(string name, int sanity, int meleeAttackCount, 
        int actionPointCount, int toughness, int strength, int movement) {

        Hunter hunter = new Hunter(this, hunterCollection.Count, name, sanity, 
            meleeAttackCount, null, actionPointCount, BASE_ACTION_REFILL, 
            toughness, strength, movement, BASE_PRICE, BASE_LEVEL);

        return addHunter(hunter);
    }

    public Hunter addHunter(string name) {

        Hunter hunter = new Hunter(this, hunterCollection.Count, name);

        return addHunter(hunter);
    }

    public Hunter addHunter() {

        Hunter hunter = new Hunter(this, hunterCollection.Count);

        return addHunter(hunter);
    }

    public bool removeHunter(Hunter hunter) {

        return hunterCollection.Remove(hunter);
    }

    public void addToParty(Hunter hunter) {

        hunterParty.Add(hunter);
    }

    public bool removeFromParty(Hunter hunter) {
        
        return hunterParty.Remove(hunter);
    }

}
