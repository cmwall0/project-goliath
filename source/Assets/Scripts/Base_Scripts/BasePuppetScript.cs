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
public class BasePuppetScript : MonoBehaviour {

    private const string MASTER_BATTLE_LOOP = "masterBaseLoop",
                         SELECTED_INDICATOR = "selectedIndicator",
                         DESTINATION_INDICATOR = "destinationObject";
    public LayerMask terrainLayerMask;

    public Color passivePathLineColor = Color.black;

    public Color activePathLineColor = Color.white;

    public float linePathWidth = 0.05f;

    public float destinationOffsetY = 0.01f;

    private GameObject player, destinationIndicator;
    private NavMeshAgent navMeshAgent;
    private Hunter connectedHunter;

    private MasterBaseScript masterBattleScript;

    private LineRenderer pathLineRenderer;

    private NavMeshPath path;

    /// <summary>
    /// Start is called before the first frame and gets all the needed 
    /// components from the puppet, its children and the scene.
    /// </summary>
    void Start() {

        // Find the master battle script component. Return an error if it is
        // not found.
        masterBattleScript = 
          GameObject.Find(MASTER_BATTLE_LOOP).
            GetComponent<MasterBaseScript>();
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

        // Find the main player camera. If it couldnt be found, throw an error.
        player = Camera.main.gameObject;
        if(player == null) {

            Debug.LogError("There is no main camera!");
        }
    }

    
    /// <summary>
    /// CUrrently handles the movement logic for the hunter.
    ///</summary>
    void Update() {

        // If the RMB is held down or the nav mesh has a path, set the
        //  path, redraw the line following the navMeshAgent's path, and
        //  change the line's color.
        if(Input.GetMouseButtonDown(1) || navMeshAgent.hasPath) {

            navMeshAgent.SetPath(path);
            drawLinePath(navMeshAgent.path);
            pathLineRenderer.endColor = Color.black;
            pathLineRenderer.startColor = pathLineRenderer.endColor;

        // If the nav mesh does not have a path, set the destination
        //  indicator, calculate the nav destination using the
        //  destination indicator's position, draw the line, and
        //  chang ethe line's color.
        } else {
            setDestinationIndicator();
            calculateNavDestination(destinationIndicator.
                transform.position);
            drawLinePath(path);
            pathLineRenderer.endColor = Color.white;
            pathLineRenderer.startColor = pathLineRenderer.endColor;
        }
        
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

}
