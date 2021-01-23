using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///<summary>
/// Used to manipulate the UI of the base
///</summary>
public class BaseUIScript : MonoBehaviour {

    private const string MASTER_OBJECT = "masterGameLoop",
                         UI_CANVAS = "uiCanvas",
                         CREATE_HUNTER_PANEL = "createHunterPanel",
                         NAME_FIELD = "nameField",
                         SANITY_FIELD = "sanityField",
                         MELEE_FIELD = "meleeField",
                         ACTION_POINT_FIELD = "actionPointField",
                         TOUGHNESS_FIELD = "toughnessField",
                         STRENGTH_FIELD = "strengthField",
                         MOVEMENT_FIELD = "movementField";
    private int givenSanity,
                givenMeleeAttackCount,
                givenActionPointCount,
                givenToughness,
                givenStrength,
                givenMovement;

    private string givenName;

    private MasterGameScript masterGameLoop;

    private InputField nameField,
                       sanityField,
                       meleeAttackCountField,
                       actionPointCountField,
                       toughnessField,
                       strengthField,
                       movementField;

    

    public Transform canvas, createHunterPanel;

    ///<summary>
    /// 
    ///</summary>
    void Start() {

        masterGameLoop = GameObject.Find(MASTER_OBJECT).
            GetComponent<MasterGameScript>();

        canvas = GameObject.Find(UI_CANVAS).transform;

        createHunterPanel = canvas.Find(CREATE_HUNTER_PANEL);

        nameField = createHunterPanel.Find(NAME_FIELD).
            GetComponent<InputField>();

        nameField.onEndEdit.AddListener(delegate {setName(nameField); });

        sanityField = createHunterPanel.Find(SANITY_FIELD).
            GetComponent<InputField>();

        sanityField.onEndEdit.AddListener(delegate {setSanity(sanityField); });

        meleeAttackCountField = createHunterPanel.Find(MELEE_FIELD).
            GetComponent<InputField>();

        meleeAttackCountField.onEndEdit.AddListener(
            delegate {setMeleeAttackCount(meleeAttackCountField); });

        actionPointCountField = createHunterPanel.Find(ACTION_POINT_FIELD).
            GetComponent<InputField>();

        actionPointCountField.onEndEdit.AddListener(
            delegate {setActionPointCount(actionPointCountField); });

        toughnessField = createHunterPanel.Find(TOUGHNESS_FIELD).
            GetComponent<InputField>();

        toughnessField.onEndEdit.AddListener(
            delegate {setToughness(toughnessField); });

        strengthField = createHunterPanel.Find(STRENGTH_FIELD).
            GetComponent<InputField>();

        strengthField.onEndEdit.AddListener(
            delegate {setStrength(strengthField); });

        movementField = createHunterPanel.Find(MOVEMENT_FIELD).
            GetComponent<InputField>();

        movementField.onEndEdit.AddListener(
            delegate {setMovement(movementField); });

        givenName = "";

        givenSanity = GameSettings.BASE_SANITY;

        givenMeleeAttackCount = GameSettings.BASE_MELEE_ATTACK_COUNT;

        givenActionPointCount = GameSettings.BASE_ACTION_POINTS;

        givenToughness = GameSettings.BASE_TOUGHNESS;

        givenStrength = GameSettings.BASE_STRENGTH;

        givenMovement = GameSettings.BASE_MOVEMENT;
    }

    public void onCreateCharacter() {

        Hunter currentHunter = masterGameLoop.createHunter(givenName, 
            givenSanity, givenMeleeAttackCount, givenActionPointCount, 
            givenToughness, givenStrength, givenMovement);

        masterGameLoop.moveToParty(currentHunter);
    }

    public void setName(InputField nameInput) {

        if(nameInput.text.Length > 0) {

            givenName = nameInput.text;
        }
    }

    public void setSanity(InputField sanityInput) {

        if(sanityInput.text.Length > 0) {
            
            Int32.TryParse(sanityInput.text, out givenSanity);
        }
    }

    public void setMeleeAttackCount(InputField meleeAttackCountInput) {

        if(meleeAttackCountInput.text.Length > 0) {
            
            Int32.TryParse(meleeAttackCountInput.text, 
                out givenMeleeAttackCount);
        }
    }

    public void setActionPointCount(InputField actionPointCountInput) {

        if(actionPointCountInput.text.Length > 0) {
            
            Int32.TryParse(actionPointCountInput.text, 
                out givenActionPointCount);
        }
    }

    public void setToughness(InputField toughnessInput) {

        if(toughnessInput.text.Length > 0) {
            
            Int32.TryParse(toughnessInput.text, out givenToughness);
        }
    }

    public void setStrength(InputField strengthInput) {

        if(strengthInput.text.Length > 0) {
            
            Int32.TryParse(strengthInput.text, out givenStrength);
        }
    }

    public void setMovement(InputField movementInput) {

        if(movementInput.text.Length > 0) {
            
            Int32.TryParse(movementInput.text, out givenMovement);
        }
    }

}
