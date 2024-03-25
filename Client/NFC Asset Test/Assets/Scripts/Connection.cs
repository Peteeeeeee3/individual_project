using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public static class Connection
{
    private static string serverAddress = "127.0.0.1";
    private static IPAddress serverIP = IPAddress.Parse(serverAddress);
    private static int serverPort = 20111;

    private static TcpClient client;
    private static Thread receivingThread;
    private static Thread sendingThread;
    private static Queue<string> messageQueue = new Queue<string>();
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

            receivingThread = new Thread(() => ReceiveMessages(client));
            receivingThread.Start();

            sendingThread = new Thread(() => SendMessages());
            sendingThread.Start();

            success = true;
        }
        catch (Exception ex)
        {
            Debug.Log($"An error occurred: {ex.Message}");
            success = false;
        }
    }

    /// <summary>
    /// Run as a thread to process incoming messages
    /// </summary>
    /// <param name="client">TCP client used for connection</param>
    static void ReceiveMessages(TcpClient client)
    {
        try
        {
            // Receive data from the server
            byte[] buffer = new byte[1024];
            while (true)
            {
                NetworkStream stream = client.GetStream();
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                // check for successful message reception
                if (receivedMessage.Length > 0)
                {
                    string[] responseLines = receivedMessage.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                    // user validation
                    if (responseLines[0].Equals("INVALID USER"))
                    {
                        subscribers.TryGetValue("LOGINSCREEN", out MonoBehaviour monoBehaviour);
                        LoginScreen loginScreen = (LoginScreen)monoBehaviour;
                        loginScreen.OnUserValidated(false, "");
                    }
                    else if (responseLines[0].Equals("VALID USER"))
                    {
                        subscribers.TryGetValue("LOGINSCREEN", out MonoBehaviour monoBehaviour);
                        LoginScreen loginScreen = (LoginScreen)monoBehaviour;
                        loginScreen.OnUserValidated(true, responseLines[1]);
                    }
                    // get figures
                    else if (responseLines[0].Equals("ERROR FIGURES") ||
                        responseLines[0].Equals("SUCCESS FIGURES"))
                    {
                        subscribers.TryGetValue("MAINMENU", out MonoBehaviour monoBehaviour);
                        MainMenu mainMenu = (MainMenu)monoBehaviour;
                        mainMenu.OnAllFiguresReceived(responseLines);
                    }
                    // if figure is not found, then no need to handle as nothing should take place
                    else if (responseLines[0].Equals("FIGURE FOUND"))
                    {
                        int dataOffset = 2;
                        subscribers.TryGetValue(responseLines[1], out MonoBehaviour monoBehaviour);
                        if (responseLines[1].Equals("NFCHANDLER"))
                        {
                            NFCHandler nfcHandler = (NFCHandler)monoBehaviour;
                            nfcHandler.OnUpdateMessanger(responseLines, dataOffset);
                        }
                        else if (responseLines[1].Equals("LEVELMANAGER"))
                        {
                            LevelManager levelManager = (LevelManager)monoBehaviour;
                            levelManager.OnCompleteLevel(responseLines, dataOffset);
                        }    
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error in receiving message: {ex.Message}");
        }
    }

    /// <summary>
    /// Called to close TCP connection
    /// </summary>
    public static void CloseConnection()
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
    }

    /// <summary>
    /// Called to queue an  message
    /// </summary>
    /// <param name="message">Message to be added to queue</param>
    public static void QueueMessage(string message)
    {
        messageQueue.Enqueue(message);
    }

    /// <summary>
    /// Called to send all messages in message queue to server
    /// </summary>
    private static void SendMessages()
    {
        while (true)
        {
            if (messageQueue.Count > 0)
            {
                string message = messageQueue.Dequeue();
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