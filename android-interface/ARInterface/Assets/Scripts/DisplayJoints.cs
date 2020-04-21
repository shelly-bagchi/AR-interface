using UnityEngine;
using UnityEngine.UI;


using BioIK;


public class DisplayJoints : MonoBehaviour
{
    public InputField outputText;
    public Toggle outputTypeToggle;  // May get rid of this
    public GameObject shoulder, upperarm, forearm, wrist1, wrist2, wrist3;

    private BioJoint j1, j2, j3, j4, j5, j6;
    private double j1val, j2val, j3val, j4val, j5val, j6val;
    

    // Use this for initialization
    void Start () {

        j1 = shoulder.GetComponent<BioJoint>();
        j2 = upperarm.GetComponent<BioJoint>();
        j3 = forearm.GetComponent<BioJoint>();
        j4 = wrist1.GetComponent<BioJoint>();
        j5 = wrist2.GetComponent<BioJoint>();
        j6 = wrist3.GetComponent<BioJoint>();

        outputText.text = "0.0  0.0  0.0  0.0  0.0  0.0  ";

    }

    // Update is called once per frame
    void Update() {
        j1val = j1.Z.CurrentValue;
        j2val = 0.0 - j2.Z.CurrentValue;
        j3val = j3.Z.CurrentValue;
        j4val = 0.0 - j4.X.CurrentValue;
        j5val = 0.0 - j5.Y.CurrentValue;
        j6val = j6.X.CurrentValue;

        //Debug.Log(j1val.ToString() + ',' + j2val.ToString() + ',' + j3val.ToString() + ',' +
        //    j4val.ToString() + ',' + j5val.ToString() + ',' + j6val.ToString());

    }

    // Use to update UI elements
    void LateUpdate () {

        outputText.text = string.Format("({0:0.0}, {1:0.0}, {2:0.0}, {3:0.0}, {4:0.0}, {5:0.0})",
                j1val, j2val, j3val, j4val, j5val, j6val);
    }
}
