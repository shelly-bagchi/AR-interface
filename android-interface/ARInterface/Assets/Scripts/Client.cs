using UnityEngine;
using UnityEngine.UI;

// From https://msdn.microsoft.com/en-us/library/kb5kfec7(v=vs.110).aspx
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

// Class to store client info & methods
public class Client : MonoBehaviour {

    public string path = null;
    public int port = 20602;
    public Boolean isConnected;

    public InputField inputIP;

    private Socket sender;

    public Client()
    {
        path = "169.254.152.40";
        port = 20602;
        isConnected = false;
    }

    public Client(string ip, int p)
    {
        path = ip;
        port = p;
        isConnected = false;
    }
    
    public Client(InputField IPin, int p)
    {
        inputIP = IPin;
        port = p;
        isConnected = false;
    }


    public void connect()
    {
        // Data buffer for incoming data.  
        //byte[] bytes = new byte[1024];
        
        //if (path == null) {
        path = inputIP.text;
        //}

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
        Debug.LogFormat("Reply = \"{0}\" ", msgRec);

        return msgRec;

    }

}
