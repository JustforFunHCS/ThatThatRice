using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LunchBoxsManager : MonoBehaviour
{
    [SerializeField]private List<GameObject> mLunchBoxArea = new List<GameObject>();
    [SerializeField]private Material mDefaultTexture;
    [SerializeField]private GameObject mLunchBox;
    private List<FoodSlot> mFoodSlots = new List<FoodSlot>();


    private void OnEnable()
    {
        mFoodSlots.Clear();
        foreach(GameObject iLunchBoxArea in mLunchBoxArea)
        {
            iLunchBoxArea.GetComponent<Renderer>().material = mDefaultTexture;
            mFoodSlots.Add(null);
        }
    }

    public void HandleAddFood(FoodSlot iFoodslot)
    {
        ExecutePlaceFood(iFoodslot,mFoodSlots);
    }
    public void HandleClearFood()
    {        
        for (int i = 0; i < mFoodSlots.Count; i++)
        {
            ExecutePlaceFoodMaterials(mLunchBoxArea[i], mDefaultTexture);
            mFoodSlots[i] = null;
        }
    }
    private void ExecutePlaceFood(FoodSlot iFoodslot,List<FoodSlot> iFoodList)
    {
        FoodSlot aFood = iFoodslot;
        List<FoodSlot> aFoodList = iFoodList;
        int aGridOfRiceSlot = aFoodList.Count - 1;
        if (aFood.name == RICE && aFoodList[aGridOfRiceSlot] == null)
        {
            aFoodList[aGridOfRiceSlot] = aFood;
            ExecutePlaceFoodMaterials(mLunchBoxArea[aGridOfRiceSlot], aFood.mFoodMaterial);
            return;
        }
        for (int i = 0; i < aFoodList.Count; i++)
        {
            if (aFoodList[i] != null) { break; }
            aFoodList[i] = aFood;
            ExecutePlaceFoodMaterials(mLunchBoxArea[i], aFood.mFoodMaterial);
        }
    }
    private void ExecutePlaceFoodMaterials(GameObject iLunchBoxObject, Material iFoodMaterial) 
    {
        GameObject aLunchBoxObject = iLunchBoxObject;
        Material aFoodMaterial = iFoodMaterial;
        aLunchBoxObject.GetComponent<Renderer>().material = aFoodMaterial;

    }

    public void HandleDropLunchBox()
    {
        HandleDisableLunchBox();
        HandleClearFood();
    }
    public void HandleEnableLunchBox()
       => mLunchBox.SetActive(true);
    
    public void HandleDisableLunchBox()
       => mLunchBox.SetActive(false);

    public bool IsLunchBoxEnable()
        => mLunchBox.activeSelf;
   
    public List<FoodSlot> GetLunchBoxFoods() 
        => mFoodSlots;

    private const string RICE = "Rice";
}
