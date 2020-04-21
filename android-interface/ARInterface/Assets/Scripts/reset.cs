using UnityEngine;
using UnityEngine.UI;

public class reset : MonoBehaviour {
    private Button thisButton;

    public GameObject touchControl_f, touchControl_d;
    private Vector3 startPos_f = new Vector3();
    private Vector3 startPos_d = new Vector3();

    //public GameObject RobotModel;
    //public GameObject DummyParent;


    // Use this for initialization
    void Start () {
        thisButton = this.GetComponent<Button>();
        thisButton.onClick.AddListener(TaskOnClick);

        startPos_f = touchControl_f.transform.position;
        startPos_d = touchControl_d.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void TaskOnClick()
    {
        // Reset robot to default location
        /*Transform myModelTrf = RobotModel.transform;
        myModelTrf.parent = DummyParent.transform;
        myModelTrf.localPosition = new Vector3(0f, -0.25f, 2.5f);
        myModelTrf.localRotation = Quaternion.identity;
        myModelTrf.localScale = new Vector3(1f, 1f, 1f);
        myModelTrf.gameObject.SetActive(true);*/


        // Reset location of IK target sphere
        touchControl_f.transform.position = startPos_f;
        touchControl_d.transform.position = startPos_d;
    }
}
