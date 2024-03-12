using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public static class Connection
{
    private static string serverAddress = "127.0.0.1";
    private static int serverPort = 20111;

    private static TcpClient client;
    private static Thread receivingThread;
    private static Thread sendingThread;
    private static List<string> messageQueue = new List<string>();
    private static Dictionary<string, MonoBehaviour> subscribers = new Dictionary<string, MonoBehaviour>();

    /// <summary>
    /// Connects client side program to the server
    /// </summary>
    public static void Connect(out bool success)
    {
        try
        {
            client = new TcpClient();

            client.Connect(serverAddress, serverPort);
            Console.WriteLine($"Connected to server at {serverAddress}:{serverPort}");

            receivingThread = new Thread(() => ReceiveMessages(client));
            receivingThread.Start();

            sendingThread = new Thread(() => SendMessages());
            sendingThread.Start();

            success = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
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
                    string[] responseLines = receivedMessage.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                    // user validation
                    if (responseLines[0].Equals("INVALID USER"))
                    {
                        subscribers.TryGetValue("LOGINSCREEN", out var loginscreen);
                        loginscreen.GetComponent<LoginScreen>().OnUserValidated(false, "");
                    }
                    else if (responseLines[0].Equals("VALID USER"))
                    {
                        subscribers.TryGetValue("LOGINSCREEN", out var loginscreen);
                        loginscreen.GetComponent<LoginScreen>().OnUserValidated(true, responseLines[1]);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in receiving message: {ex.Message}");
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
            Console.WriteLine("Connection closed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in closing connection: {ex.Message}");
        }
    }

    /// <summary>
    /// Called to queue an  message
    /// </summary>
    /// <param name="message">Message to be added to queue</param>
    public static void QueueMessage(string message)
    {
        messageQueue.Add(message);
    }

    /// <summary>
    /// Called to send all messages in message queue to server
    /// </summary>
    private static void SendMessages()
    {
        while (true)
        {
            Console.WriteLine("sending message");
            foreach (string message in messageQueue)
            {
                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(message);
                Console.WriteLine(data);
                stream.Write(data, 0, data.Length);
            }

            // clear message queue once messages are sent
            messageQueue.Clear();
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
}