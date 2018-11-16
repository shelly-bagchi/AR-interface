using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.IO;
using System.Text;

using HoloToolkit.Unity.SharingWithUNET;

#if !UNITY_EDITOR && UNITY_WSA
using Windows.ApplicationModel.Background;
#endif


/*#if (UNITY_EDITOR || UNITY_ANDROID)
using System.Net;
using System.Net.Sockets;
#endif


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


public class GetFromServer_BACKUP : MonoBehaviour {
    private Button thisButton;

    public InputField crpiIP;
    public Dropdown CommandInput;
    public InputField MessageOutput;


    public GameObject CanvasObj;
    private Slider[] sliderList = new Slider[6];

    // Create class to hold client details
    private OldClient crpiClient;

    public GenericNetworkTransmitter transmitter;

    void Start()
    {
        thisButton = this.GetComponent<Button>();
        //Button btn = yourButton.GetComponent<Button>();
        thisButton.onClick.AddListener(TaskOnClick);

        //GameObject IF = GameObject.Find("Canvas/Scaler/MessageOutput");
        //MessageOutput = IF.GetComponent<InputField>();
        


        initializeSliders();

        //crpiIP.text = "169.254.152.40";  // Ethernet
        crpiIP.text = "169.254.152.53";  // Wifi
        crpiClient = new OldClient(transmitter, crpiIP, 20602);
        //crpiClient = new client(transmitter);
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



    // Create the list of GameObjects that represent each slider in the canvas
    void initializeSliders()
    {
        var CanvasChildren = CanvasObj.GetComponentsInChildren<Slider>();

        for (int i = 0; i < CanvasChildren.Length; i++)
        {
            if (CanvasChildren[i].name == "Slider0")
            {
                sliderList[0] = CanvasChildren[i];
            }
            else if (CanvasChildren[i].name == "Slider1")
            {
                sliderList[1] = CanvasChildren[i];
            }
            else if (CanvasChildren[i].name == "Slider2")
            {
                sliderList[2] = CanvasChildren[i];
            }
            else if (CanvasChildren[i].name == "Slider3")
            {
                sliderList[3] = CanvasChildren[i];
            }
            else if (CanvasChildren[i].name == "Slider4")
            {
                sliderList[4] = CanvasChildren[i];
            }
            else if (CanvasChildren[i].name == "Slider5")
            {
                sliderList[5] = CanvasChildren[i];
            }
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



//#if WINDOWS_UWP
public class OldClient : MonoBehaviour {

    public string path = null;
    public int port = 20602;
    public InputField inputIP;

    public bool isConnected = false;

    private GenericNetworkTransmitter GNT;

    private bool DownloadingData = false;
    private byte[] savedData = null;


    public OldClient(GenericNetworkTransmitter gnt)
    {
        GNT = gnt;

        path = "127.0.0.1";

        init();

    }

    public OldClient(GenericNetworkTransmitter gnt, string ip, int prt)
    {
        GNT = gnt;

        path = ip;
        port = prt;

        init();

    }

    public OldClient(GenericNetworkTransmitter gnt, InputField IPin, int prt)
    {
        GNT = gnt;

        inputIP = IPin;
        port = prt;

        init();

    }

    private void init() {

        GNT.DataReadyEvent += NetworkTransmitter_DataReadyEvent;

#if !UNITY_EDITOR && UNITY_WSA
        BackgroundExecutionManager.RequestAccessAsync();
#endif
    }


    public void connect()
    {
        GNT.SendConnectionPort = port;
        if (path == null)
        {
            path = inputIP.text;
        }
        GNT.SetServerIp(path);

        // Connect...?
        // Happens later?

        isConnected = true;

    }

    public void disconnect() { }
    


    public string sendMsg(string msg)
    {
        if (isConnected != true)
        {
            Debug.Log(String.Format("Client not initialized.  Waiting for connection."));
            connect();
        }


        byte[] msgBytes = Encoding.ASCII.GetBytes(msg);
        int bufferSize = 1024;
        byte[] buffer = new byte[bufferSize];
        msgBytes.CopyTo(buffer, 0);

        GNT.SetData(buffer);
        //GNT.ConfigureAsServer();

        WaitForData();


        return savedData.ToString();

    }


    public void WaitForData()
    {
        bool DownloadingData = GNT.RequestAndGetData();
        if (!DownloadingData)
        {
            Invoke("WaitForData", 0.5f);
        }
    }

    private void NetworkTransmitter_DataReadyEvent(byte[] data)
    {
        Debug.Log("Data has arrived.");
        savedData = data;
        Debug.Log(data.Length);
        DownloadingData = false;
        //gotOne = true;
    }
}


/*
#else
public class client
    {
        public string path = null;
        public int port = 20602;
        public InputField inputIP;

        private bool isConnected = false;


        // -- From https://docs.unity3d.com/Manual/UNetUsingTransport.html --
        private ConnectionConfig config;
        private int myReliableChannelId, myUnreliableChannelId, maxConnections = 1, hostId, connectionId;
        private HostTopology topology;
        private byte error;

        public client()
        {
            path = "127.0.0.1";

            init();
            hostId = NetworkTransport.AddHost(topology, port, path);

        }

        public client(string ip, int prt)
        {
            path = ip;
            port = prt;

            init();
            hostId = NetworkTransport.AddHost(topology, port, path);

        }

        public client(InputField IPin, int prt)
        {
            inputIP = IPin;
            port = prt;

            init();
            hostId = NetworkTransport.AddHost(topology, port);

        }

        private void init()
        {
            // Initializing the Transport Layer with no arguments (default settings)
            NetworkTransport.Init();
            config = new ConnectionConfig();
            myReliableChannelId = config.AddChannel(QosType.Reliable);
            //int myUnreliableChannelId = config.AddChannel(QosType.Unreliable);

            topology = new HostTopology(config, maxConnections);
        }


        private void checkConnection()
        {
            int outHostId;
            int outConnectionId;
            int outChannelId;
            int bufferSize = 1024;
            byte[] buffer = new byte[bufferSize];
            int receiveSize;

            NetworkEventType evnt = NetworkTransport.Receive(out outHostId, out outConnectionId, out outChannelId, buffer, bufferSize, out receiveSize, out error);
            Debug.Log("Data rcvd after connect:  " + buffer.ToString());
            switch (evnt)
            {
                case NetworkEventType.ConnectEvent:
                    if (outHostId == hostId &&
                        outConnectionId == connectionId &&
                        (NetworkError)error == NetworkError.Ok)
                    {
                        Debug.Log("Connected");
                        isConnected = true;
                    }
                    break;
                case NetworkEventType.DisconnectEvent:
                    if (outHostId == hostId &&
                        outConnectionId == connectionId)
                    {
                        Debug.Log("Connected, error:" + error.ToString());  // ???
                        isConnected = false;
                    }
                    break;
            }
        }

        public void connect()
        {
            if (path == null)
            {
                path = inputIP.text;
            }

            // Connect
            connectionId = NetworkTransport.Connect(hostId, path, port, 0, out error);
            //Debug.Log("Connect Error?  " + (NetworkError)error);

            //while (isConnected != true)
                checkConnection();
        }

        public void disconnect()
        {
            NetworkTransport.Disconnect(hostId, connectionId, out error);
            //while (isConnected != false)
                checkConnection();
        }

        public bool IsConnected()
        {
            //return NetworkTransport.IsStarted;
            return isConnected;
        }



        public string sendMsg(string msg)
        {
            if (isConnected != true)
            {
                Debug.Log(String.Format("Client not connected.  Waiting for connection."));
                connect();
            }

            //int recHostId;
            int channelId;
            int bufferSize = 1024;
            byte[] buffer = new byte[bufferSize];

            // Serialize
            //Stream stream = new MemoryStream(buffer);
            //BinaryFormatter formatter = new BinaryFormatter();
            // formatter.Serialize(stream, msg);

            // Convert msg into bytes
            byte[] msgBytes = Encoding.ASCII.GetBytes(msg);



            int dataSize;

            Debug.Log("Send buffer:  " + buffer.ToString());

            NetworkTransport.Send(hostId, connectionId, myReliableChannelId, msgBytes, bufferSize, out error);
            //Debug.Log("Send Error?  " + (NetworkError)error);


            NetworkEventType recData = NetworkTransport.ReceiveFromHost(hostId, out connectionId, out channelId,
                buffer, bufferSize, out dataSize, out error);
            //Debug.Log("Rcv Error?  " + (NetworkError)error);

            switch (recData)
            {
                case NetworkEventType.Nothing:         //1
                    break;
                case NetworkEventType.ConnectEvent:    //2
                    break;
                case NetworkEventType.DataEvent:       //3
                    Debug.Log("Received:  " + buffer);
                    break;
                case NetworkEventType.DisconnectEvent: //4
                    break;
            }


            return buffer.ToString();

        }

    }
#endif*/

