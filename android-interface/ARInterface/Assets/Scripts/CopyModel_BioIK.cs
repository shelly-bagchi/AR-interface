using UnityEngine;
using BioIK;

public class CopyModel_BioIK : MonoBehaviour {
    public GameObject inputModel;//, outputModel;
    //public GameObject shoulder, upperarm, forearm, wrist1, wrist2, wrist3, EE;

    public BioJoint[] j_in = new BioJoint[6];
    public BioJoint[] j_out = new BioJoint[6];
    public GameObject EE_out, IKSphere_out;

    private GameObject[] jointListIn = new GameObject[6];
    private GameObject[] jointListOut = new GameObject[6];

    // Use this for initialization
    void Start ()
    {
    }

    // Update is called once per frame
    void Update ()
    {
        if (inputModel.activeSelf)
        {
            j_out[0].Z.SetTargetValue(j_in[0].Z.CurrentValue);
            j_out[0].Z.ProcessMotion(MotionType.Instantaneous);
            j_out[1].Z.SetTargetValue(j_in[1].Z.CurrentValue);
            j_out[1].Z.ProcessMotion(MotionType.Instantaneous);
            j_out[2].Z.SetTargetValue(j_in[2].Z.CurrentValue);
            j_out[2].Z.ProcessMotion(MotionType.Instantaneous);
            j_out[3].X.SetTargetValue(j_in[3].X.CurrentValue);
            j_out[3].X.ProcessMotion(MotionType.Instantaneous);
            j_out[4].Y.SetTargetValue(j_in[4].Y.CurrentValue);
            j_out[4].Y.ProcessMotion(MotionType.Instantaneous);
            j_out[5].X.SetTargetValue(j_in[5].X.CurrentValue);
            j_out[5].X.ProcessMotion(MotionType.Instantaneous);

            IKSphere_out.transform.SetPositionAndRotation(EE_out.transform.position, EE_out.transform.rotation);
        }

    }

    // Before camera renders
    void LateUpdate()
    {


    }

    

}
