using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextControler : MonoBehaviour
{
    [SerializeField]private Text mObjectInfoText;
    [SerializeField]private GameObject mTextBackGround;
    [SerializeField]private Color mFideColor;

    private void OnEnable()
    {       
        HandleHideTextGameObject();
    }
    public void HandleHideTextGameObject()
    {
        mTextBackGround.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        mObjectInfoText.text = "";
    }

    public void HandleChangeTextDetail(string iTextDetail)
    {
        Debug.Log(iTextDetail);
        mTextBackGround.GetComponent<Image>().color = mFideColor;
        mObjectInfoText.text = iTextDetail;
    }
}
