using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class DL_LEDCue : MonoBehaviour
{


    public string port = "COM3";
    private SerialPort _serialPort;
    private int delay;
    private string lick_raw;


    //public int pinValue;
    private int pinValue;
    public int c_1;

    // for saving data
   
    public int r;
    public int rflag = 1;
    private PC_LEDCue pc;

    private static bool created = false;

    public void Awake()
    {
        // for saving data
        GameObject player = GameObject.Find("Player");
        pc = player.GetComponent<PC_LEDCue>();
    }

    void Start()
    {
        // connect to Arduino uno serial port
        connect(port, 57600, true, 4);
        Debug.Log("Connected to lick detector serial port");

    }


    void Update()
    {
        rflag = 0;

        _serialPort.Write(pc.cmd.ToString() + ',');
        try
        {
            lick_raw = _serialPort.ReadLine();
            string[] lick_list = lick_raw.Split('\t');
            c_1 = int.Parse(lick_list[0]);
            r = int.Parse(lick_list[1]);


        }
        catch (TimeoutException)
        {
            Debug.Log("lickport timeout");
        }


        //Debug.Log(pinValue);
       

        

    }

    void OnApplicationQuit()
    {
        _serialPort.Write("8,");
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
