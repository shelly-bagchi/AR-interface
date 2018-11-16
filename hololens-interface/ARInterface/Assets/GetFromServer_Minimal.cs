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




public class GetFromServer_Minimal : MonoBehaviour {
    private Button thisButton;

    public InputField crpiIP;
    public InputField MessageOutput;
    

    // Create class to hold client details
    private Client crpiClient;


    void Start()
    {
        thisButton = this.GetComponent<Button>();
        thisButton.onClick.AddListener(() => TaskOnClick(thisButton));


        //crpiIP.text = "127.0.0.1";  // Local
        //crpiIP.text = "169.254.80.80";  // Local?

        //crpiIP.text = "169.254.152.40";  // Ethernet
        //crpiIP.text = "169.254.152.39";  // Ethernet - NEW??
        //crpiIP.text = "169.254.152.53";  // Wifi


        crpiIP.text = "169.254.152.10";  // Jeremy's laptop

        crpiClient = new Client(crpiIP, 20602);
    }
    

    void Update()
    {
    }


    public void TaskOnClick(Button ClickedButton) {
        String name = ClickedButton.gameObject.name;
        Debug.LogFormat("Button clicked:  " + name);

        String cmd = "";
        switch (name)
        {
            case "ConnectBtn":
                cmd = "connect";
                break;
            case "PickBtn":
                cmd = "pick";
                break;
            case "PlaceBtn":
                cmd = "place";
                break;
            case "GCOnBtn":
                cmd = "gravcomp_on";
                break;
            case "GCOffBtn":
                cmd = "gravcomp_off";
                break;
            default:
                Debug.LogFormat("Command not recognized, try another.");
                cmd = "DNE";
                break;
        }
        Debug.LogFormat("Set cmd:  " + cmd);

        // Encode the data string into a byte array & send to server.
        string ans = crpiClient.sendMsg(cmd);
        
        // Display output
        MessageOutput.text = String.Format("{0}", ans);


    }


}