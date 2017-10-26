﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class DetectLicks_2Port : MonoBehaviour
{


    public string port = "COM8";
    private SerialPort _serialPort;
    private int delay;
    private string lick_raw;
    private GameObject player;

    //public int pinValue;
    private int pinValue;
    public int c_1;
    public int c_2;

    // for saving data
    private SP sp;
    private string lickFile;
    private string serverLickFile;

    public int r;
    public int rflag = 1;
    private PC pc;

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

    void Start()
    {
        // connect to Arduino uno serial port
        connect(port, 9600, true, 4);
        Debug.Log("Connected to lick detector serial port");


        // for saving data
        player = GameObject.Find("Player");
        sp = player.GetComponent<SP>();
        pc = player.GetComponent<PC>();
       
    }


    void Update()
    {
        rflag = 0;

        _serialPort.Write(pc.cmd.ToString());
        try
        {
            lick_raw = _serialPort.ReadLine();
            string[] lick_list = lick_raw.Split('\t');
            c_1 = int.Parse(lick_list[0]);
            c_2 = int.Parse(lick_list[1]);
            r = int.Parse(lick_list[2]);
            //rflag = 1;
            //Debug.Log(lick_raw);

        }
        catch (TimeoutException)
        {
            Debug.Log("lickport timeout");
        }


        //Debug.Log(pinValue);
        if (sp.saveData)
        {
            var sw = new StreamWriter(sp.lickFile, true);
            sw.Write(c_1 + "\t" + c_2 + "\t" + player.transform.position.z + "\t" + r + "\t" + Time.realtimeSinceStartup + "\t" + sp.morph + "\r\n");
            sw.Close();

        }

    }

   
    private void connect(string serialPortName, Int32 baudRate, bool autoStart, int delay)
    {
        _serialPort = new SerialPort(serialPortName, baudRate);

        _serialPort.DtrEnable = true; // win32 hack to try to get DataReceived event to fire
        _serialPort.RtsEnable = true;
        _serialPort.PortName = serialPortName;
        _serialPort.BaudRate = baudRate;

        _serialPort.DataBits = 8;
        _serialPort.Parity = Parity.None;
        _serialPort.StopBits = StopBits.One;
        _serialPort.ReadTimeout = 1000; // since on windows we *cannot* have a separate read thread
        _serialPort.WriteTimeout = 1000;


        if (autoStart)
        {
            this.delay = delay;
            this.Open();
        }
    }

    private void Open()
    {
        _serialPort.Open();

        if (_serialPort.IsOpen)
        {
            Thread.Sleep(delay);
        }
    }

    private void Close()
    {
        if (_serialPort != null)
            _serialPort.Close();
    }

    private void Disconnect()
    {
        Close();
    }

    void OnDestroy()
    {
        Disconnect();
    }

}
