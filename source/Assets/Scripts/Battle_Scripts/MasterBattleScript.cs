/*
 * Filename: Master_Battle_Script.cs
 * Author: Chad
 * Description: Contains the game loop logic for the battle scenarios.
 *
 * Copyright (c) 2020
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterBattleScript : MonoBehaviour {

    // Camera which is used for the battle. 
    private GameObject battleCam;

    public int huntPartySize = 4;

    public GameObject puppetPrefab;

    // Start is called before the first frame update
    void Start() {

        MasterGameScript.instance.onBattleSceneChanged(this);
    }

    public void init(List<Hunter> hunters) {

        for (int i = 0; i < hunters.Count; i++) {

            int x = i * 4 - 6;
            int y = 0;

            GameObject currentHunter = Instantiate(puppetPrefab,
                new Vector3(x, 0, y), Quaternion.identity);
            
            currentHunter.GetComponent<BattlePuppetScript>().
                setHunter(hunters[i]);
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
