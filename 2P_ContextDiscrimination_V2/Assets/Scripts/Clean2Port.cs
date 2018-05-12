using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class Clean2Port : MonoBehaviour {

    public string port = "COM8";
    private SerialPort _serialPort;
    private int delay;
    private int cmd = 9;
    private string tmp;

    // Use this for initialization
    void Start() {
        connect(port, 115200, true, 4);
    }

    // Update is called once per frame
    void Update() {
        cmd = 9;
        //byte[] buffer = new byte[] { Convert.ToByte(cmd) };
        //_serialPort.Write(buffer,0,1);
        _serialPort.Write(cmd.ToString()+',');
        try
        {
            tmp = _serialPort.ReadLine();
        }
        catch (TimeoutException)
        {
            Debug.Log("lickport timeout");
        }
    }

    


    void OnApplicationQuit()
    {
        cmd = 10;
        _serialPort.Write(cmd.ToString() + ',');
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
