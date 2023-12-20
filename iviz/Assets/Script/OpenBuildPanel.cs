using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenBuildPanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void ButtonClick()
    {
        Debug.Log("Listener 的方法！");
    }
}
 