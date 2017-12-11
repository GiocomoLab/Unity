﻿using UnityEngine;
using System;
using System.Collections;
using Uniduino;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine.SceneManagement;

public class SlideTableCOM_2AFC : MonoBehaviour
{

    public string actPort = "COM5";
    public SerialPort actuatorPort;
    private int delay;

    public int actCmd = 0;
    public bool actFlag = false;
    private string sceneName;
    private GameObject player;
    private PC_2AFC pc;
    private RR_2AFC rr;

    private void Awake()
    {

        player = GameObject.Find("Player");
        pc = player.GetComponent<PC_2AFC>();
        rr = player.GetComponent<RR_2AFC>();


    }

    // Use this for initialization
    void Start()
    {
        connect(actPort, 9600, true, 4); // connect to linear actuator port
        Debug.Log("Connected to actuator serial port");
        actFlag = true;
        //rr.servoBool = 1;
    }

    // Update is called once per frame
    void Update()
    {

        if (pc.cmd == 1 | pc.cmd == 2)
        {
            actuatorPort.Write("1"); // move forward
        }
        else
        {
            actuatorPort.Write("2"); // move port back
        }

    }



    private void connect(string serialPortName, Int32 baudRate, bool autoStart, int delay)
    {
        actuatorPort = new SerialPort(serialPortName, baudRate);

        actuatorPort.DtrEnable = true; // win32 hack to try to get DataReceived event to fire
        actuatorPort.RtsEnable = true;
        actuatorPort.PortName = serialPortName;
        actuatorPort.BaudRate = baudRate;

        actuatorPort.DataBits = 8;
        actuatorPort.Parity = Parity.None;
        actuatorPort.StopBits = StopBits.One;
        actuatorPort.ReadTimeout = 1000; // since on windows we *cannot* have a separate read thread
        actuatorPort.WriteTimeout = 1000;


        if (autoStart)
        {
            this.delay = delay;
            this.Open();
        }
    }

    private void Open()
    {
        actuatorPort.Open();

        if (actuatorPort.IsOpen)
        {
            Thread.Sleep(delay);
        }
    }

    private void Close()
    {
        if (actuatorPort != null)
            actuatorPort.Close();
    }

    private void Disconnect()
    {
        actuatorPort.Write("2");
        Close();
    }

    void OnDestroy()
    {
        actuatorPort.Write("2");
        Disconnect();
    }
}
