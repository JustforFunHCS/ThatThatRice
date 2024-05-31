using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionControl : MonoBehaviour
{
    [SerializeField]private Joystick mMovementJoystick;
    [SerializeField]private Joystick mRotateJoystick;
    [SerializeField]private FoodSlotManager mFoodSlotManager;

    private GameObject mCamera;
    private GameObject mPlayerEyes;
    private GameObject mObjectFacingTarget;

    private CharacterController mCharacterController ;
    private LunchBoxsManager mLunchBoxsManager;
    [SerializeField] private float mMovementSpeed = 6;
    [SerializeField] private float mRotateSpeed = 110 ;

    private const string PLAYEREYES = "PlayerEyes";
    public enum ObjectType { Food, Customer , Lunchbox , GarbageCan }
    private ObjectType mCurrentFacingObjectType;

    private float mAxisX, mAxisY;
    [SerializeField]
    private bool mIsGameStart = true;

    private void Start()
    {
        mCamera = Camera.main.gameObject;
        GetPlayerEyes(gameObject.transform);
        mCamera.transform.parent = mPlayerEyes.transform;
        mCamera.transform.localPosition = Vector3.zero;
        mCharacterController = GetComponent<CharacterController>();
        mLunchBoxsManager = GetComponent<LunchBoxsManager>();
        mLunchBoxsManager.HandleDisableLunchBox();
    }
    void Update()
    {
        if (!mIsGameStart) return;
        Vector3 aMoveDirection = GetMoveDirection(mMovementJoystick, gameObject);
        Vector2 aRotateDirection = new Vector2 (mAxisX,mAxisY);
        Movement(aMoveDirection,mMovementSpeed);
        Rotate(mRotateJoystick,aRotateDirection, mRotateSpeed);
        Camerafollow();

    }
    private void Movement(Vector3 iMoveDirection , float iMovementSpeed)
    {                
        mCharacterController.Move(new Vector3( iMoveDirection.x ,0 ,iMoveDirection.z) * Time.deltaTime * iMovementSpeed);        
    }
    private void Camerafollow()
    {
        mCamera.transform.rotation = gameObject.transform.rotation;
    }
    private void Rotate(Joystick iRotateJoystick ,Vector2 iRotateAxis , float iRotateSpeed)
    {
        Vector2 aRotateAxis = iRotateAxis;
        aRotateAxis.x -= iRotateJoystick.Vertical * Time.deltaTime * iRotateSpeed;
        aRotateAxis.x  = Mathf.Clamp(aRotateAxis.x, -90f, 90f);
        aRotateAxis.y += iRotateJoystick.Horizontal * Time.deltaTime * iRotateSpeed;
    
        Quaternion aRotation = Quaternion.Euler(aRotateAxis.x , aRotateAxis.y ,0);
        SetRotationAxis(aRotateAxis);
        transform.rotation = aRotation;
    }
    private void SetRotationAxis(Vector2 iRotateAxis)
    {
        mAxisX = iRotateAxis.x;
        mAxisY = iRotateAxis.y;
    }

    public void HandleChangeFacingTarget(ObjectType itype, GameObject iFacingTarget)
    {
        mObjectFacingTarget = iFacingTarget;
        mCurrentFacingObjectType = itype;
    }
    public void HandleStartAction()
    {
        ComfirmCurrentFacingTarget(mObjectFacingTarget,mCurrentFacingObjectType);
    }
    private void ComfirmCurrentFacingTarget(GameObject iObjectFacingTarget,ObjectType iObjectType)
    {
        ObjectType aCurrentFacingTargetType = iObjectType;
        GameObject aObjectFacingTarget = iObjectFacingTarget;
        if(aCurrentFacingTargetType == ObjectType.Food)
        { mLunchBoxsManager.HandleAddFood(mFoodSlotManager.GetFoodSlot(aObjectFacingTarget)); }
        else if(aCurrentFacingTargetType == ObjectType.Customer)
        {
           aObjectFacingTarget.GetComponent<CustomerControl>().HandleInteract(gameObject);            
        }
        else if(aCurrentFacingTargetType == ObjectType.Lunchbox)
        {
            if (!mLunchBoxsManager.IsLunchBoxEnable()) mLunchBoxsManager.HandleEnableLunchBox();
        }
        else if(aCurrentFacingTargetType == ObjectType.GarbageCan)
        {
            if (mLunchBoxsManager.IsLunchBoxEnable()) mLunchBoxsManager.HandleClearFood();
        }
    }   
    private GameObject GetPlayerEyes(Transform iParent)
    {
        foreach (Transform aChildObject in iParent)
        {
            if(aChildObject.gameObject.tag == PLAYEREYES)
            {
                return mPlayerEyes = aChildObject.gameObject;
            }
        }
        Debug.LogError("missing Eyes!!");
        return null;
    }
    private Vector3 GetMoveDirection(Joystick iMovementJoystick, GameObject iMovementObject)
    {
        Transform aObjectTransform = iMovementObject.transform;
        float aJoystickVertical = iMovementJoystick.Vertical;
        float aJoystickHorizontal = iMovementJoystick.Horizontal;
        Vector3 aMoveDirection = aObjectTransform.forward * aJoystickVertical + aObjectTransform.right * aJoystickHorizontal;
        return aMoveDirection;
    }
}
