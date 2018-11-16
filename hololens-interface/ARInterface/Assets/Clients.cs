using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.IO;
using System.Linq;

//using MixedRealityNetworking;


#if !UNITY_EDITOR && UNITY_WSA_10_0
    //using Windows.ApplicationModel.Background;
    using System.Runtime.InteropServices.WindowsRuntime;

    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;
    using System.Threading.Tasks;


#elif UNITY_EDITOR || UNITY_ANDROID
    using System.Net;
    using System.Net.Sockets;
#endif


namespace ARInterface
{
    public class Client : MonoBehaviour
    {
        // Use this for initialization
        void Start() { }

        // Update is called once per frame
        void Update() { }
        

        private string path = null;
        private int port = 20602;
        public InputField inputIP;
        public Boolean isConnected = false;

        private string msgData = "";


        public Client()
        {
            path = "127.0.0.1";

            init();
        }

        public Client(string ip, int p)
        {
            path = ip;
            port = p;

            init();
        }

        public Client(InputField IPin, int p)
        {
            inputIP = IPin;
            port = p;

            init();
        }



        public void connect()
        {
            if (path == null)
            {
                path = inputIP.text;
            }

        #if !UNITY_EDITOR && UNITY_WSA_10_0
            connectUWP();
        #elif UNITY_EDITOR || UNITY_ANDROID
            connectA();
        #endif

        }



        public string sendMsg(string msg)
        {
            if (path == null)
            {
                path = inputIP.text;
            }

        #if !UNITY_EDITOR && UNITY_WSA_10_0
            sendMsgUWP(msg);
            return msgData;
        #elif UNITY_EDITOR || UNITY_ANDROID
            return sendMsgA(msg);
        #endif

        }


#if !UNITY_EDITOR && UNITY_WSA_10_0

        StreamSocket socket = new StreamSocket();


        private void init()
        {
            /*#if !UNITY_EDITOR && UNITY_WSA
                BackgroundExecutionManager.RequestAccessAsync();
            //#endif*/

            socket.Control.NoDelay = false;
        }


        private async Task connectUWP()
        {
            Debug.LogFormat("Connecting to {0}", path.ToString());

            try {
                Windows.Networking.HostName serverHost = new Windows.Networking.HostName(path);
                //socket = new StreamSocket();
                await socket.ConnectAsync(serverHost, port.ToString());
            }

            catch (Exception e) {
                Debug.LogFormat("Exception occurred!  /n" + e.ToString());
                await socket.CancelIOAsync();
                throw;
            }


            isConnected = true;

            Debug.Log(sendMsg("Connected from Unity client."));

        }


        public async Task sendMsgUWP(string msg)
        {
            if (isConnected != true)
            {
                Debug.Log("Client not initialized.  Waiting for connection.");
                connect();
            }

            if (isConnected != true)
            {
                Debug.Log("Connection failed, try again.");
                return;
            }


            DataWriter writer;
            // Create the data writer object backed by the in-memory stream.
            using (writer = new DataWriter(socket.OutputStream))
            {
                // Set the Unicode character encoding for the output stream
                writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                // Specify the byte order of a stream.
                writer.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;

                // Gets the size of UTF-8 string.
                writer.MeasureString(msg);
                // Write a string value to the output stream.
                writer.WriteString(msg);


                // Send the contents of the writer to the backing stream.
                try
                {
                    await writer.StoreAsync();

                    //IBuffer outMsg = Windows.Storage.Streams.Buffer(1024);
                    // write to buffer??
                    //await socket.OutputStream.WriteAsync();
                }

                catch (Exception e)
                {
                    Debug.LogFormat("Exception occurred!  /n" + e.ToString());
                    throw;
                }

                await writer.FlushAsync();
                // In order to prolong the lifetime of the stream, detach it from the DataWriter
                writer.DetachStream();
            }

            // Read message reply

            // Reset message data
            msgData = "";

            // This should wait until done
            msgData = await read();


            /*
            // Wait for reply
            float timeCounter = 0.0f;
            while (msgData == "")
            {
                // Increment timer
                timeCounter += Time.deltaTime;
                if (timeCounter >= 30.0)
                {
                    Debug.Log("No reply in 30s; timed out");
                    return "";
                }
            }*/


            return;

        }

        // From:
        // https://stackoverflow.com/questions/37309851/how-to-connect-to-unity-game-server-socket-from-uwp-app-via-sockets
        private async Task<String> read()
        {
            DataReader reader;
            StringBuilder strBuilder;

            using (reader = new DataReader(socket.InputStream))
            {
                strBuilder = new StringBuilder();

                // Set the DataReader to only wait for available data (so that we don't have to know the data size)
                reader.InputStreamOptions = Windows.Storage.Streams.InputStreamOptions.Partial;
                // The encoding and byte order need to match the settings of the writer we previously used.
                reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                reader.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;

                // Send the contents of the writer to the backing stream. 
                // Get the size of the buffer that has not been read.
                await reader.LoadAsync(256);

                // Keep reading until we consume the complete stream.
                while (reader.UnconsumedBufferLength > 0)
                {
                    strBuilder.Append(reader.ReadString(reader.UnconsumedBufferLength));
                    await reader.LoadAsync(256);
                }

                reader.DetachStream();
                return strBuilder.ToString();
            }
        }



        // Needs code
        public void disconnect()
        {

            isConnected = false;
        }


        //}


#elif UNITY_EDITOR || UNITY_ANDROID

        private Socket sender;


        private void init()
        {
        }


        private void connectA()
        {
            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // The example uses port 11000 on the local computer.  
                //! Obsolete:  IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                Debug.LogFormat("Connecting from {0}", ipHostInfo.ToString());
                //IPAddress ipAddressLocal = ipHostInfo.AddressList[0];
                

                // Grab IP from input
                byte[] ipBytes = Array.ConvertAll(path.Split('.'), byte.Parse);
                IPAddress ipAddressRemote = new IPAddress(ipBytes);
                

                Debug.LogFormat("Connecting to {0}", path.ToString());
                IPEndPoint remoteEP = new IPEndPoint(ipAddressRemote, port);


                // Create a TCP/IP socket
                sender = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint; Catch any errors 
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

                    sendMsg("Connected from Unity client.");

                    // ---Where to do this??---
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


        public string sendMsgA(string msg)
        {
            if (isConnected == false)
            {
                Debug.Log("Client not connected.  Connecting now.");
                connect();
            }

            if (isConnected != true)
            {
                Debug.Log("Connection failed, try again.");
                return "";
            }

            // Convert input to bytes
            byte[] msgBytes = Encoding.ASCII.GetBytes(msg);

            // Data buffer for incoming data. 
            byte[] bytes = new byte[1024];

            // Send the data through the socket.  
            int bytesSent = sender.Send(msgBytes);
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

#endif


    }
}