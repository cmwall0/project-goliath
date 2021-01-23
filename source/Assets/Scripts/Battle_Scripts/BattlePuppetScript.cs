using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

///<summary>
/// Script attached the the hunter puppet object which handles movement and 
/// actions.
///</summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]
public class BattlePuppetScript : MonoBehaviour, ParentPuppetScript {

    private const string MASTER_BATTLE_LOOP = "masterBattleLoop",
                         SELECTED_INDICATOR = "selectedIndicator",
                         DESTINATION_INDICATOR = "destinationObject";
    public bool isMoving;
    public LayerMask terrainLayerMask;

    public Color passivePathLineColor = Color.black;

    public Color activePathLineColor = Color.white;

    public float linePathWidth = 0.05f;

    public float destinationOffsetY = 0.01f;

    public string puppetName;

    public int currentActionPointCount {
        get;
        private set;
    }

    public int maxSanityModifier {
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

    private GameObject player, destinationIndicator, selectedIndicator;
    
    private NavMeshAgent navMeshAgent;

    private Hunter connectedHunter;

    private MasterBattleScript masterBattleScript;

    private LineRenderer pathLineRenderer;

    private NavMeshPath path;

    /// <summary>
    /// Start is called before the first frame and gets all the needed 
    /// components from the puppet, its children and the scene.
    /// </summary>
    void Start() {

        // Find the master battle script component. Return an error if it is
        // not found.
        masterBattleScript = GameObject.Find(MASTER_BATTLE_LOOP).
            GetComponent<MasterBattleScript>();

        if(masterBattleScript == null) {

            Debug.LogError("Master Battle Script could not be found!");
        }

        // Find the destination indicator. Return and error if it is not found,
        //  otherwise, un-child it and disable it.
        destinationIndicator = transform.Find(DESTINATION_INDICATOR).gameObject;

        if(destinationIndicator == null) {

            Debug.LogError("The Destination Object could not be found in " +
                            gameObject.name);

        } else {

            destinationIndicator.transform.parent = null;

            enableDestinationIndicator(false);

        }

        // Find the nav mesh agent and remove updated rotation.
        navMeshAgent = GetComponent<NavMeshAgent>();
        
        navMeshAgent.updateRotation = false;

        // Create a new mesh path for the passive path line to calcualte the
        // predicted movement of the puppet.
        path = new NavMeshPath();

        // Find the line renderer and set its width and position count.
        pathLineRenderer = GetComponent<LineRenderer>();
        pathLineRenderer.startWidth = linePathWidth;
        pathLineRenderer.endWidth = pathLineRenderer.startWidth;
        pathLineRenderer.positionCount = 0;

        // Find the selected indicator. If it cannot be found, throw an error,
        //  otherwise, diable it.
        selectedIndicator = transform.Find(SELECTED_INDICATOR).gameObject;
        if(selectedIndicator == null) {

            Debug.LogError("The selected indicator could not be found in "
                           + gameObject.name);
        } else {

            enableSelectedIndicator(false);
        }

        // Find the main player camera. If it couldnt be found, throw an error.
        player = Camera.main.gameObject;

        if(player == null) {

            Debug.LogError("There is no main camera!");
        }

        // Initialize battle actions
        battleActions = new List<BattleAction>();

        // Add punch as basic action all hunters have (if they have a weapon we
        //   could add a system where it removes the punch but for now it is
        //   on all hunters). !!REMEMBER TO DO ANYTHING RELATED TO THE CONNECTED
        //   HUNTER IN THE SET HUNTER FUNCTION!!
        battleActions.Add(Punch.getAction());
        battleActions.Add(DoublePunch.getAction());

        // Finally, set modifiers to 0
        maxSanityModifier = 0;
        meleeAttackCountModifier = 0;
        actionPointCountModifier = 0;
        actionRefillModifier = 0;
        toughnessModifier = 0;
        strengthModifier = 0;
        movementModifier = 0;
    }

    
    /// <summary>
    /// Currently handles the movement logic for the hunter.
    ///</summary>
    void Update() {

    }

    ///<summary>
    /// Calculat the nav mesh's destination from the navMeshAgent to the
    /// parameter destination using the Navmesh and save the path in
    /// the path object;
    ///</summary>
    /// <param name="destination">The new destination for the agent.</param>
    public void calculateNavDestination(Vector3 destination) {

        NavMesh.CalculatePath(navMeshAgent.transform.position, destination, 
                              NavMesh.AllAreas, path);
    }

    ///<summary>
    /// Make the agent move on the given path.
    ///</summary>
    /// <param name="path">The path which the agent should now take</param>
    private void setPath(NavMeshPath path) {

        navMeshAgent.SetPath(path);
    }

    ///<summary>
    /// Enable/Disable the selected indicator game object.
    ///</summary>
    /// <param name="set">Boolean which tells if the indicator should be on
    /// or off.</param>
    public void enableSelectedIndicator(bool set) {

        selectedIndicator.SetActive(set);
    }

    ///<summary>
    /// Enable/Disable the destination indicator game object.
    ///</summary>
    /// <param name="set">Boolean which tells if the indicator should be on
    /// or off.</param>
    public void enableDestinationIndicator(bool set) {

        destinationIndicator.SetActive(set);
    }
    ///<summary>
    /// Enable/Disable the path line renderer
    ///</summary>
    /// <param name="set">Boolean which tells if the line renderer should be on
    /// or off.</param>
    public void enableLineRenderer(bool set) {

        pathLineRenderer.enabled = set;
    }

    ///<summary>
    /// Set the position of the destination indicator on the navmesh
    ///</summary>
    public void setDestinationIndicator() {

        // Get the ray from the camera to the mouse position
        Ray castPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        // Holds the info of what was hit by the ray.
        RaycastHit hit;

        enableDestinationIndicator(true);

        // If the castPoint ray hit a piece of the terrain, continue 
        //   computation.
        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, 
                            terrainLayerMask)) {

            // Holds the info of what was hit regarding the nav mesh by the ray.
            NavMeshHit navHit;

            // If the hit point of the initial ray hits the navmesh, then set 
            //  the position of the destination indicator to that point.
            if (NavMesh.SamplePosition(hit.point, out navHit, 1, 
                NavMesh.AllAreas)) {

                Vector3 targetVector = navHit.position;

                destinationIndicator.transform.position = 
                    new Vector3(targetVector.x, 
                                destinationOffsetY + targetVector.y, 
                                targetVector.z);
            }
        }
    }

    ///<summary>
    /// Draws the given nav mesh's path in the line renderer
    ///</summary>
    /// <param name="linePath">The path which the line should render</param>
    private void drawLinePath(NavMeshPath linePath) {

        // Set the position count to be the number of corners in the path.
        pathLineRenderer.positionCount = linePath.corners.Length;

        // Set the first position to be this object's position.
        pathLineRenderer.SetPosition(0, transform.position);

        // If the amount o corners is less than or equal to one,
        //  end computation.
        if (linePath.corners.Length <= 1) {
            return;
        }

        // For each corner in the path, set the corresponding line corner's 
        //  position to be equal to it.
        for (int i = 1; i < linePath.corners.Length; i++) {

            Vector3 position = new Vector3(linePath.corners[i].x, 
                                           linePath.corners[i].y, 
                                           linePath.corners[i].z);
            pathLineRenderer.SetPosition(i, position);
        }
    }

    public void processMovement() {

        if(navMeshAgent.hasPath) {

            navMeshAgent.SetPath(path);

            enableLineRenderer(false);

        } else {

            enableLineRenderer(true);

            setDestinationIndicator();

            calculateNavDestination(destinationIndicator.transform.
                position);

            drawLinePath(path);

            if(isMoving) {

                pathLineRenderer.endColor = Color.white;

                pathLineRenderer.startColor = pathLineRenderer.endColor;

                isMoving = false;  
            }
        }

        // If the RMB is held down or the nav mesh has a path, set the
        //  path, redraw the line following the navMeshAgent's path, and
        //  change the line's color.
        if(Input.GetMouseButtonDown(1)) {

            pathLineRenderer.endColor = Color.black;

            pathLineRenderer.startColor = pathLineRenderer.endColor;
            
            isMoving = true;

            navMeshAgent.SetPath(path);

        // If the nav mesh does not have a path, set the destination
        //  indicator, calculate the nav destination using the
        //  destination indicator's position, draw the line, and
        //  chang ethe line's color.
        }
    }

    ///<summary>
    /// Set the connected hunter of this puppet
    ///</summary>
    ///<param name="hunter"> the given being set to this puppet </param>
    public void setHunter(Hunter hunter) {

        connectedHunter = hunter;

        puppetName = connectedHunter.hunterName;

        currentActionPointCount = connectedHunter.actionPointCount;
    }

    public List<BodyPart> GetBodyParts() {
        return connectedHunter.bodyParts;
    }

    /*Hunter Name Functions*/
    public string getHunterName() {
        return connectedHunter.hunterName;
    }

    public string getPuppetName() {
        return puppetName;
    }

    /*Hunter ID Functions*/
    public int getHunterID() {
        return connectedHunter.hunterID;
    }

    /* Hunter alive functions*/
    public bool getAlive() {
        return connectedHunter.alive;
    }

    public void setAlive(bool isAlive) {

        connectedHunter.setAlive(isAlive);
    }

    /*Max Sanity Functions*/
    public int getUnmodifiedMaxSanity() {

        return connectedHunter.maxSanity;
    }

    public int getModifiedMaxSanity() {

        return connectedHunter.maxSanity + maxSanityModifier;
    }

    public void setMaxSanity(int givenMaxSanity) {

        connectedHunter.setMaxSanity(givenMaxSanity);
    }

    public void setMaxSanityModifier(int givenMaxSanityModifier) {

        maxSanityModifier = givenMaxSanityModifier;
    }

    /*Current Sanity Functions*/
    public int getCurrentSanity() {

        return connectedHunter.currentSanity;
    }

    public void setCurrentSanity(int givenCurrentSanity) {

        connectedHunter.setCurrentSanity(givenCurrentSanity);
    } 

    /*Melee Attack Count Functions*/
    public int getUnmodifiedMeleeAttackCount() {

        return connectedHunter.meleeAttackCount;
    }

    public int getModifiedMeleeAttackCount() {

        return connectedHunter.meleeAttackCount + meleeAttackCountModifier;
    }

    public void setMeleeAttackCount(int givenMeleeAttackCount) {

        connectedHunter.setMeleeAttackCount(givenMeleeAttackCount);
    }

    public void setMeleeAttackCountModifier(int givenMeleeAttackCount) {
        
        meleeAttackCountModifier = givenMeleeAttackCount;
    }

    /*Action Point Count Functions*/
    public int getUnmodifiedActionPointCount() {

        return connectedHunter.actionPointCount;
    }

    public int getModifiedActionPointCount() {

        return connectedHunter.actionPointCount + actionPointCountModifier;
    }

    public void setActionPointCount(int givenActionPointCount) {

        connectedHunter.setActionPointCount(givenActionPointCount);
    }

    public void setActionPointCountModifier(int givenActionPointCount) {
        
        actionPointCountModifier = givenActionPointCount;
    }

    /*Action Refill Functions*/
    public int getUnmodifiedActionRefill() {

        return connectedHunter.actionRefill;
    }

    public int getModifiedActionRefill() {

        return connectedHunter.actionRefill + actionRefillModifier;
    }

    public void setActionRefill(int givenActionRefill) {

        connectedHunter.setActionRefill(givenActionRefill);
    }

    public void setActionRefillModifier(int givenActionRefill) {
        
        actionRefillModifier = givenActionRefill;
    }

    /*Toughness Functions*/
    public int getUnmodifiedToughness() {

        return connectedHunter.toughness;
    }

    public int getModifiedToughness() {

        return connectedHunter.toughness + toughnessModifier;
    }

    public void setToughness(int givenToughness) {

        connectedHunter.setToughness(givenToughness);
    }

    public void setToughnessModifier(int givenToughnessModifier) {
        
        toughnessModifier = givenToughnessModifier;
    }

    /*Strength Functions*/
    public int getUnmodifiedStrength() {

        return connectedHunter.strength;
    }

    public int getModifiedStrength() {

        return connectedHunter.strength + strengthModifier;
    }

    public void setStrength(int givenStrength) {

        connectedHunter.setStrength(givenStrength);
    }

    public void setStrengthModifier(int givenStrengthModifier) {
        
        strengthModifier = givenStrengthModifier;
    }

    /*Movement Functions*/
    public int getUnmodifiedMovement() {

        return connectedHunter.movement;
    }

    public int getModifiedMovement() {
        return connectedHunter.movement + movementModifier;
    }

    public void setMovement(int givenMovement) {

        connectedHunter.setMovement(givenMovement);
    }

    public void setMovementModifier(int givenMovementModifier) {
        
        movementModifier = givenMovementModifier;
    }

    public void setCurrentActionPointCount(int givenCount) {
        
        currentActionPointCount = givenCount;
    }

    public int getCurrentActionPointCount() {
        
        return currentActionPointCount;
    }

    /*Price Functions*/
    public int getPrice() {
        return connectedHunter.price;
    }

    /*Level Functions*/
    public int getLevel() {
        return connectedHunter.level;
    }

    public List<BodyPart> getBodyParts() {

        return connectedHunter.bodyParts;
    }
}
