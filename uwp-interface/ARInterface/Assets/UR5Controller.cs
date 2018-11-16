// Author: Long Qian
// Email: lqian8@jhu.edu

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Events;

// Needed //////////////////////////////////////////////////
//using HoloLensXboxController;
///////////////////////////////////////////////////////////

public class UR5Controller : MonoBehaviour {

    public GameObject RobotBase;
    public float[] jointValues = new float[6];
    private GameObject[] jointList = new GameObject[6];
    private float[] upperLimit = { 180f, 180f, 180f, 180f, 180f, 180f };
    private float[] lowerLimit = { -180f, -180f, -180f, -180f, -180f, -180f };

    public GameObject CanvasObj;
    private Slider[] sliderList = new Slider[6];

    public InputField TextControl;

    // Needed //////////////////////////////////////////////////
    //private ControllerInput controllerInput;
    ///////////////////////////////////////////////////////////

    // Use this for initialization
    void Start()
    {
        initializeJoints();
        initializeSliders();

        TextControl.text = "(0,0,0,0,0,0)";

        // Needed //////////////////////////////////////////////////
        //controllerInput = new ControllerInput(0, 0.19f);
        // First parameter is the number, starting at zero, of the controller you want to follow.
        // Second parameter is the default “dead” value; meaning all stick readings less than this value will be set to 0.0.
        ///////////////////////////////////////////////////////////

        //Canvas myCanvas = CanvasObj.GetComponent<Canvas>();
        //CanvasObj.AddComponent<TextMesh>();
        //CanvasObj.GetComponent<TextMesh>().text = "hi!!!";

    }

    // Update is called once per frame
    void Update() {
        TextControl.text = string.Format("({0:0.0}, {1:0.0}, {2:0.0}, {3:0.0}, {4:0.0}, {5:0.0})",
            jointValues[5], jointValues[4], jointValues[3],
            jointValues[2], jointValues[1], jointValues[0]);

        // Needed //////////////////////////////////////////////////
        //controllerInput.Update();
        ///////////////////////////////////////////////////////////
    }

    // Right before camera renders
    void LateUpdate() {

        for (int i = 0; i < 6; i++)
        {
            Vector3 currentRotation = jointList[i].transform.localEulerAngles;
            //Debug.Log(currentRotation);
            currentRotation.z = jointValues[i];
            jointList[i].transform.localEulerAngles = currentRotation;
        }
    }

    void OnGUI() {

        /*#if UNITY_EDITOR
                int boundary = 20;
                int labelHeight = 20;
                GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = 20;
                GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                for (int i = 0; i < 6; i++) {
                    GUI.Label(new Rect(boundary, boundary + (i * 2 + 1) * labelHeight, labelHeight * 4, labelHeight), "Joint " + i + ": ");
                    jointValues[i] = GUI.HorizontalSlider(new Rect(boundary + labelHeight * 4, boundary + (i * 2 + 1) * labelHeight + labelHeight / 4, labelHeight * 5, labelHeight), jointValues[i], lowerLimit[i], upperLimit[i]);
                }
        #else        
                Debug.Log("Entered else");
        #endif*/


        for (int i = 0; i < 6; i++) {
            jointValues[i] = sliderList[i].value;
        }
    }


    // Create the list of GameObjects that represent each joint of the robot
    void initializeJoints() {
        var RobotChildren = RobotBase.GetComponentsInChildren<Transform>();
        for (int i = 0; i < RobotChildren.Length; i++)
        {
            if (RobotChildren[i].name == "control0")
            {
                jointList[0] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "control1")
            {
                jointList[1] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "control2")
            {
                jointList[2] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "control3")
            {
                jointList[3] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "control4")
            {
                jointList[4] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "control5")
            {
                jointList[5] = RobotChildren[i].gameObject;
            }
        }
    }

    // Create the list of GameObjects that represent each slider in the canvas
    void initializeSliders() {
        var CanvasChildren = CanvasObj.GetComponentsInChildren<Slider>();

        for (int i = 0; i < CanvasChildren.Length; i++) {
            if (CanvasChildren[i].name == "Slider0")
            {
                sliderList[0] = CanvasChildren[i];

                sliderList[0].minValue = lowerLimit[0];
                sliderList[0].maxValue = upperLimit[0];
                sliderList[0].value = 0;
            }
            else if (CanvasChildren[i].name == "Slider1")
            {
                sliderList[1] = CanvasChildren[i];

                sliderList[1].minValue = lowerLimit[1];
                sliderList[1].maxValue = upperLimit[1];
                sliderList[1].value = 0;
            }
            else if (CanvasChildren[i].name == "Slider2")
            {
                sliderList[2] = CanvasChildren[i];

                sliderList[2].minValue = lowerLimit[2];
                sliderList[2].maxValue = upperLimit[2];
                sliderList[2].value = 0;
            }
            else if (CanvasChildren[i].name == "Slider3")
            {
                sliderList[3] = CanvasChildren[i];

                sliderList[3].minValue = lowerLimit[3];
                sliderList[3].maxValue = upperLimit[3];
                sliderList[3].value = 0;
            }
            else if (CanvasChildren[i].name == "Slider4")
            {
                sliderList[4] = CanvasChildren[i];

                sliderList[4].minValue = lowerLimit[4];
                sliderList[4].maxValue = upperLimit[4];
                sliderList[4].value = 0;
            }
            else if (CanvasChildren[i].name == "Slider5")
            {
                sliderList[5] = CanvasChildren[i];

                sliderList[5].minValue = lowerLimit[5];
                sliderList[5].maxValue = upperLimit[5];
                sliderList[5].value = 0;
            }
        }
    }
}
