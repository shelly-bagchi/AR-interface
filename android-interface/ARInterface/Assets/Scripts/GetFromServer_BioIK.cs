using UnityEngine;
using UnityEngine.UI;

using BioIK;

// From https://msdn.microsoft.com/en-us/library/kb5kfec7(v=vs.110).aspx
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

//using UnityEditor.PackageManager.Client;


public class GetFromServer_BioIK : MonoBehaviour {
    private Button thisButton;

    public InputField crpiIP;
    public Dropdown CommandInput;
    public InputField MessageOutput;

    public InputField JointOutput;
    
    //public GameObject shoulder, upperarm, forearm, wrist1, wrist2, wrist3, EE;
    // f=fixed, d=dynamic (Vuforia/AR)
    public GameObject[] robot_f = new GameObject[6];
    public GameObject[] robot_d = new GameObject[6];
    public GameObject EE_f, EE_d;
    private BioJoint[] joints_f = new BioJoint[6];
    private BioJoint[] joints_d = new BioJoint[6];
    //private BioJoint TCP = new BioJoint();
    private double[] jointVals = new double[6];
    public GameObject IKSphere_v, IKSphere;

    //public Slider gripperControl;
    //private Slider[] sliderList = new Slider[6];

    // Use class to hold client details
    private Client crpiClient;


    public Toggle liveUpdateToggle;
    float timeCounter = 0.0f;
    //public Toggle plotForcesToggle;
    public Toggle modelToggle;
    private Boolean ARModel = false;


    void Start()
    {
        thisButton = this.GetComponent<Button>();
        thisButton.onClick.AddListener(TaskOnClick);

        modelToggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(modelToggle);
        });

        //crpiClient.connect();

        //crpiIP.text = "127.0.0.1";  // Local
        //crpiIP.text = "169.254.80.80";  // Local...?

        //crpiIP.text = "169.254.152.40";  // Ethernet - CollabLab-A
        //crpiIP.text = "169.254.152.39";  // Ethernet??  Temp
        crpiIP.text = "169.254.152.52";  // Wifi
        //crpiIP.text = "129.6.108.19";  // Wifi - NISTnet
        //129.6.35.80

        //crpiIP.text = "169.254.152.10";  // Jeremy's laptop

        crpiClient = new Client(crpiIP, 20602);


        JointOutput.text = "0.0  0.0  0.0  0.0  0.0  0.0  ";

        //RobotSetup();
        JointSetup();

    }

    // Note - doesn't work, needs hierarchy and full names
    /*
    void RobotSetup()
    {
        shoulder =  robot.transform.Find("shoulder_link").gameObject;
        upperarm =  robot.transform.Find("upper_arm_link").gameObject;
        forearm =   robot.transform.Find("forearm_link").gameObject;
        wrist1 =    robot.transform.Find("wrist_1_link").gameObject;
        wrist2 =    robot.transform.Find("wrist_2_link").gameObject;
        wrist3 =    robot.transform.Find("wrist_3_link").gameObject;
        EE =        robot.transform.Find("ee_link").gameObject;
    }*/

    void JointSetup()
    {
        for(int i=0; i<6; i++)
        {
            joints_f[i] = robot_f[i].GetComponent<BioJoint>();
            joints_d[i] = robot_d[i].GetComponent<BioJoint>();
        }
        /*j[1] =  upperarm.GetComponent<BioJoint>();
        j[2] =  forearm.GetComponent<BioJoint>();
        j[3] =  wrist1.GetComponent<BioJoint>();
        j[4] =  wrist2.GetComponent<BioJoint>();
        j[5] =  wrist3.GetComponent<BioJoint>();*/
        //TCP =   EE.GetComponent<BioJoint>();
    }


    // Handle toggle change
    void ToggleValueChanged(Toggle change)
    {
        if (modelToggle.isOn)
        {
            ARModel = true;
        }
        else
        {
            ARModel = false;
        }

    }


    void Update()
    {
        // Increment timer
        timeCounter += Time.deltaTime;

        // Check if client exists
        /*if (crpiClient == null)
        {
            crpiClient = new client(crpiIP.text, 20602);
        }*/

        if (crpiClient.isConnected && timeCounter>=0.1f)
        {
            //Debug.LogFormat("Timer: {0}", timeCounter);

            // Check for live update status and request pose if so
            if (liveUpdateToggle.isOn)
            {
                // Live update axes  
                byte[] msg = Encoding.ASCII.GetBytes("Get Axes");
                string ans = crpiClient.sendMsg(msg);
                Debug.LogFormat("Robot Axes: {0}", ans);
                //setSliders(ans);
                setModelAxes(ans);
            }

            // Do other stuff here while connected


            /*if (plotForcesToggle.isOn)
            {
                // Plot forces at every interval 
                byte[] msg2 = Encoding.ASCII.GetBytes("Get Forces");
                string ans2 = crpiClient.sendMsg(msg2);
                Debug.LogFormat("Robot Forces: {0}", ans2);
                plotForces(ans2);
            }*/


            timeCounter = 0.0f;
        }

        // Check which model is being shown
        /*if (modelToggle.isOn)
        {
            ARModel = true;
        }
        else
        {
            ARModel = false;
        }*/

    }


    // Use to update UI elements
    void LateUpdate()
    {
        if (ARModel)
        {
            jointVals[0] = joints_d[0].Z.CurrentValue;
            jointVals[1] = joints_d[1].Z.CurrentValue;
            jointVals[2] = joints_d[2].Z.CurrentValue;
            jointVals[3] = joints_d[3].X.CurrentValue;
            jointVals[4] = joints_d[4].Y.CurrentValue;
            jointVals[5] = joints_d[5].X.CurrentValue;
        }
        else
        {
            jointVals[0] = joints_f[0].Z.CurrentValue;
            jointVals[1] = joints_f[1].Z.CurrentValue;
            jointVals[2] = joints_f[2].Z.CurrentValue;
            jointVals[3] = joints_f[3].X.CurrentValue;
            jointVals[4] = joints_f[4].Y.CurrentValue;
            jointVals[5] = joints_f[5].X.CurrentValue;
        }

        JointOutput.text = string.Format("({0:0.0}, {1:0.0}, {2:0.0}, {3:0.0}, {4:0.0}, {5:0.0})",
                jointVals[0], jointVals[1], jointVals[2], jointVals[3], jointVals[4], jointVals[5]);

    }


    void TaskOnClick() {
        //Debug.Log("You have clicked the button!");
        String cmd = CommandInput.captionText.text;
        Debug.LogFormat(cmd);

        // Encode the data string into a byte array & send 
        byte[] msg = Encoding.ASCII.GetBytes(cmd);
        string ans = crpiClient.sendMsg(msg);


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
            //setSliders(ans);
            setModelAxes(ans);
        }

        // --- Send joint angles from robot ---
        else if (cmd.Equals("Send Axes"))
        {
            //String axes = getSliderVals();
            String axes = getModelAxes();
            Debug.LogFormat("Axes from model: {0}", axes);

            //Send second message
            // Encode the data string into a byte array.  
            msg = Encoding.ASCII.GetBytes(axes.Substring(1, axes.Length - 2));
            // Temp - test encoding
            Debug.Log(Encoding.ASCII.GetString(msg));
            ans = crpiClient.sendMsg(msg);

            Debug.LogFormat("Sent axes to robot.");
        }


        // --- Turn 0-G mode on/off ---
        else if (cmd.Equals("Zero-G Mode"))
        {
            Debug.LogFormat("0-G Mode: {0}", ans);  // on or off
            // Need to enable live update when activated
            if (ans=="on") {
                liveUpdateToggle.isOn = true;
            }
            else {
                liveUpdateToggle.isOn = false;
            }
        }


        // --- NEW:  Toggle gripper ---
        else if (cmd.Equals("Toggle Gripper"))
        {
            Debug.LogFormat("Gripper: {0}", ans);  // open or closed
        }

        MessageOutput.text = String.Format("{0}", ans);


    }

    public String getModelAxes()
    {
        if (ARModel)
        {
            jointVals[0] = -1 *  joints_d[0].Z.CurrentValue;
            jointVals[1] = -1 *  joints_d[1].Z.CurrentValue;
            jointVals[2] =       joints_d[2].Z.CurrentValue;
            jointVals[3] = -1 *  joints_d[3].X.CurrentValue;
            jointVals[4] = -1 *  joints_d[4].Y.CurrentValue;
            jointVals[5] =       joints_d[5].X.CurrentValue;
        }
        else
        {
            jointVals[0] = -1 *  joints_f[0].Z.CurrentValue;
            jointVals[1] = -1 *  joints_f[1].Z.CurrentValue;
            jointVals[2] =       joints_f[2].Z.CurrentValue;
            jointVals[3] = -1 *  joints_f[3].X.CurrentValue;
            jointVals[4] = -1 *  joints_f[4].Y.CurrentValue;
            jointVals[5] =       joints_f[5].X.CurrentValue;

        }

        return string.Format("({0:0.0}, {1:0.0}, {2:0.0}, {3:0.0}, {4:0.0}, {5:0.0})",
                jointVals[0], jointVals[1], jointVals[2], jointVals[3], jointVals[4], jointVals[5]);
    }


    public void setModelAxes(String input)  // set target sphere also!
    {
        input = input.Replace('(', ' ').Replace(')', ' ');
        //Debug.Log(input);
        Double[] axes = Array.ConvertAll(input.Split(','), Double.Parse);
        //Debug.LogFormat("Converted Axes: {0}...", axes[0]);


        if (ARModel)
        {
            joints_d[0].Z.SetTargetValue(-1 * axes[0]);
            joints_d[0].Z.ProcessMotion(MotionType.Instantaneous);
            joints_d[1].Z.SetTargetValue(-1 * axes[1]);
            joints_d[1].Z.ProcessMotion(MotionType.Instantaneous);
            joints_d[2].Z.SetTargetValue(axes[2]);
            joints_d[2].Z.ProcessMotion(MotionType.Instantaneous);
            joints_d[3].X.SetTargetValue(-1 * axes[3]);
            joints_d[3].X.ProcessMotion(MotionType.Instantaneous);
            joints_d[4].Y.SetTargetValue(-1 * axes[4]);
            joints_d[4].Y.ProcessMotion(MotionType.Instantaneous);
            joints_d[5].X.SetTargetValue(axes[5]);
            joints_d[5].X.ProcessMotion(MotionType.Instantaneous);

            IKSphere_v.transform.SetPositionAndRotation(EE_d.transform.position, EE_d.transform.rotation);
            IKSphere.transform.SetPositionAndRotation(EE_d.transform.position, EE_d.transform.rotation);
        }
        else
        {
            joints_f[0].Z.SetTargetValue(-1 * axes[0]);
            joints_f[0].Z.ProcessMotion(MotionType.Instantaneous);
            joints_f[1].Z.SetTargetValue(-1 * axes[1]);
            joints_f[1].Z.ProcessMotion(MotionType.Instantaneous);
            joints_f[2].Z.SetTargetValue(axes[2]);
            joints_f[2].Z.ProcessMotion(MotionType.Instantaneous);
            joints_f[3].X.SetTargetValue(-1 * axes[3]);
            joints_f[3].X.ProcessMotion(MotionType.Instantaneous);
            joints_f[4].Y.SetTargetValue(-1 * axes[4]);
            joints_f[4].Y.ProcessMotion(MotionType.Instantaneous);
            joints_f[5].X.SetTargetValue(axes[5]);
            joints_f[5].X.ProcessMotion(MotionType.Instantaneous);

            IKSphere_v.transform.SetPositionAndRotation(EE_f.transform.position, EE_f.transform.rotation);
            IKSphere.transform.SetPositionAndRotation(EE_f.transform.position, EE_f.transform.rotation);
        }

        return;
    }


    // --------------------------------------------------------------------------------


    /*
    // Get sliders & offset to joint values for robot
    public String getSliderVals()
    {
        String res = "";
        //Double[] axes = new Double[6];

        float tempVal = 0.0f;
        for (int i = 0; i < 6; i++)
        {
            // Offsets - should be opposite of set
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
            res += tempVal.ToString();
            //axes[i] = (double)tempVal;
            if (i != 5) {
                 res += ",";
            }

            //use to convert
            //Double[] axes = Array.ConvertAll(res.Split(','), Double.Parse);
        }
        //res += ")";

        return res;
    }


    // Set sliders to joint values rcvd from robot
    private void setSliders(string input)
    {
        input = input.Replace('(', ' ').Replace(')', ' ');
        //Debug.Log(input);
        Double[] axes = Array.ConvertAll(input.Split(','), Double.Parse);
        //Debug.LogFormat("Converted Axes: {0}...", axes[0]);

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
    */


    /*
    // Set plot to updated force values
    public ParticleSystem plot;

    [Range(10, 100)]
    public int resolution = 10;

    private int currentResolution;
    private ParticleSystem.Particle[] points;

    private Double[] forceHistory = new Double[10];

    void plotForces(string input)
    {
        input = input.Replace('(', ' ').Replace(')', ' ');
        Double[] forces = Array.ConvertAll(input.Split(','), Double.Parse);


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
            Color c = points[i].startColor;
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
    }*/


}


/*
// Class to store client info & methods
public class client
{
    public string path = null;
    public int port = 20602;
    public Boolean isConnected;

    public InputField inputIP;

    private Socket sender;

    public client()
    {
        path = "169.254.152.40";
        port = 20602;
        isConnected = false;
    }

    public client(string ip, int p)
    {
        path = ip;
        port = p;
        isConnected = false;
    }

    public client(InputField IPin, int p)
    {
        inputIP = IPin;
        port = p;
        isConnected = false;
    }


    public void connect()
    {
        // Data buffer for incoming data.  
        //byte[] bytes = new byte[1024];

        if (path == null)
        {
            path = inputIP.text;
        }

        // Connect to a remote device.  
        try
        {

            // Establish the remote endpoint for the socket.  
            // The example uses port 11000 on the local computer.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            Debug.LogFormat("Connecting from {0}", ipHostInfo.ToString());

            //IPAddress ipAddressLocal = ipHostInfo.AddressList[0];
            // Grab IP from input (ideally sent from text field)
            byte[] ipBytes = Array.ConvertAll(path.Split('.'), byte.Parse);
            IPAddress ipAddressRemote = new IPAddress(ipBytes);

            //path = ipAddressRemote;
            //port = 20602;
            // Grab port from initialization info
            Debug.LogFormat("Connecting to {0}", path.ToString());
            IPEndPoint remoteEP = new IPEndPoint(ipAddressRemote, port);


            // Create a TCP/IP  socket.  
            sender = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.  
            try
            {

                if (isConnected == false)
                {
                    sender.Connect(remoteEP);

                    Debug.LogFormat("Socket connected to {0}", sender.RemoteEndPoint.ToString());

                    if (sender.Connected)
                    {
                        isConnected = true;
                    }
                    else
                    {
                        Debug.LogFormat("Could not connect, retry...");
                    }
                }

                // Encode the data string into a byte array.  
                byte[] msg = Encoding.ASCII.GetBytes("Connected from Unity Server.");

                sendMsg(msg);

                // ---Where to do this?---
                // Release the socket.  
                //sender.Shutdown(SocketShutdown.Both);
                //sender.Close();

            }
            catch (ArgumentNullException ane)
            {
                Debug.LogFormat("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Debug.LogFormat("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Debug.LogFormat("Unexpected exception : {0}", e.ToString());
            }

        }
        catch (Exception e)
        {
            Debug.LogFormat(e.ToString());
        }
    }


    public void disconnect()
    {
        if (isConnected)
        {
            // Release the socket.  
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();

            isConnected = false;
        }

    }


    public string sendMsg(byte[] msg)
    {
        if (isConnected == false)
        {
            Debug.Log(String.Format("Client not connected.  Connecting now."));
            connect();
        }

        // Data buffer for incoming data. 
        byte[] bytes = new byte[1024];

        // Send the data through the socket.  
        int bytesSent = sender.Send(msg);
        if (bytesSent <= 0)
        {
            Debug.LogFormat("No bytes were sent.");
        }

        // Receive the response from the remote device.  
        int bytesRec = sender.Receive(bytes);
        if (bytesRec <= 0)
        {
            Debug.LogFormat("No bytes were received.");
        }
        string msgRec = Encoding.ASCII.GetString(bytes, 0, bytesRec);
        Debug.LogFormat("Echoed test = {0}", msgRec);

        return msgRec;

    }

}
*/
