using UnityEngine;
using UnityEngine.UI;
using System.Collections;
///<summary>
///
///</summary>
public class BattleUIMBPButtonScript : MonoBehaviour {

    private MonsterBodyPart attachedMonsterBodyPart;

    public Text BodypartNameText;
    private BattleUIScript uiScript;

    public void attachUIScript(BattleUIScript givenScript) {

        uiScript = givenScript;
    } 

    public void setBodyPart(MonsterBodyPart givenMBP) {

        attachedMonsterBodyPart = givenMBP;
        
        BodypartNameText.text = givenMBP.name;
    }

    public void Button_Click() {

        if(uiScript == null) {
            
            Debug.LogError("UI Script not attached!");

            return;
        }

        uiScript.bodyPartChosen(attachedMonsterBodyPart);
    }
}
