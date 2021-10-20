using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonChooseUfoPart : MonoBehaviour 
{
    private UfoEditor _ufoEditor;
    void Start()
    {
        _ufoEditor = GameObject.Find("UfoParts").GetComponent<UfoEditor>();
    }

    public void OnClick()
    {
        print("click!");
    }

    public void OnMouseDown()
    {
        print("click!!");
    }

    public void Test()
    {
        print("click!!!");
    }

    public void OnPointerClick()
    {
        print("click!!!");
    }
    
    // this.onClick.AddListener(TaskOnClick);
}
