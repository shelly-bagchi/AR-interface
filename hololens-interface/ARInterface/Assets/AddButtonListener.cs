using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddButtonListener : MonoBehaviour {
    private Button thisButton;
    public GameObject ConnectButton;

    // Use this for initialization
    void Start ()
    {
        thisButton = this.GetComponent<Button>();
        thisButton.onClick.AddListener(() => ConnectButton.GetComponent<GetFromServer_Minimal>().TaskOnClick(thisButton));


    }

    // Update is called once per frame
    void Update () {
		
	}
}
