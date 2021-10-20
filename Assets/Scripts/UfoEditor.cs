using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UfoEditor : MonoBehaviour
{
    private GameObject _ui;
    private GameObject _rings;
    [SerializeField] private GameObject buttonChooseBottom;
    [SerializeField] private GameObject buttonChooseMiddle;
    private readonly List<string> _partCategories = new List<string> { "bottoms", "middles", "tops", "rings" };
    private readonly List<List<GameObject>> _all = new List<List<GameObject>>();

    void Start()
    {
        _ui = GameObject.Find("UI");
        _rings = transform.Find("rings").gameObject;

        int catI = 0;
        foreach (string category in _partCategories)
        {
            int childI = 0;
            List<GameObject> go = new List<GameObject>();
            foreach (Transform child in transform.Find(category).transform)
            {
                go.Add(child.gameObject);

                var button = Instantiate(buttonChooseBottom, _ui.transform);
                button.GetComponent<RectTransform>().transform.position = new Vector3(childI++ * 50 + 50, catI * 50 + 200, 0);
                button.transform.Find("Text").GetComponent<Text>().text = childI.ToString();
                // button.name = $"ButtonChoose_{category}_{childI - 1}";
                button.name = $"ButtonChoose-{category}_{catI}_{childI - 1}";
                button.GetComponent<Button>().onClick.AddListener(ShowPart);
                child.gameObject.SetActive(false);
                // go.Add(child.gameObject);
            }

            ++catI;
            _all.Add(go);
        }
        // print(_all[0]);
        // print(_all[1].gameObject.name);
        // print(_all[2].gameObject.name);
        // print(_all[3].gameObject.name);
    }

    void ShowPart()
    {
        // print(EventSystem.current.currentSelectedGameObject.name);
        var buttonNameParsed = EventSystem.current.currentSelectedGameObject.name.Split('_');
        int catNo = int.Parse(buttonNameParsed[1]);
        int partNo = int.Parse(buttonNameParsed[2]);

        //print(int.Parse(buttonNameParsed[1]));

        int i = 0;
        foreach (GameObject child in _all[catNo])
        {
            if (i == partNo /*&& !child.activeSelf*/)
                child.SetActive(!child.activeSelf);
            else if (i != partNo && child.activeSelf)
                child.SetActive(false);
            i++;
        }
    }
    
    void FixedUpdate()
    {
        transform.eulerAngles = new Vector3(Mathf.Sin(Time.time * .8f) * 14, Time.time * 20, 0);
        _rings.transform.localEulerAngles = new Vector3(0, - Time.time * 100, 0);
    }
}
