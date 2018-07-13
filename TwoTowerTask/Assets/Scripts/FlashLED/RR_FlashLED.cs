using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;


public class RR_FlashLED : MonoBehaviour
{
   
    public string port = "COM6";
    private int pulses;
    private SerialPort _serialPort;
    private int delay;
    
    public float true_delta_z;
    private float realSpeed = 0.0447f;
    
   
    public void Awake()
    {
        
    }

    void Start()
    {
        // connect to Arduino uno serial port
        connect(port, 57600, true, 4);
        Debug.Log("Connected to rotary encoder serial port");

       
    }

    void Update()
    {
        
        
        // read quadrature encoder
        _serialPort.Write("\n");
        try
        {
            pulses = int.Parse(_serialPort.ReadLine());
            //Debug.Log (pulses);
            true_delta_z = -1f * pulses * realSpeed;
            

        }
        catch (TimeoutException)
        {
            Debug.Log("rotary timeout");
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
