using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;


using ARInterface;

/*
//#if UNITY_WSA
#if WINDOWS_UWP
// https://docs.microsoft.com/en-us/windows/uwp/networking/sockets
// https://docs.microsoft.com/en-us/windows/uwp/networking/networking-basics
using Windows.Networking;
using Windows.Networking.Sockets;
//using Windows.Networking.Connectivity;
//using HoloToolkit.Unity;
//using HoloToolkit.Sharing;
//using HoloToolkit.Sharing.Tests;
#else
// https://msdn.microsoft.com/en-us/library/kb5kfec7(v=vs.110).aspx
//using UnityEngine.Networking;
//using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Networking;
#endif*/




public class GetFromServer : MonoBehaviour {
    private Button thisButton;

    public InputField crpiIP;
    public Dropdown CommandInput;
    public InputField MessageOutput;


    public UR5Controller controller;
    private Slider[] sliderList = new Slider[6];

    // Create class to hold client details
    private Client crpiClient;


    void Start()
    {
        thisButton = this.GetComponent<Button>();
        thisButton.onClick.AddListener(TaskOnClick);        
        
        sliderList = controller.getSliderList();


        //crpiIP.text = "127.0.0.1";  // Local
        //crpiIP.text = "169.254.80.80";  // Local?

        //crpiIP.text = "169.254.152.40";  // Ethernet - CollabLab-A
        //crpiIP.text = "169.254.152.39";  // Ethernet??  Temp
        //crpiIP.text = "169.254.152.53";  // Wifi
        crpiIP.text = "129.6.108.19";  // Wifi - NISTnet

        //crpiIP.text = "169.254.152.10";  // Jeremy's laptop

        crpiClient = new Client(crpiIP, 20602);
    }

    public Toggle liveUpdateToggle;
    public Toggle plotForcesToggle;
    float timeCounter = 0.0f;

    void Update()
    {

        // Increment timer
        timeCounter += Time.deltaTime;

        if (crpiClient.isConnected && timeCounter>=0.1f)
        {
            //Debug.LogFormat("Timer: {0}", timeCounter);


            if (liveUpdateToggle.isOn)
            {
                // Live update axes
                string ans = crpiClient.sendMsg("Get Axes");
                Debug.LogFormat("Robot Axes: {0}", ans);
                setSliders(ans);
            }

            if (plotForcesToggle.isOn)
            {
                // Plot forces at every interval
                string ans2 = crpiClient.sendMsg("Get Forces");
                Debug.LogFormat("Robot Forces: {0}", ans2);
                //TODO:  plotForces(ans2);
            }


            timeCounter = 0.0f;
        }

    }

    void TaskOnClick() {
        //Debug.Log("You have clicked the button!");
        String cmd = CommandInput.captionText.text;
        Debug.LogFormat(cmd);

        // Encode the data string into a byte array.
        string ans = crpiClient.sendMsg(cmd);


        if (cmd.Equals("Get Pose"))
        {
            Debug.LogFormat("Robot Pose: {0}", ans);
        }

        else if (cmd.Equals("Get Forces"))
        {
            Debug.LogFormat("Robot Forces: {0}", ans);
        }

        else if (cmd.Equals("Get Axes"))
        {
            Debug.LogFormat("Robot Axes: {0}", ans);
            setSliders(ans);
        }

        // -- NEW:  Send joint angles from robot --
        else if (cmd.Equals("Send Axes"))
        {
            String axes = getSliders();
            Debug.LogFormat("Axes from sliders: {0}", axes);

            // Encode the data string into a byte array.
            ans = crpiClient.sendMsg(axes.ToString());

            Debug.LogFormat("Sent axes to robot.");

                
        }

        MessageOutput.text = String.Format("{0}", ans);


    }



    // Set sliders to joint values rcvd from robot
    String getSliders()
    {
        String res = "";
        //Double[] axes = new Double[6];

        float tempVal = 0.0f;
        for (int i = 0; i < 6; i++)
        {
            // Offsets - ideally opposite of set
            switch (i)
            {
                case 0:
                    tempVal = -1.0f * (sliderList[i].value + 45.0f);
                    break;
                case 1:
                    tempVal = sliderList[i].value - 90.0f;
                    break;
                case 3:
                    tempVal = sliderList[i].value - 90.0f;
                    break;
                case 4:
                    tempVal = -1.0f * sliderList[i].value;
                    break;
                default:
                    tempVal = sliderList[i].value;
                    break;
            }

            // Check if out of bounds and loop around
            if (tempVal > 180.0f) 
            {
                tempVal = -180.0f + (tempVal - 180.0f);
            }
            else if (tempVal < -180.0f) 
            {

                tempVal = 180.0f + (tempVal + 180.0f);
            }

            // Save modified slider value
            //axes[i] = (double)tempVal;
            res += tempVal.ToString() + ",";
        }
        //res += ")";

        return res;
    }


    // Set sliders to joint values rcvd from robot
    void setSliders(string input)
    {
        input = input.Replace('(', ' ').Replace(')', ' ');
        //Debug.Log(input);
        //Double[] axes = Array.ConvertAll(input.Split(','), Double.Parse);
        string[] splitStr = input.Split(',');
        Double[] axes = splitStr.Select(x => Double.Parse(x)).ToArray();
        Debug.LogFormat("Converted Axes: {0}...", axes[0]);

        double tempVal = 0.0;
        for (int i = 0; i < 6; i++)
        {
            // Offsets
            switch (i)
            {
                case 0:
                    tempVal = (-1.0 * axes[i]) - 45.0;
                    break;
                case 1:
                    tempVal = axes[i] + 90.0;
                    break;
                case 3:
                    tempVal = axes[i] + 90.0;
                    break;
                case 4:
                    tempVal = -1.0 * axes[i];
                    break;
                default:
                    tempVal = axes[i];
                    break;
            }

            // Check if out of bounds and loop around
            if (tempVal > 180.0) 
            {
                tempVal = -180.0 + (tempVal - 180.0);
            }
            else if (tempVal < -180.0) 
            {

                tempVal = 180.0 + (tempVal + 180.0);
            }

            // Set slider, which will set model
            sliderList[i].value = (float)tempVal;
        }
    }

    

    // Set plot to updated force values
    public ParticleSystem plot;

    [Range(10, 100)]
    public int resolution = 10;

    private int currentResolution;
    private ParticleSystem.Particle[] points;

    private Double[] forceHistory = new Double[10];

    private void plotForces(string input)
    {
        input = input.Replace('(', ' ').Replace(')', ' ');
        //Double[] forces = Array.ConvertAll(input.Split(','), Double.Parse);
        string[] splitStr = input.Split(',');
        Double[] forces = splitStr.Select(x => Double.Parse(x)).ToArray();


        if (currentResolution != resolution || points == null)
        {
            CreatePoints();
        }


        Array.Copy(forceHistory, 1, forceHistory, 0, forceHistory.Length-1);
        forceHistory[forceHistory.Length-1] = forces[0];
        Debug.LogFormat("forceHistory: {0}", forceHistory);


        for (int i = 0; i < resolution; i++)
        {
            Vector3 p = points[i].position;
            p.y = (float)forceHistory[i];
            points[i].position = p;
            Color c = points[i].color;
            c.g = p.y;
            points[i].color = c;
        }


        plot.GetComponent<ParticleSystem>().SetParticles(points, points.Length);
    }


    private void CreatePoints()
    {
        currentResolution = resolution;
        points = new ParticleSystem.Particle[resolution];
        float increment = 1f / (resolution - 1);
        for (int i = 0; i < resolution; i++)
        {
            float x = i * increment;
            points[i].position = new Vector3(x, 0f, 0f);
            points[i].color = new Color(x, 0f, 0f);
            points[i].size = 0.1f;
        }
    }


}