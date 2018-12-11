using UnityEngine;
using UnityEngine.UI;

public class reset : MonoBehaviour {
    private Button thisButton;

    public GameObject touchControl;
    private Vector3 startPos = new Vector3();

    // Use this for initialization
    void Start () {
        thisButton = this.GetComponent<Button>();
        thisButton.onClick.AddListener(TaskOnClick);

        startPos = touchControl.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void TaskOnClick()
    {
        touchControl.transform.position = startPos;
    }
}
