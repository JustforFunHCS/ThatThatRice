using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CustomerControl : MonoBehaviour
{
    [SerializeField]private float mStopDistance = 0;
    [SerializeField]private float mMovementSpeed = 3;
    [SerializeField]private TMP_FontAsset mTextFont;

    private bool mIsCreatedFoodList;
    private int mFoodListLength = 3;


    public enum ActorState { Walking, Order, Waiting , PickedUped }
    [SerializeField]
    private ActorState mCurrentState = ActorState.Walking;

    private Transform mStopPoint , mStartPoint , mLookPoint;
    private ChatBox mChatBox = new ChatBox();
    [SerializeField]
    private List<FoodSlot> mFoodList= new List<FoodSlot>();
    private void OnEnable()
    {
        mCurrentState= ActorState.Order;
        GetNavMeshAgent().stoppingDistance = mStopDistance;
        GetNavMeshAgent().speed = mMovementSpeed;
        GetNavMeshAgent().updatePosition = true;
        GetAnimator().applyRootMotion = false;
    }
    private void Update()
    {
        GetNavMeshAgent().destination = mStopPoint.position;
        AnimateMove(true);

        if (IsArrived())
        {
            if(!mIsCreatedFoodList) CreateFoodList();
            if (mCurrentState == ActorState.Walking) { mCurrentState = ActorState.Order; }
                LookAtTarget();
                GetNavMeshAgent().updatePosition = false;
                AnimateMove(false);
            
        }       
        if(mCurrentState == ActorState.PickedUped)
        {
            GetNavMeshAgent().updatePosition = true;
            GetNavMeshAgent().destination = mStartPoint.position;
            AnimateMove(true);
            return;
        }
    }

    private void LookAtTarget()
    {
        var lookPos = mLookPoint.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 2f);
    }
    private void AnimateMove(bool iBool)
    {
        GetAnimator().applyRootMotion = !iBool;
        GetAnimator().SetBool("Ismoving", iBool);
    }
    


    private void CreateFoodList()
    {
        mFoodList = FoodSlotData.GetRandomFood(mFoodListLength);        
        mIsCreatedFoodList = true;
    }
    
    private void Order()
    {
        string aTextString = "";
        for (int i = 0;i< mFoodList.Count;i++) 
        {
            if (mFoodList[i].name == "Rice" && mFoodList[mFoodList.Count -1] != mFoodList[i]) 
            {
                FoodSlot aTemporarySlot = mFoodList[i];
                mFoodList[i] = mFoodList[mFoodList.Count - 1];
                mFoodList[mFoodList.Count - 1] = aTemporarySlot;                
            }
        }
        for (int i = 0; i < mFoodList.Count; i++)
        {
            aTextString += "\n" + mFoodList[i].mName;
        }
        if (mFoodList[mFoodList.Count - 1].name != "Rice") aTextString += "淨餸";
        mChatBox.HandleChangeTextDetail("我要" + aTextString);
    }

    private void PackUpLunchBox()
    {
        mCurrentState= ActorState.PickedUped;
        mChatBox.HandleCloseChatBox();
    }
    public void HandleSetStartAndStopPoint(Transform iStartPosition , Transform iEndPosition , Transform iLookPosition)
    {
        mStartPoint = iStartPosition;
        mStopPoint = iEndPosition;
        mLookPoint = iLookPosition;
    }

    public void HandleInteract(GameObject iInteractTarget)
    {
        Debug.Log(iInteractTarget);
        switch (mCurrentState)
        {
            case ActorState.Order:
                GetAnimator().Play("Talk");
                mChatBox.HandleCreateChatBox(this.transform, mTextFont);
                Order();
                mCurrentState = ActorState.Waiting;
                break;
            case ActorState.Waiting:
                LunchBoxsManager aLunchBox = iInteractTarget.GetComponent<LunchBoxsManager>();
                foreach (FoodSlot LunchBoxFood in aLunchBox.GetLunchBoxFoods())
                {
                    if (LunchBoxFood == null) return;
                }
                if (!IsLunchBoxCorrect(iInteractTarget, mFoodList))
                {
                    GetAnimator().Play("Angry");
                    mChatBox.HandleChangeTextDetail(mChatBox.RandomAngryWord());
                    Invoke("Order", 1f);
                    return;
                }
                // Correct List !!
                GetAnimator().Play("Happy");
                mChatBox.HandleChangeTextDetail(mChatBox.RandomThankYouWord());
                Invoke("PackUpLunchBox", 3f) ;
                iInteractTarget.SendMessage("HandleDropLunchBox");
                break;
        }
    }
    public void HandleSetChatBoxFont(TMP_FontAsset iFont)
    {
        mTextFont = iFont;
    }
    private bool IsLunchBoxCorrect(GameObject iLunchBox, List<FoodSlot> iFoodList)
    {
        LunchBoxsManager aLunchBoxsManager = iLunchBox.gameObject.GetComponent<LunchBoxsManager>();
        List<FoodSlot> aFoodList = iFoodList;
        foreach(FoodSlot aFood in aFoodList)
        {
            if (!aLunchBoxsManager.GetLunchBoxFoods().Contains(aFood))
            {
                return false;
            }
        }
        return true;
    }

    public bool IsArrived() 
    {
        return !GetNavMeshAgent().pathPending && GetNavMeshAgent().remainingDistance <= GetNavMeshAgent().stoppingDistance;
    }

    private Animator GetAnimator() => GetComponent<Animator>();
    private NavMeshAgent GetNavMeshAgent() => GetComponent<NavMeshAgent>();

    public ActorState GetCurrentState() => mCurrentState;
}

public class ChatBox 
{
    private TextMeshProUGUI mChatBoxText;
    protected GameObject mChatBoxObject;

    private string[] mThankYouWords = new string[3] { "Thanks", "我就係要依啲!", "唔該你" };
    private string[] mAngryWords = new string[3] { "我唔係要依啲野!", "你到底有冇聽到我要咩?", "你比錯咗我!" };
    public void HandleCreateChatBox(Transform iParents ,TMP_FontAsset iFont)
    {
        if(mChatBoxObject != null) { Debug.Log("this GameObject Already have one"); return; }
         mChatBoxObject = new GameObject("mChatBoxObject");
        mChatBoxObject.transform.SetParent(iParents);
        mChatBoxObject.transform.localPosition = Vector3.zero;
        mChatBoxObject.AddComponent<RectTransform>().transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        Canvas aChatBoxCanvas = mChatBoxObject.AddComponent<Canvas>();
        aChatBoxCanvas.renderMode = RenderMode.WorldSpace;
        GameObject aFoodList_BackGround = new GameObject("FoodList(BackGround)");
        aFoodList_BackGround.transform.SetParent(mChatBoxObject.transform);
        aFoodList_BackGround.transform.localPosition = new Vector3(-2, 2.2f, 0);
        aFoodList_BackGround.AddComponent<RectTransform>();
        aFoodList_BackGround.GetComponent<RectTransform>().sizeDelta = new Vector2(110, 55);
        aFoodList_BackGround.GetComponent<RectTransform>().localScale = new Vector2(0.025f, 0.025f);
        aFoodList_BackGround.AddComponent<Image>().color = new Color(0, 0, 0, 0.36f);
        mChatBoxText = new GameObject("FoodList(Text)").AddComponent<TextMeshProUGUI>();
        mChatBoxText.transform.SetParent(aChatBoxCanvas.transform);
        mChatBoxText.transform.localPosition = new Vector3(-2, 2.2f, 0);
        mChatBoxText.GetComponent<RectTransform>().localScale = new Vector3(0.025f, 0.025f, 0.025f);
        mChatBoxText.GetComponent<RectTransform>().sizeDelta = new Vector2(110, 55);
        mChatBoxText.font = iFont;
        mChatBoxText.fontSize = 10;
        mChatBoxText.color = Color.white;

    }
    public void HandleChangeTextDetail(string iWord)
    {
        mChatBoxText.text = iWord;
    }

    public void HandleCloseChatBox()
    {
        mChatBoxObject.SetActive(false);
    }

    public string RandomAngryWord()
    {
        int iRandom = Random.Range(0, mAngryWords.Length);
        return mAngryWords[iRandom];
    }
    public string RandomThankYouWord()
    {
        int iRandom = Random.Range(0, mThankYouWords.Length);
        return mThankYouWords[iRandom];
    }


}
