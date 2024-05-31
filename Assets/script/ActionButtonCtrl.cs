using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ActionButtonCtrl : MonoBehaviour
{
    public List<GameObject> mButtonObjects = new List<GameObject>();

    private List<ActionButtons> mActionButtons = new List<ActionButtons>();

    private UnityAction<BaseEventData> mCallBack;
    private delegate void MyMehod(BaseEventData pd);
    private PlayerActionControl mPlayerActionControl;

public void Start()
    {
        Initialize();
    }
    public void Initialize()
    {
        mPlayerActionControl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerActionControl>();
        foreach (GameObject iButtons in mButtonObjects)
        {
            HandleAddActionButton(iButtons, iButtons.name);
        }
    }

    private void addTriggersListener(GameObject obj, EventTriggerType eventTriggerType, MyMehod myMehod)
    {
        mCallBack = new UnityAction<BaseEventData>(myMehod);
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventTriggerType;
        entry.callback.AddListener(mCallBack);
        AddButton(obj, entry);
    }
    private GameObject AddButton(GameObject iUIObject, EventTrigger.Entry iEventTrigger)
    {
        EventTrigger aEventTrigger = iUIObject.GetComponent<EventTrigger>();
        if (aEventTrigger == null)
        {
            aEventTrigger = iUIObject.AddComponent<EventTrigger>();
        }
        if (aEventTrigger.triggers.Count == 0)
        {
            aEventTrigger.triggers = new List<EventTrigger.Entry>();
        }
        aEventTrigger.triggers.Add(iEventTrigger);
        return iUIObject;
    }
    private void OnbuttonDown(BaseEventData iOnbuttonDownEvent)
    {
        if (iOnbuttonDownEvent.selectedObject.tag != "ActionButton") { return; }
        foreach (ActionButtons aActionButtons in mActionButtons)
        {
            if (iOnbuttonDownEvent.selectedObject == aActionButtons.GetGameObject())
            {
                Debug.Log("You are pressing " + iOnbuttonDownEvent.selectedObject.name);
                if (iOnbuttonDownEvent.selectedObject.name == "MenuButton") { GameQuit(); return; }
                mPlayerActionControl.HandleStartAction();
            }
        }
    }
    private void OnbuttonUp(BaseEventData iOnButtonUpEvent)
    {
        if (iOnButtonUpEvent.selectedObject.tag != "ActionButton") { return; }
        Debug.Log("You are nolonger pressing " + iOnButtonUpEvent.selectedObject.name);
    }

    private void GameQuit()
    {
        Application.Quit();
    }
    public void HandleAddActionButton(GameObject iButton, string iButtonName)//為每個技能按鈕加入以下資料
    {
        foreach (ActionButtons btn in mActionButtons)
        {
            if (iButton == btn.GetGameObject())
            {
                return;
            }
        }
        mActionButtons.Add(new ActionButtons(iButtonName, iButton));
        addTriggersListener(iButton, EventTriggerType.PointerDown, OnbuttonDown);
        addTriggersListener(iButton, EventTriggerType.PointerUp, OnbuttonUp);
    }

}
class ActionButtons
{

    protected string mButtonName;
    protected GameObject mButtonGameObject;

    public ActionButtons(string iButtonName, GameObject iButtonGameObject)
    {
        mButtonName = iButtonName;
        mButtonGameObject = iButtonGameObject;
    }


    public string GetButtonName() => mButtonName;
    public GameObject GetGameObject() => mButtonGameObject;
    public Button GetButtonComponent() => mButtonGameObject.GetComponent<Button>();
}
