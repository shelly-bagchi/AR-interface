
    // ----- HL VERSION -----
#if UNITY_WSA
    public class client
    {
        public string path;
        public int port;

        //private Windows.Networking.Sockets.StreamSocket socket;
        private SharingStage sharingStage;
        //private CustomMessages msgHandler = new CustomMessages();

            // -- From HLTK.Sharing.Tests.CustomMessage.cs ---

        /// <summary>
        /// Helper object that we use to route incoming message callbacks to the member
        /// functions of this class
        /// </summary>
        private NetworkConnectionAdapter connectionAdapter;

        /// <summary>
        /// Cache the connection object for the sharing service
        /// </summary>
        private NetworkConnection serverConnection;

        // String to hold recieved message
        private string inMsg;


        public client()
        {
            sharingStage = new SharingStage();
        }

        public client(string ip, int p)
        {
            sharingStage = new SharingStage();
            path = ip;
            port = p;

            inMsg = "";

            //Create the StreamSocket and establish a connection to the echo server.
            //socket = new Windows.Networking.Sockets.StreamSocket();
        }


        /// <summary>
        /// Message enum containing our information bytes to share.
        /// The first message type has to start with UserMessageIDStart
        /// so as not to conflict with HoloToolkit internal messages.
        /// </summary>
        public enum TestMessageID : byte
        {
            HeadTransform = MessageID.UserMessageIDStart,
            Max
        }

        /*public delegate void MessageCallback(NetworkInMessage msg);
        private Dictionary<TestMessageID, MessageCallback> messageHandlers = 
            new Dictionary<TestMessageID, MessageCallback>();
        public Dictionary<TestMessageID, MessageCallback> MessageHandlers
        {
            get
            {
                return messageHandlers;
            }
        }*/


        public void connect()
        {
            sharingStage.ConnectToServer(path, port);
            InitializeMessageHandlers();
        }
        public bool isConnected()
        {
            return sharingStage.IsConnected;
        }

        private void InitializeMessageHandlers()
        {

            serverConnection = sharingStage.Manager.GetServerConnection();
            if (serverConnection == null)
            {
                Debug.Log("Cannot initialize CustomMessages. Cannot get a server connection.");
                return;
            }

            connectionAdapter = new NetworkConnectionAdapter();
            connectionAdapter.MessageReceivedCallback += OnMessageReceived;

            // Cache the local user ID
            //LocalUserID = SharingStage.Instance.Manager.GetLocalUser().GetID();

            for (byte index = (byte)TestMessageID.HeadTransform; index < (byte)TestMessageID.Max; index++)
            {
                /*if (MessageHandlers.ContainsKey((TestMessageID)index) == false)
                {
                    MessageHandlers.Add((TestMessageID)index, null);
                }*/

                serverConnection.AddListener(index, connectionAdapter);
            }
        }


        /*
        public void connect()
        {
            try
            {
                //The server hostname that we will be establishing a connection to. We will be running the server and client locally,
                //so we will use localhost as the hostname.
                Windows.Networking.HostName serverHost = new Windows.Networking.HostName(path);

                //Every protocol typically has a standard port number. For example HTTP is typically 80, FTP is 20 and 21, etc.
                //For the echo server/client application we will use a random port 1337.
                //string serverPort = "20602";
                socket.ConnectAsync(serverHost, port.ToString());

                //Write data to the echo server.
                Stream streamOut = socket.OutputStream.AsStreamForWrite();
                StreamWriter writer = new StreamWriter(streamOut);
                string request = "test";
                writer.WriteLineAsync(request);
                writer.FlushAsync();

                //Read data from the echo server.
                Stream streamIn = socket.InputStream.AsStreamForRead();
                StreamReader reader = new StreamReader(streamIn);
                string response = reader.ReadLineAsync();
            }
            catch (Exception e)
            {
                Debug.LogFormat("Exception : {0}", e.ToString());
            }
        }*/


        public string sendMsg(string msg)
        {
            if (serverConnection == null || serverConnection.IsConnected() == false)
            {
                Debug.Log(String.Format("Client not connected.  Connecting now."));
                connect();
            }
            
            // Convert msg into bytes
            byte[] msgBytes = Encoding.ASCII.GetBytes(msg);



            // If we are connected to a session, broadcast our info
            // -- do we need this in addition to checking SS? --
            //if (serverConnection != null && serverConnection.IsConnected())

            // Create an outgoing network message to contain all the info we want to send
            NetworkOutMessage outMsg = serverConnection.CreateMessage(msgBytes[0]);
            outMsg.WriteArray(msgBytes, (uint)(msgBytes.Length - 1));
            /*for (int i = 1; i < msgBytes.Length; i++)
            {
                outMsg.Write(msgBytes[i]);
            }*/


            // Send the message as a broadcast, 
            //which will cause the server to forward it to all other users in the session.
            serverConnection.Broadcast(
                outMsg,
                MessagePriority.Immediate,
                MessageReliability.UnreliableSequenced,
                MessageChannel.Avatar);


            // Wait until reply is received
            while (inMsg == "") ;
            string temp = inMsg;
            inMsg = "";
            return temp;
            
            

            /*
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
            
            return msgRec;*/

        }

        private void OnMessageReceived(NetworkConnection connection, NetworkInMessage msg)
        {
            /*byte messageType = msg.ReadByte();
            MessageCallback messageHandler = MessageHandlers[(TestMessageID)messageType];
            if (messageHandler != null)
            {
                messageHandler(msg);
            }*/

            inMsg = msg.ReadString();
        }

    }
#else
        // ----- UNITY VERSION -----
//#if (UNITY_EDITOR || UNITY_ANDROID)
    // Class to store client info & methods
    public class client
    {
        public IPAddress path;
        public int port;
        public Boolean isConnected;

        private Socket sender;

        public client()
        {
            isConnected = false;
        }

        public client(IPAddress ip, int p)
        {
            path = ip;
            port = p;
            isConnected = false;
        }


        public void connect()
        {
            // Data buffer for incoming data.  
            //byte[] bytes = new byte[1024];

            // Connect to a remote device.  
            try
            {

                // Establish the remote endpoint for the socket.  
                // The example uses port 11000 on the local computer.  
                //! Obsolete:  IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                Debug.LogFormat("Connecting from {0}", ipHostInfo.ToString());
                //IPAddress ipAddressLocal = ipHostInfo.AddressList[0];
                // Manually set IP -- fix this??
                byte[] ipBytes = { 169, 254, 152, 39 };  //Ethernet
                //byte[] ipBytes = { 169, 254, 152, 52 };  //Wifi
                IPAddress ipAddressRemote = new IPAddress(ipBytes);

                path = ipAddressRemote;
                port = 20602;

                Debug.LogFormat("Connecting to {0}", path.ToString());
                IPEndPoint remoteEP = new IPEndPoint(path, port);


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


        public string sendMsg(string msgString)
        {
            if (isConnected == false)
            {
                Debug.Log(String.Format("Client not connected.  Connecting now."));
                connect();
            }
            
            // Convert msg into bytes
            byte[] msg = Encoding.ASCII.GetBytes(msgString);

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
#endif