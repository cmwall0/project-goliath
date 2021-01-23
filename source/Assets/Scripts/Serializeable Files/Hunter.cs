/*
 * Filename: Hunter_Script.cs
 * Author: Chad
 * Description: Used to manage the hunter and extract info for other scripts 
 *              and game objects to use. The actor is any object related to
 *              the monster or the hunter. NOTE: this script is attached to
 *              each individual hunter and is not a script for the player
 *              object itself.
 *
 * Copyright (c) 2020
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Hunter {

    public List<HunterPerk> perkList {
        get;
        private set;
    }

    public List<BodyPart> bodyParts {
        get;
        private set;
    }

    public List<Equipment> inventory {
        get;
        private set;
    }

    public string hunterName {
        get;
        private set;
    }

    public int hunterID {
        get;
        private set;
    }

    public bool alive {
        get;
        private set;
    }

    public int level {
        get;
        private set;
    }

    // Sanity is like an extra bodypart which can only be damaged by specific 
    //   instances (IE seeing something horrific) or using dark arts to cast a 
    //   spell.
    public int maxSanity {
        get;
        private set;
    }

    public int currentSanity {
        get;
        private set;
    }

    // The amount of attacks the hunter would do in melee
    public int meleeAttackCount {
        get;
        private set;
    }

    // Pool of points which can be used to do actions
    public int actionPointCount {
        get;
        private set;
    }

    // The amount of points which refresh at the start of the hunter phase
    public int actionRefill {
        get;
        private set;
    }

    // When a hit is landed, toughness acts as a deterrent against that roll. 
    //   Think of it as a hit landing but ricocheting off the target.  
    //   Toughness will be further explained in Strength
    public int toughness {
        get;
        private set;
    }

    // Strength of a hunter determines how well a base melee attack will 
    //   penetrate/go through toughness. The current calculation to determine 
    //   penetration would be Hit% = Strength/(Toughness * 2).
    public int strength {
        get;
        private set;
    }

    // The distance the hunter can move in one movement action
    public int movement {
        get;
        private set;
    }

    // Price to bring the hunter into the hunter party
    public int price {
        get;
        private set;
    }

    // Ctor for a nameless hunter
    public Hunter(GameSettings gs, int givenID) 
        : this(gs, givenID, GameSettings.BASE_NAME) {}

    // Ctor for a named hunter
    public Hunter(GameSettings gs, int givenID, string givenName) 
        : this (gs, givenID, givenName, GameSettings.BASE_SANITY, 
                /*gs.BASE_PERKS, */ 
                GameSettings.BASE_MELEE_ATTACK_COUNT, null,
                GameSettings.BASE_ACTION_POINTS, 
                GameSettings.BASE_ACTION_REFILL, GameSettings.BASE_TOUGHNESS,
                GameSettings.BASE_STRENGTH, GameSettings.BASE_MOVEMENT,
                GameSettings.BASE_PRICE, GameSettings.BASE_LEVEL) {}

    // Ctor for full control on what is set for the hunter. This should be made
    // for when a special hunter is found. That or a hunter is created from
    // scratch.
    public Hunter(GameSettings gs, int givenID, string givenName, 
                  int givenSanity, 
                  /*List<Perk> givenPerks,*/ int givenMeleeAttackCount, 
                  List<BodyPart> givenBodyParts, int givenActionPoints, 
                  int givenActionRefill, int givenToughness, int givenStrength,
                  int givenMovement, int givenPrice, int givenLevel) {

        hunterName = givenName;

        hunterID = givenID;

        maxSanity = givenSanity;

        currentSanity = maxSanity;

        // perkList = givenPerks

        meleeAttackCount = givenMeleeAttackCount;

        if(givenBodyParts == null) {

            bodyParts = new List<BodyPart>();

            bodyParts.Add(new BodyPart("Torso"));

            bodyParts.Add(new BodyPart("Head"));

            bodyParts.Add(new BodyPart("RArm"));

            bodyParts.Add(new BodyPart("LArm"));

            bodyParts.Add(new BodyPart("RLeg"));

            bodyParts.Add(new BodyPart("LLeg"));

        } else {

            bodyParts = givenBodyParts;
        }

        actionPointCount = givenActionPoints;

        actionRefill = givenActionRefill;

        toughness = givenToughness;

        strength = givenStrength;

        movement = givenMovement;

        price = givenPrice;

        level = givenLevel;

        alive = true;
    }

    public void setHunterName(string givenName) {

        hunterName = givenName;
    }

    public void setAlive(bool isAlive) {
        
        alive = isAlive;
    }

    public void setMaxSanity(int givenMaxSanity) {

        maxSanity = givenMaxSanity;
    }

    public void setCurrentSanity(int givenCurrentSanity) {

        currentSanity = givenCurrentSanity;
    }

    public void setMeleeAttackCount(int givenMeleeAttackCount) {

        meleeAttackCount = givenMeleeAttackCount;
    }

    public void setActionPointCount(int givenActionPointCount) {

        actionPointCount = givenActionPointCount;
    }

    public void setActionRefill(int givenActionRefill) {

        actionRefill = givenActionRefill;
        
    }

    public void setToughness(int givenToughness) {

        toughness = givenToughness;
        
    }

    public void setStrength(int givenStrength) {
        
        strength = givenStrength;
    }

    public void setMovement(int givenMovement) {
        
        movement = givenMovement;
    }
}

// Bodypart class
public class BodyPart {

    private const int BASE_HP = 3;
    private const int BASE_ARMOR = 0;

    public string name {
        get;
        private set;
    }

    // Each limb can take a certain amount of wounds. Usually goes from 
    //   3 -> 2 -> 1 -> Critical/Severed. Can add more HP points to limbs 
    //   (i.e. Healthy 2-> Healthy 1 -> ...). Make it such that when a limb 
    //   enters critical (and when it is damaged in critical state), a random 
    //   status effect from a list based on the limb is chosen to be applied to 
    //   the hunter ranging from bleeding to death by blood loss.
    public int health {
        get;
        private set;
    }

    // If an attack hits AND penetrates, armor may save your limb from being 
    //   damaged. Each armor piece on a limb can add a %Chance to reflect
    public int armor {
        get;
        private set;
    }

    // Have an arrary of objects which each prepresent a different debuff which
    // occurs @ critical dmg.

    public BodyPart(string baseName, int baseHealth, int baseArmor) {

        health = baseHealth;

        armor = baseArmor;

        name = baseName;
    }

    public BodyPart(string _name) : this(_name, BASE_HP, BASE_ARMOR) {
    }
}
