using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;


public class RR_MovingWall : MonoBehaviour
{
   
    public string port = "COM6";
    private int pulses;
    private SerialPort _serialPort;
    private int delay;
    private SP_MovingWall sp;
    private PC_MovingWall pc;
    public float delta_z;
    public float true_delta_z;
    private float realSpeed = 0.0447f;
    public float speedBool = 0;
    public float startBool = 0;
    private bool firstFlag = true;

    private static bool created = false;
    public void Awake()
    {
        if (!created)
        {
            // this is the first instance - make it persist
            DontDestroyOnLoad(this);
            created = true;
        }
        else
        {
            // this must be a duplicate from a scene reload - DESTROY!
            Destroy(this);
        }
    }

    void Start()
    {
        // connect to Arduino uno serial port
        connect(port, 57600, true, 4);
        Debug.Log("Connected to rotary encoder serial port");

        // set speed
        speedBool = 0;

        // connect to playerController script
        GameObject player = GameObject.Find("Player");
        pc = player.GetComponent<PC_MovingWall>();
        sp = player.GetComponent<SP_MovingWall>();
    }

    void Update()
    {
        
        if (firstFlag){ speedBool = 1; firstFlag = false;}
        if (Input.GetKeyDown(KeyCode.G)) { startBool = 1; };
        // read quadrature encoder
        _serialPort.Write("\n");
        try
        {
            pulses = int.Parse(_serialPort.ReadLine());
            //Debug.Log (pulses);
            true_delta_z = -1f * pulses * realSpeed;
            delta_z = -1f * startBool * speedBool * pulses * realSpeed;
            Vector3 movement = new Vector3(0.0f, 0.0f, delta_z);
            transform.position = transform.position + movement;

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
