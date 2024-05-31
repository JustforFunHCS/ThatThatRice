using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraDetect : MonoBehaviour
{
    [SerializeField] private FoodSlotManager mFoodSlotManager;
    [SerializeField] private PlayerActionControl mPlayerActionControl;
    [SerializeField] private UITextControler mTextControler;

    private string[] mInteractiveObjectTags = new string[] { FOOD,CUSTOMER ,LUNCH_BOX , GARBAGE_CAN };
    private const string FOOD = "Food", CUSTOMER = "Customer",LUNCH_BOX="Lunch box",GARBAGE_CAN="Garbage Can";

    private void OnEnable()
    {
        GameObject aCanvasObject = GameObject.Find("Canvas");
        mTextControler = aCanvasObject.GetComponent<UITextControler>();
        mPlayerActionControl = GetPlayerActionControl();
    }


    private void FixedUpdate()
    {
        ObjectScan(transform);
    }
    private void ObjectScan(Transform iObjectTransform)
    {
        Transform aObjectTransform = iObjectTransform;
        RaycastHit aHit;
        if (Physics.Raycast(aObjectTransform.position, aObjectTransform.TransformDirection(Vector3.forward), out aHit, 5))
        {
            Debug.DrawRay(aObjectTransform.position, aObjectTransform.TransformDirection(Vector3.forward) * aHit.distance, Color.yellow);
            if (IsInteractiveObject(aHit.collider.gameObject.tag))
            {
                string aTag = aHit.collider.gameObject.tag;
                if (aTag == FOOD)
                {
                    mTextControler.HandleChangeTextDetail(mFoodSlotManager.GetFoodName(aHit.collider.gameObject));
                    mPlayerActionControl.HandleChangeFacingTarget(PlayerActionControl.ObjectType.Food, aHit.collider.gameObject);
                    return;
                }
                else if (aTag == CUSTOMER)
                {
                    mPlayerActionControl.HandleChangeFacingTarget(PlayerActionControl.ObjectType.Customer, aHit.collider.gameObject);
                    return;
                }
                else if (aTag == LUNCH_BOX)
                {
                    mTextControler.HandleChangeTextDetail("¶º²°");
                    mPlayerActionControl.HandleChangeFacingTarget(PlayerActionControl.ObjectType.Lunchbox, aHit.collider.gameObject);
                    return;
                }
                else if (aTag == GARBAGE_CAN)
                {
                    mTextControler.HandleChangeTextDetail("©U§£±í");
                    mPlayerActionControl.HandleChangeFacingTarget(PlayerActionControl.ObjectType.GarbageCan, aHit.collider.gameObject);
                    return;
                }
            }
            mTextControler.HandleHideTextGameObject();
            return;
        }
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 5, Color.white);
        mTextControler.HandleHideTextGameObject();
        // Debug.Log("Did not Hit");
        return;
    }

    private PlayerActionControl GetPlayerActionControl()
    {
        GameObject          aPlayerObject  = GameObject.FindGameObjectWithTag("Player");
        PlayerActionControl aActionControl = aPlayerObject.GetComponent<PlayerActionControl>();
        return aActionControl;

    }

    private bool IsInteractiveObject(string iTagName)
    {
        foreach(string aTagName in mInteractiveObjectTags)
        {
            if (aTagName == iTagName) return true;
        }
        return false;
    }
}
