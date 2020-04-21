using UnityEngine;

public class CopyModel : MonoBehaviour {
    public GameObject inputModel;
    private GameObject[] jointListIn = new GameObject[6];
    private GameObject[] jointListOut = new GameObject[6];

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // Right before camera renders
    void LateUpdate()
    {

        for (int i = 0; i < 6; i++)
        {
            Vector3 currentRotation = jointListIn[i].transform.localEulerAngles;
            //Debug.Log(currentRotation);
            //currentRotation.z = jointValues[i];
            jointListOut[i].transform.localEulerAngles = currentRotation;
        }
    }
    


}
