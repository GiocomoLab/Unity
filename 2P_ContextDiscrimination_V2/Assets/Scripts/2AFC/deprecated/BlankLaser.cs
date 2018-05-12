using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


public class BlankLaser : MonoBehaviour
{

    private static int localPort;

    // prefs 
    private string IP;  // define in init
    private int port;  // define in init

    // "connection" things
    IPEndPoint remoteEndPoint;
    UdpClient client;

    // messages to scanbox
    private string L0 = "L0";
    private string L1 = "L1";
    private bool LOn = true;
    private string GO = "G";
    private string STOP = "S";
    public static bool LStart = false;
    //



    private GameObject player;


    private static bool created = false;

    public void Awake()
    {
        if (!created)
        {
            // this is the first instance - make it persist
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
        else
        {
            // this must be a duplicate from a scene reload - DESTROY!
            Destroy(this.gameObject);
        }
    }

    // start from unity3d
    public void Start()
    {
        player = GameObject.Find("Player");

        init();
        //StartCoroutine(StartLaser());

    }

    void Update()
    {
        //if (player.transform.position.z < -10 & LOn)
        //{
        //    sendString(L0);
        //    LOn = false;
        //}
        //else if (player.transform.position.z >= -10 & !LOn)
        //{
        //    sendString(L1);
        //    LOn = true;
        //}

    }

    // init
    public void init()
    {
        // 
        //print("UDPSend.init()");

        // IP and port for NeurolabwarePC
        IP = "171.65.17.36";
        port = 7000;

        // create new IP endpoint
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();

        // status
        print("Sending to " + IP + " : " + port);
        print("Testing: nc -lu " + IP + " : " + port);

    }


    // sendData
    private void sendString(string message)
    {
        try
        {
            if (message != "")
            {

                // get UTF8 encoding of string
                byte[] data = Encoding.UTF8.GetBytes(message);

                // send message
                client.Send(data, data.Length, remoteEndPoint);
            }
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    IEnumerator StartLaser()
    {
        sendString(GO);
        //yield return new WaitForSeconds(1.5f);

        Debug.Log("Laser Start");
        LStart = true;
        yield return null;
    }


    void OnApplicationQuit()
    {
        sendString(STOP);
        LStart = false;
    }
}