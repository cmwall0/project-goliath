using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// The master game script which is called at the start of the game and persists
/// through each phase
///</summary>
public class MasterGameScript : MonoBehaviour {

    private const int BASE_HUNTER_PARTY_SIZE = 4;

    private readonly string[] names = { "Aczins", "Ahie", "Braccons",
                                        "Chesean", "Degi", "Ektaik", "Enga",
                                        "Ex'ax", "Ghungraid", "Hiekoil",
                                        "Hislairs", "Kharu", "Khixens",
                                        "Miesors", "Nuq'ol", "Ohros", "Qalqod",
                                        "Qel", "Rolgeh", "Rolzit", "Ruve",
                                        "Sinzear", "Sit'aex", "Strosqels",
                                        "Thrulxeix", "Trarux", "Tukuks",
                                        "Vegzuks", "Vihmo", "Yuux'eth" };


    // define settings. Settings can only be set privately (within this class) 
    //  but can be gotten from anywhere.
    public UserSettings userSettings { private set; get; }
    public GameSettings gameSettings { private set; get; }

    public static MasterGameScript instance {
        private set; 
        get;
    }

    /// <summary>
    /// Called when the script is called and initializes the settings and loads
    ///     the first scene
    /// </summary>
    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
            return;
        }

        // Tells Unity to not destroy the gameObject on a new scene load.
        DontDestroyOnLoad(gameObject);

        /* A new series of user settings */
        userSettings = new UserSettings();

        /* A new series of game settings */
        gameSettings = new GameSettings();
    }

    /// <summary>
    /// TODO
    /// </summary>
    public void onBattleSceneChanged(MasterBattleScript battleScript) {

        battleScript.init(gameSettings.hunterParty);
    }

    public void onHuntSceneChanged(MasterHuntScript huntScript) {
        int hunterCount = gameSettings.hunterParty.Count;

        Hunter[] hunter = new Hunter[hunterCount];
    }

    /// <summary>
    /// Create a hunter to the game and return that hunter.
    /// </summary>
    /// <param name="name">Create a hunter with the given name.</param>
    /// <returns>The hunter just created</returns>
    public Hunter createHunter(string name) {

        return gameSettings.addHunter(name);
    }

    /// <summary>
    /// Create a hunter to the game and return that hunter.
    /// </summary>
    /// <returns>The hunter just created</returns>
    public Hunter createHunter() {

        return createHunter(names[Random.Range(0, names.Length - 1)]);
    }

    public Hunter createHunter(string name, int sanity, int meleeAttackCount, 
        int actionPointCount, int toughness, int strength, int movement) {

        string newName;

        if(name == "") {

            newName = names[Random.Range(0, names.Length - 1)];

        } else {

            newName = name;
        }

        return gameSettings.addHunter(newName, sanity, meleeAttackCount,
            actionPointCount, toughness, strength, movement);
    }

    public void moveToParty(Hunter hunter) {

        gameSettings.addToParty(hunter);
    }

    public void removeFromParty(Hunter hunter) {
        
        gameSettings.removeFromParty(hunter);
    }
}
