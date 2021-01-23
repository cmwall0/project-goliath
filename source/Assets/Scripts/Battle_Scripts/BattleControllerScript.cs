using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Used to control and manipulate the camera and the player's ability to 
///     interact with the field/hunters.
/// </summary>

[RequireComponent(typeof(CharacterController))]
public class BattleControllerScript : MonoBehaviour {

    private const string MASTER_BATTLE_LOOP = "masterBattleLoop",
                         ACTUAL_CAMERA = "actualCamera";



    // The cam's movement velocity
    public float cameraVelocity = 10.0f;

    // How far away from any from the edge of the screen the mouse needs to be  
    //   to trigger movement
    public float edgeDeltaTrigger = 3.0f;

    // The max allowed distance to scroll out
    public float maxScrollDistance = 30.0f;

    // The min allowed distance to scroll in
    public float minScrollDistance = 5.0f;

    // The velocity for camera scrolling
    public float scrollVelocity = 10.0f;

    // The current zoom level calculated by the scroll wheel * scroll velocity
    public float zoomLevel = 15.0f;

    private GameObject cameraObject;

    private CharacterController cameraController;

    private MasterBattleScript masterBattleScript;

    /// <summary>
    /// Called when the script is called and initializes the battle script for 
    /// this script.
    /// </summary>
    void Start() {

        masterBattleScript = GameObject.Find(MASTER_BATTLE_LOOP).
            GetComponent<MasterBattleScript>();

        cameraObject = transform.Find(ACTUAL_CAMERA).gameObject;

        if(cameraObject == null) {

            Debug.LogError("The camera could not be found in "
                           + gameObject.name);
        }

        cameraController = GetComponent<CharacterController>();
    }

    /// <summary>
    /// Updates each frame to determine what the player/camera must calculate 
    /// and execute in that exact frame. Currently calculate shunter selection
    /// </summary>
    public void Update() {


    }

    ///<summary>
    /// Handles the movement of the camera which should not be bound by the
    /// framerate.
    ///<summary>
    public void FixedUpdate() {

        Vector3 movement = Vector3.zero;

        // Right Movement
        if (Input.GetAxis("Horizontal") > 0 
        || (Input.mousePosition.x >= Screen.width - edgeDeltaTrigger && 
            Input.mousePosition.x <= Screen.width)) {

            movement += transform.right * cameraVelocity * Time.fixedDeltaTime;

        // Left Movement
        } else if (Input.GetAxis("Horizontal") < 0 
        || (Input.mousePosition.x <= edgeDeltaTrigger && 
            Input.mousePosition.x >= 0)) {

            movement += -transform.right * cameraVelocity * Time.fixedDeltaTime;
        }

        // Up Movement
        if (Input.GetAxis("Vertical") > 0 
        || (Input.mousePosition.y >= Screen.height - edgeDeltaTrigger &&
            Input.mousePosition.y <= Screen.height)) {

            movement += transform.forward * cameraVelocity * 
                Time.fixedDeltaTime;

        // Down Movement
        } else if (Input.GetAxis("Vertical") < 0
        || (Input.mousePosition.y <= edgeDeltaTrigger && 
            Input.mousePosition.y >= 0)) {

            movement += -transform.forward * cameraVelocity * 
                Time.fixedDeltaTime;
        }

        // The command which moves the camera after all axis are read.
        cameraController.Move(movement);

        // Get the scrollwheel value and sum it to the current zoom level
        zoomLevel += Input.GetAxis("Mouse ScrollWheel") * -scrollVelocity;

        // Clamp the zoom level between the min and max scroll distance
        zoomLevel = Mathf.Clamp(zoomLevel, minScrollDistance, 
                                maxScrollDistance);

        // The command which moves the camera after the scroll value is 
        //   calculated.
        cameraObject.transform.localPosition = new Vector3(
            cameraObject.transform.localPosition.x,
            zoomLevel,
            -zoomLevel);

    }


}
