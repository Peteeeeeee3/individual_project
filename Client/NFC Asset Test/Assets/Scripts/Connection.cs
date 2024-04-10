using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public static class Connection
{
    private static string serverAddress = "35.233.88.74";
    //private static string serverAddress = "127.0.0.1";
    private static IPAddress serverIP = IPAddress.Parse(serverAddress);
    private static int serverPort = 20111;

    private static TcpClient client;
    private static Thread receivingThread;
    private static Thread sendingThread;
    private static Thread processingThread;
    private static Queue<string> outboundMessageQueue = new Queue<string>();
    private static Queue<string> inboundMessageQueue = new Queue<string>();
    private static Dictionary<string, MonoBehaviour> subscribers = new Dictionary<string, MonoBehaviour>();

    /// <summary>
    /// Connects client side program to the server
    /// </summary>
    public static void Connect(out bool success)
    {
        try
        {
            client = new TcpClient();

            client.Connect(serverIP, serverPort);
            Debug.Log($"Connected to server at {serverAddress}:{serverPort}");

            receivingThread = new Thread(() => ReceiveInboundMessages(client));
            receivingThread.Start();

            processingThread = new Thread(() => ProcessInboundMessages());
            processingThread.Start();

            sendingThread = new Thread(() => SendOutboundMessages());
            sendingThread.Start();

            success = true;
        }
        catch (Exception ex)
        {
            Debug.Log($"An error occurred: {ex.Message}");
            success = false;
        }

        Application.wantsToQuit += CloseConnection;
    }

    /// <summary>
    /// Run as a thread to process incoming messages
    /// </summary>
    /// <param name="client">TCP client used for connection</param>
    static void ReceiveInboundMessages(TcpClient client)
    {
        // Receive data from the server
        byte[] buffer = new byte[1024];
        while (true)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                inboundMessageQueue.Enqueue(receivedMessage);
            }
            catch (Exception ex)
            {
                Debug.Log($"Error in receiving message: {ex.Message}");
                outboundMessageQueue.Enqueue(ex.Message);
            }
        }
    }

    /// <summary>
    /// Run as a thread to precess inbound messages
    /// </summary>
    static void ProcessInboundMessages()
    {
        while (true)
        {
            // skip if no messages in queue
            if (inboundMessageQueue.Count == 0) { continue; }

            string receivedMessage = inboundMessageQueue.Dequeue();
            Connection.QueueMessage("DEBUG\nInbound Message: " + receivedMessage);
            try
            {
                // check for successful message reception
                if (receivedMessage.Length > 0)
                {
                    string[] responseLines = receivedMessage.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                    // user validation
                    if (responseLines[0].Equals("INVALID USER"))
                    {
                        subscribers.TryGetValue("LOGINSCREEN", out MonoBehaviour monoBehaviour);
                        ((LoginScreen)monoBehaviour).OnUserValidated(false, "");
                    }
                    else if (responseLines[0].Equals("VALID USER"))
                    {
                        subscribers.TryGetValue("LOGINSCREEN", out MonoBehaviour monoBehaviour);
                        ((LoginScreen)monoBehaviour).OnUserValidated(true, responseLines[1]);
                    }
                    // get figures
                    else if (responseLines[0].Equals("ERROR FIGURES") ||
                        responseLines[0].Equals("SUCCESS FIGURES"))
                    {
                        subscribers.TryGetValue("MAINMENU", out MonoBehaviour monoBehaviour);
                        ((MainMenu)monoBehaviour).OnAllFiguresReceived(responseLines);
                    }
                    // if figure is not found, then no need to handle as nothing should take place
                    else if (responseLines[0].Equals("FIGURE FOUND"))
                    {
                        int dataOffset = 2;
                        subscribers.TryGetValue(responseLines[1], out MonoBehaviour monoBehaviour);
                        if (responseLines[1].Equals("G_NFCHANDLER"))
                        {
                            GameNFCHandler nfcHandler = (GameNFCHandler)monoBehaviour;
                            nfcHandler.OnUpdateMessanger(responseLines, dataOffset);
                        }
                        else if (responseLines[1].Equals("LEVELMANAGER"))
                        {
                            LevelManager levelManager = (LevelManager)monoBehaviour;
                            levelManager.OnCompleteLevel(responseLines, dataOffset);
                        }
                    }
                    else if (responseLines[0].Equals("FIGURE NOT REGISTERED"))
                    {
                        subscribers.TryGetValue("MAINMENU", out MonoBehaviour monoBehaviour);
                        ((MainMenu)monoBehaviour).OnFigureRegisterFailed();
                    }
                    else if (responseLines[0].Equals("FIGURE ALREADY OWNED"))
                    {
                        subscribers.TryGetValue("MAINMENU", out MonoBehaviour monoBehaviour);
                        ((MainMenu)monoBehaviour).OnFigureAlreadyOwned();
                    }
                    else if (responseLines[0].Equals("FIGURE REGISTERED"))
                    {
                        subscribers.TryGetValue("MAINMENU", out MonoBehaviour monoBehaviour);
                        ((MainMenu)monoBehaviour).OnFigureRegistered();
                    }
                    else if (responseLines[0].Equals("USER REGISTERED"))
                    {
                        subscribers.TryGetValue("LOGINSCREEN", out MonoBehaviour monoBehaviour);
                        ((LoginScreen)monoBehaviour).OnUserRegistered(true);
                    }
                    else if (responseLines[0].Equals("USERNAME IN USE"))
                    {
                        subscribers.TryGetValue("LOGINSCREEN", out MonoBehaviour monoBehaviour);
                        ((LoginScreen)monoBehaviour).OnUserRegistered(false);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                QueueMessage(e.Message);
            }
        }
    }

    /// <summary>
    /// Called to close TCP connection
    /// </summary>
    public static bool CloseConnection()
    {
        try
        {
            receivingThread.Abort();
            sendingThread.Abort();
            client.Close();
            Debug.Log("Connection closed.");
        }
        catch (Exception ex)
        {
            Debug.Log($"Error in closing connection: {ex.Message}");
        }

        return true;
    }

    /// <summary>
    /// Called to queue an  message
    /// </summary>
    /// <param name="message">Message to be added to queue</param>
    public static void QueueMessage(string message)
    {
        outboundMessageQueue.Enqueue(message);
    }

    /// <summary>
    /// Called to send all messages in message queue to server
    /// </summary>
    private static void SendOutboundMessages()
    {
        while (true)
        {
            if (outboundMessageQueue.Count > 0)
            {
                string message = outboundMessageQueue.Dequeue();
                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
        }
    }

    /// <summary>
    /// Allows for a Monobehaviour class to become a subscriber and act as an observer
    /// </summary>
    /// <param name="subscriberName">Used to identify the individual subscriber</param>
    /// <param name="subscriber">Subscriber to be added</param>
    public static void Subscribe(string subscriberName, MonoBehaviour subscriber)
    {
        subscribers.Add(subscriberName, subscriber);
    }

    /// <summary>
    /// Removes subscriber from list
    /// </summary>
    /// <param name="subscriberName">Name of subscriber to remove</param>
    public static void Unsubscribe(string subscriberName)
    {
        subscribers.Remove(subscriberName);
    }
}