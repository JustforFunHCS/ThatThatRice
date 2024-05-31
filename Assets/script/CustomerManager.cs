using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> mCustomerModel = new List<GameObject>();
    [SerializeField] private List<RuntimeAnimatorController> mCustomerAnimation = new List<RuntimeAnimatorController>();
    [SerializeField] private Transform mLookAtPoint, mStopPoint;
    [SerializeField] private TMP_FontAsset mFont;

    private CoolDownTimer mCoolDownTimer = new CoolDownTimer();
    [SerializeField]
    private List<GameObject> mCustomers = new List<GameObject>();

    private int mLimitOfCustomer = 1;
    private int mCurrentNumberOfCustomer;
    private const string CUSTOMER = "Customer";
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < mLimitOfCustomer; i++)
        {
            mCustomers.Add(null);
        }
    }
    void Update()
    {
        if (mCurrentNumberOfCustomer < mLimitOfCustomer && mCoolDownTimer.IsCoolDownComplete())
        {
            SpawnCustomer();
        }
    }


    private void SpawnCustomer()
    {
        mCoolDownTimer.handleChangeAnount(Random.Range(1, 5));
        int aModelNumber = Random.Range(0, mCustomerModel.Count);
        int aAnimatorCtrl = Random.Range(0, mCustomerAnimation.Count);
        for (int i = 0; i < mCustomers.Count; i++)
        {
            if (mCustomers[i] == null)
            {
                GameObject aTheChosenOne = Instantiate(mCustomerModel[aModelNumber], transform.position, transform.rotation);
                aTheChosenOne.tag = CUSTOMER;
                aTheChosenOne.GetComponent<Animator>().runtimeAnimatorController = mCustomerAnimation[aAnimatorCtrl];
                aTheChosenOne.AddComponent<NavMeshAgent>();
                aTheChosenOne.AddComponent<CustomerControl>().HandleSetStartAndStopPoint(transform, mStopPoint, mLookAtPoint);
                aTheChosenOne.GetComponent<CustomerControl>().HandleSetChatBoxFont(mFont);
                mCustomers[i] = aTheChosenOne;
                mCurrentNumberOfCustomer++;
                break;
            }
        }

        mCoolDownTimer.HandleStartCoolDown();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);
        if (mCustomers.Contains(other.gameObject) && other.gameObject.GetComponent<CustomerControl>().GetCurrentState() == CustomerControl.ActorState.PickedUped)
        {
            for (int i = 0; i < mCustomers.Count; i++)
            {
                if (mCustomers[i] == other.gameObject)
                {
                    mCustomers[i] = null;                    
                    Destroy(other.gameObject);
                    mCurrentNumberOfCustomer--;
                }
            }

        }
    }
}
    public class CoolDownTimer
    {
        [SerializeField]
        private float mCoolDownAmount = 0.5f;

        private float mCoolDownCompleteTime = 0;
        [SerializeField]
        private bool mIsCoolDownComplete = true;
        public bool IsCoolDownComplete()
        {
            return mIsCoolDownComplete = Time.time >= mCoolDownCompleteTime ? true : false;
        }

        public void HandleStartCoolDown()
        {
            mCoolDownCompleteTime = Time.time + mCoolDownAmount;
        }

        public void handleChangeAnount(float iAmount)
        {
            mCoolDownAmount = iAmount;
        }
    }