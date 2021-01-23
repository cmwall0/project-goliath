using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///<summary>
///
///</summary>
public class BattleUIScript : MonoBehaviour {

    private const string MASTER_OBJECT = "masterGameLoop",
                         BATTLE_COMBAT_SCRIPT = "battleCombatScript",
                         PLAYER_CONTROLLER = "playerController", 
                         UI_CANVAS = "uiCanvas",
                         ACTION_PANEL = "actionPanel",
                         PLAYER_PANEL = "playerPanel",
                         MONSTER_BODY_PART_PANEL = "monsterBodyPartPanel",
                         MBP_PANEL_SCROLL_VIEW = "scrollView",

                         MBP_PANEL_SCROLL_CONTENT = "scrollContent",
                         SANITY_BAR = "sanityBar",
                         SANITY_TEXT = "text",
                         AP_TEXT = "text",
                         AP_BAR = "apBar",
                         NAME_TEXT = "name",
                         FADE = "fade";

    public LayerMask puppetRaycastLayers;

    public LayerMask targetableLayers;

    // The hunter being selected by the player.
    private BattlePuppetScript selectedPuppet;

    private BattleMonsterScript selectedMonster;

    private const int ACTION_BUTTON_COUNT = 20;

    // The current target info index value of the current processing action.
    private int currentTargetIndex = 0;

    // The cumulative count of all targets taken aside from those taken for the
    //   current target info.
    private int previousTargetsCount = 0;

    // The index of the current action being processed
    private int actionTargetIndex = 0;

    private Transform actionPanel, playerPanel, sanityBar, apBar,
    monsterBodyPartPanel, mbpScrollView;

    // Original Action: The initial action which was called.
    // Processing Action: Used for follow up functionality. 
    //                    The current action being processed.
    private BattleAction originalAction, processingAction;

    private BattleCombatScript battleCombatScript;

    private List<List<System.Object>> targets;

    private Button[] actionButtons;

    public GameObject buttonTemplate;

    ///<summary>
    /// When the UI script is intantiated, find all the transform game objects.
    ///</summary>
    void Start() {

        // Find the battle combat script in scene
        battleCombatScript = GameObject.Find(BATTLE_COMBAT_SCRIPT).
            GetComponent<BattleCombatScript>();

        // Find the action panel in the UI canvas
        actionPanel = GameObject.Find(UI_CANVAS).transform.Find(ACTION_PANEL);

        // Find the player panel in the UI canvas
        playerPanel = GameObject.Find(UI_CANVAS).transform.Find(PLAYER_PANEL);

        // Find the monster body part panel in the UI canvas
        monsterBodyPartPanel = GameObject.Find(UI_CANVAS).transform.
            Find(MONSTER_BODY_PART_PANEL);

        // FInd the scroll view in the monster body part
        mbpScrollView = monsterBodyPartPanel.Find(MBP_PANEL_SCROLL_VIEW);

        // Deactivate the monster body part
        monsterBodyPartPanel.gameObject.SetActive(false);

        // Find the sanity bar in the UI canvas
        sanityBar = GameObject.Find(SANITY_BAR).transform;

        // Find the ap bar in the UI canvas
        apBar = GameObject.Find(AP_BAR).transform;

        // Find the fade in the UI canvas
        GameObject.Find(UI_CANVAS).transform.Find(FADE).gameObject.
            SetActive(false);

        // Instantiate a new list for each list of targets for each action
        targets = new List<List<System.Object>>();

        actionButtons = getActionButtons();

        resetUI();
    }

    /// <summary>
    /// Updates each frame to determine what the player/camera must calculate 
    /// and execute in that exact frame. Currently calculate shunter selection
    /// </summary>
    void Update() {

        // If the processing action is not null...
        if(processingAction != null) {

            // If all the targets have been set for the current action list...
            if(processingAction.totalTargetCount == targets[actionTargetIndex].Count) {

                if(!(processingAction is FollowupAction)) {

                    battleCombatScript.processAction(originalAction, targets, 
                        (ParentPuppetScript)selectedPuppet);

                    resetProcessingAction();

                } else {

                    targets.Add(new List<System.Object>());

                    actionTargetIndex++;

                    previousTargetsCount = 0;

                    currentTargetIndex = 0;

                    processingAction = ((FollowupAction)processingAction).getFollowup();
                }

            // If the current targetInfo being processed has reached
            //   the needed amount of targets, then increment the 
            //   currentTargetInfoIndex to move to the next
            //   targetInfo being processed and update the previous
            //   target count to add the now previous targetInfo
            //   target count.
            } else if(processingAction.targetInfos[currentTargetIndex].
                targetCount == targets[actionTargetIndex].Count - 
                previousTargetsCount) {

                currentTargetIndex++;

                previousTargetsCount = targets[actionTargetIndex].Count;

                selectedMonster = null;

                monsterBodyPartPanel.gameObject.SetActive(false);
            }

        } else if (selectedPuppet != null) {

            updateUI(selectedPuppet.battleActions, 
                selectedPuppet.getPuppetName(), 
                selectedPuppet.currentActionPointCount,
                selectedPuppet.getModifiedActionPointCount(),
                selectedPuppet.getCurrentSanity(),
                selectedPuppet.getModifiedMaxSanity()
            );

            selectedPuppet.processMovement();
        }

        if(selectedMonster != null
            && monsterBodyPartPanel.gameObject.activeSelf == false) {

            List<GameObject> children = new List<GameObject>();

            foreach (Transform child in 
                mbpScrollView.Find(MBP_PANEL_SCROLL_CONTENT)) {

                children.Add(child.gameObject);
            }

            children.ForEach(child => Destroy(child));

            addBodyPartsToScrollView(selectedMonster.connectedMonster);

            monsterBodyPartPanel.gameObject.SetActive(true);
                
        } else if(selectedMonster == null) {

            monsterBodyPartPanel.gameObject.SetActive(false);
        }

        processMoueInput();
    }

    /// <summary>
    /// Ran every frame; used to process the mouse's input on the battlefield.
    /// </summary>
    public void processMoueInput() {

        if((selectedPuppet != null && selectedPuppet.isMoving) 
            || MouseInputUIBlocker.BlockedByUI) return;

        if(Input.GetMouseButtonDown(0)) {

            if(processingAction != null) {

                /* The ray where the the mouse is positioned */
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                /* Holds the data for what got hit by the ray IF something got 
                    hit */
                RaycastHit hit;

                // If something was hit
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 
                                    targetableLayers)) {

                    System.Object objectTarget = getTargetableObject(hit.
                        transform.parent, processingAction, currentTargetIndex);

                    if(objectTarget != null){

                        Debug.LogError("Valid Hit!");

                        if(objectTarget is BattleMonsterScript 
                            && processingAction.targetInfos[currentTargetIndex].
                            containsMonsterBodyPart) {

                            selectedMonster = (BattleMonsterScript)objectTarget;

                        } else {
                            
                            targets[actionTargetIndex].Add(objectTarget);
                        }
                    } else {
                        Debug.LogError("Invalid Hit!");
                    }
                }

            } else if (!MouseInputUIBlocker.BlockedByUI) {

                /* The ray where the the mouse is positioned */
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                /* Holds the data for what got hit by the ray IF something got 
                hit */
                RaycastHit hit;

                // If something was hit
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 
                                    puppetRaycastLayers)) {
                    
                    setSelectedPuppet(hit.transform.parent);

                } else {

                    setSelectedPuppet(null);
                } 
            }

        } else if (Input.GetMouseButtonDown(1)) {

            if(processingAction != null) {
                
                resetProcessingAction();
            }
        }
    }

    public void setUI(List<BattleAction> actions, string selectedName, 
        int currentAP, int maxAP, int currentSanity, int maxSanity) {

        setUIActionList(actions);

        setUIPlayerPanel(selectedName);

        setUIAPBar(currentAP, maxAP);

        setUISanityBar(currentSanity, maxSanity);
    }

    public void updateUI(List<BattleAction> actions, string selectedName, 
        int currentAP, int maxAP, int currentSanity, int maxSanity) {

        updateUIActionList(actions);

        setUIPlayerPanel(selectedName);

        setUIAPBar(currentAP, maxAP);

        setUISanityBar(currentSanity, maxSanity);
    }

    public void resetUI() {

        resetUIActionList();

        resetUIPlayerPanel();

        resetUIAPBar();

        resetUISanityBar();
    }

    private void setUIActionList(List<BattleAction> actions) {

        Debug.LogError(actions[0].name);
        Debug.LogError(actions[1].name);
actionButtons[0].interactable = true;
actionButtons[1].interactable = true;
actionButtons[0].onClick.AddListener(() => setProcessingAction(actions[0]));
actionButtons[1].onClick.AddListener(() => setProcessingAction(actions[1]));

        // for (int i = 0; i < actions.Count; i++) {

        //     actionButtons[i].interactable = true;

        //     actionButtons[i].onClick.AddListener(() => setProcessingAction(actions[0]));
        // }
    }

    private bool setProcessingAction(BattleAction givenAction) {

        if(givenAction.apCost > selectedPuppet.currentActionPointCount) {

            return false;
        }

        processingAction = givenAction;

        originalAction = givenAction;

        targets.Add(new List<System.Object>());

        return true;
    }

    private bool resetProcessingAction() {

        if(processingAction == null) {

            return false;
        }

        processingAction = null;

        originalAction = null;

        selectedMonster = null;

        previousTargetsCount = 0;

        currentTargetIndex = 0;

        actionTargetIndex = 0;

        foreach(List<System.Object> list in targets) {
            list.Clear();
        }

        targets.Clear();

        return true;
    }

    private void resetUIActionList() {

        foreach (Button child in actionButtons) {

            child.interactable = false;
        }
    }

    private void updateUIActionList(List<BattleAction> actions) {

        for (int i = 0; i < actions.Count; i++) {

            // actionButtons[i].interactable = actions[i].
            //     isUseable();
        }
    }

    private void setUIPlayerPanel(string name) {

        playerPanel.Find(NAME_TEXT).GetComponent<Text>().text = name;
    }

    private void resetUIPlayerPanel() {

        playerPanel.Find(NAME_TEXT).GetComponent<Text>().text = "";
    }

    private void setUISanityBar(int currentSanity, int maxSanity) {

        sanityBar.gameObject.SetActive(true);

        sanityBar.Find(SANITY_TEXT).GetComponent<Text>().text = 
            barTextSetter(currentSanity, maxSanity);

        sanityBar.GetComponent<Slider>().maxValue = maxSanity;

        sanityBar.GetComponent<Slider>().value = currentSanity;
    }

    private void resetUISanityBar() {

        sanityBar.gameObject.SetActive(false);
    }

    private void setUIAPBar(int currentAP, int maxAP) {

        apBar.gameObject.SetActive(true);

        apBar.Find(AP_TEXT).GetComponent<Text>().text = 
            barTextSetter(currentAP, maxAP);

        apBar.GetComponent<Slider>().maxValue = maxAP;

        apBar.GetComponent<Slider>().value = currentAP;
    }

    private void resetUIAPBar() {

        apBar.gameObject.SetActive(false);
    }

    private Button[] getActionButtons() {

        Button[] buttons = new Button[20];

        for (int i = 0; i < ACTION_BUTTON_COUNT; i++) {
            
            Button currentButton =  actionPanel.Find("action"+i).
                GetComponent<Button>();

            buttons[i] = currentButton;
        }

        return buttons;
    }

    private string barTextSetter(int currentValue, int maxValue) {
        return (currentValue + "/" + maxValue);
    }

    /// <summary>
    /// Simple method to set the currently selected puppet to parameter puppet. 
    ///     Sets the puppet this controller controls.
    /// </summary>
    /// <param name="puppet">The puppet this controller controls.</param>
    public void setSelectedPuppet (Transform puppet) {

        // If there is a selectedHunter, deselect it
        if (selectedPuppet != null) {

            selectedPuppet.enableSelectedIndicator(false);

            selectedPuppet.enableDestinationIndicator(false);

            selectedPuppet.enableLineRenderer(false);
        }

        // If puppet is null, set the puppet to null and end this function
        if (puppet == null) {

            selectedPuppet = null;

            resetUI();
            
        } else {

            // Find the BattlePuppetScript.
            BattlePuppetScript caughtPuppet = puppet.
                GetComponentInChildren<BattlePuppetScript>();

            // If there is no BattlePuppetScript, then we have selected a
            //   monster, so skip this. Otherwise, if it is not null,
            //   set it as the selected puppet.
            if(caughtPuppet != null){
                selectedPuppet = caughtPuppet;

                selectedPuppet.enableSelectedIndicator(true);

                setUI(selectedPuppet.battleActions, 
                    selectedPuppet.getPuppetName(), 
                    selectedPuppet.currentActionPointCount,
                    selectedPuppet.getModifiedActionPointCount(),
                    selectedPuppet.getCurrentSanity(),
                    selectedPuppet.getModifiedMaxSanity()
                );
            }
        }
    }

    /// <summary>
    /// Get the potential target and filters out what is and isnt a valid target
    /// based on the action given. Returns the target as a System.Object if it
    /// is a valid target. Returns null otherwise.
    /// </summary>  
    /// <param name="potentialTarget"></param>
    /// <param name="action"></param>
    /// <param name="currentTargetInfoIndex">The current index of </param>
    /// <returns></returns>
    public System.Object getTargetableObject(Transform potentialTarget, 
        BattleAction action, int currentTargetInfoIndex) {

        // Distance check
        if(action.range != -1) {

            float dist = Vector3.Distance(selectedPuppet.transform.position,
                potentialTarget.position);

            if(dist > action.range) return null;

        // Check for self casting
        } else if (action.range == 0) {

            if(action.targetInfos[0].targetTypes[0] == ActionTarget.SELF) {

                return (System.Object)selectedPuppet;

            } else {

                Debug.LogError
                    ("ERROR! Range == 0 but not self cast for action: " 
                    + action);

                return null;
            }
        }

        

        foreach(ActionTarget possibleTargetType in action.targetInfos
            [currentTargetInfoIndex].targetTypes){

            switch(possibleTargetType) {

                case ActionTarget.MONSTER:

                case ActionTarget.MONSTER_PART:

                    BattleMonsterScript monster = potentialTarget.
                        GetComponentInChildren<BattleMonsterScript>();

                    if(monster != null) {

                        return (System.Object)monster;

                    } else {
                        
                        return null;
                    }

                default:
                    Debug.LogError("ERROR! UNKNWON TARGET INFO: " +
                        possibleTargetType);

                    return null;
            }
        }
        
        return null;
    }

    public void bodyPartChosen(MonsterBodyPart givenMonsterBodyPart) {

        Debug.LogError("The bodypart" + givenMonsterBodyPart.name + 
            "was chosen");

        targets[actionTargetIndex].Add((System.Object)givenMonsterBodyPart);
    }

    /// <summary>
    /// Adds the bodyparts of the given monsterto the scroll view as a set of
    /// buttons.
    /// </summary>
    /// <param name="givenMonster">The given monster</param>
    public void addBodyPartsToScrollView(Monster givenMonster) {
        
        foreach(MonsterBodyPart bodypart in givenMonster.bodyParts) {

            GameObject gameObject = Instantiate(buttonTemplate) as GameObject;

            BattleUIMBPButtonScript buttonScript = 
                gameObject.GetComponent<BattleUIMBPButtonScript>();

            buttonScript.attachUIScript(this);

            buttonScript.setBodyPart(bodypart);

            gameObject.SetActive(true);

            gameObject.GetComponent<Button>().interactable = bodypart.hp > 0;

            gameObject.transform.SetParent(mbpScrollView.Find(MBP_PANEL_SCROLL_CONTENT).   
            transform);
        }
    }
}
