using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSlotManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> mfoodPlate = new List<GameObject>();
    [SerializeField] private List<FoodSlot> mfoodSlots = new List<FoodSlot>();
    [SerializeField] private GameObject mCooker;
    [SerializeField] private FoodSlot mRice;
    [SerializeField] private List<FoodSlot> mfoodPlateSlot = new List<FoodSlot>();

    public void Start()
    {
        Initialize();
    }
    public void Initialize()
    {
        List<FoodSlot> afoodList = mfoodSlots;
        afoodList.Add(mRice);
        FoodSlotData.HandleCreateFoodList(afoodList);
        RandomSlot();
        ChangePlateDetail();
    }

    private void RandomSlot()
    {
        List<int> aRandomNumber = new List<int>();
        aRandomNumber = Enumerable.Range(0, mfoodSlots.Count).ToList();
        aRandomNumber.OrderBy(x => Guid.NewGuid()).ToList();
        for(int i = 0; i < aRandomNumber.Count; i++)
        {
            mfoodPlateSlot.Add(mfoodSlots[aRandomNumber[i]]);
        }
    }
    private void ChangePlateDetail()
    {
        int aNumber = 0;
        foreach(GameObject iPlate in mfoodPlate)
        {
             iPlate.GetComponent<Renderer>().material = mfoodPlateSlot[aNumber].mFoodMaterial;
             aNumber++;
        }
    }

    public string GetFoodName(GameObject ifoodPate)
    {
        if (ifoodPate == mCooker) return mRice.mName;
        for (int i = 0; i< mfoodPlate.Count; i++)
        {
            if (ifoodPate == mfoodPlate[i]) return mfoodPlateSlot[i].mName;
        }
        return "Error";
    }   

    public FoodSlot GetFoodSlot(GameObject ifoodPate)
    {
        if (ifoodPate == mCooker) return mRice;
        for (int i = 0; i < mfoodPlate.Count; i++)
        {
            if (ifoodPate == mfoodPlate[i]) return mfoodPlateSlot[i];
        }
        return null;
    }
}

public static class FoodSlotData
{
    private static List<FoodSlot> mFoodList;

    public static void HandleCreateFoodList(List<FoodSlot> iFoodSlots)
    {
        mFoodList = iFoodSlots;
    }
    public static List<FoodSlot> GetFoodList() => mFoodList;

    public static List<FoodSlot> GetRandomFood(int iRandomRange)
    {
        List<FoodSlot> aRandomList = new List<FoodSlot>();
        List<int> aRandomNumber = new List<int>();
        aRandomNumber = Enumerable.Range(0, iRandomRange).ToList();
        aRandomNumber.OrderBy(x => Guid.NewGuid()).ToList();
        for (int i = 0; i < aRandomNumber.Count; i++)
        {
            aRandomList.Add(mFoodList[aRandomNumber[i]]);
        }
        return aRandomList;
    }

}
