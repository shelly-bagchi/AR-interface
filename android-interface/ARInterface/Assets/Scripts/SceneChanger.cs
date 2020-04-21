using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// https://docs.unity3d.com/2018.3/Documentation/ScriptReference/UI.Dropdown-onValueChanged.html

public class SceneChanger : MonoBehaviour {
    private Dropdown sceneDropdown;
    // Use this for initialization
    void Start () {
		sceneDropdown = GetComponent<Dropdown>();
        sceneDropdown.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(sceneDropdown);
        });
    }
	
	// Update is called once per frame
	void Update () {}

    // Output the new value of the Dropdown
    void DropdownValueChanged(Dropdown change)
    {
        Debug.Log("Changing to scene " + change.value);
        // With scene chooser at start
        //SceneManager.LoadSceneAsync(change.value + 1, LoadSceneMode.Single);
        // Without (launches directly into scene)
        SceneManager.LoadSceneAsync(change.value, LoadSceneMode.Single);
    }
}
