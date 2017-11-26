using UnityEngine;
using System;
using System.Collections;
using Uniduino;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine.SceneManagement;

public class SlideTableServoCOM_OneSided : MonoBehaviour
{

    public string actPort = "COM4";
    public SerialPort actuatorPort;
    private int delay;

    public int actCmd = 0;
    public bool actFlag = false;
    private string sceneName;
    private GameObject player;
    private PC_OneSided pc;
    private SP_OneSided sp;
    private float morph = -1;
    private int numTraversalsLocal = -1;


    private void Awake()
    {

        player = GameObject.Find("Player");
        pc = player.GetComponent<PC_OneSided>();
        sp = player.GetComponent<SP_OneSided>();


    }

    // Use this for initialization
    void Start()
    {
        connect(actPort, 9600, true, 4); // connect to linear actuator port
        Debug.Log("Connected to actuator serial port");
        actFlag = true;
    }

    // Update is called once per frame
    void Update()
    {

        
        
        


        if (pc.cmd == 1 | pc.cmd == 2 | pc.cmd == 7)
        {
            actuatorPort.Write("1"); // move forward
        }
        else
        {
            actuatorPort.Write("2"); // move port back
            if (sp.morph == 0f)
            {
                StartCoroutine(roll0());
            }
            else if (sp.morph == 1f)
            {
                StartCoroutine(roll1());

            }
        }

        

    }


    IEnumerator roll0()
    {
        //actuatorPort.Write("2"); // move port back
        yield return new WaitForSeconds(0.75f);
        actuatorPort.Write("3");
        yield return null;

    }

    IEnumerator roll1()
    {
       // actuatorPort.Write("2"); // move port back
        yield return new WaitForSeconds(0.75f);
        actuatorPort.Write("4");
        yield return null;

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
