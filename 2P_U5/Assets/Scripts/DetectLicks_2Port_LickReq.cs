using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class DetectLicks_2Port_LickReq : MonoBehaviour {

	
    public string port = "COM5";
    private SerialPort _serialPort;
    private int delay;
    private string lick_raw;

    
    //public int pinValue;
    private int pinValue;
    public int c_1;
    public int c_2;

	// for saving data
	private string localDirectory;
	private string serverDirectory;
	private SessionParams_2AFC paramsScript;
	private string mouse;
	private string session;
	private bool saveData=true;
	private string lickFile;
	private string serverLickFile;

    public int r;
    private PC_2AFC_LickReq pc;

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

    void Start( )
	{
        // connect to Arduino uno serial port
        connect(port, 9600, true, 4);
        Debug.Log("Connected to lick detector serial port");

        
		// for saving data
		GameObject player = GameObject.Find ("Player");
		paramsScript = player.GetComponent<SessionParams_2AFC> ();
        pc = player.GetComponent<PC_2AFC_LickReq>();
		mouse = paramsScript.mouse;
		session = paramsScript.session;
		//saveData = paramsScript.saveData;
		localDirectory = paramsScript.localDirectory;
		serverDirectory = paramsScript.serverDirectory;
		lickFile = localDirectory + "/" + mouse + "/" + session + "_licks.txt";
		serverLickFile = serverDirectory + "/" + mouse + "/" + session + "_licks.txt";
	}

	
	void Update () 
	{

        
        //Debug.Log(pc.cmd.ToString());
        _serialPort.Write(pc.cmd.ToString());
        try
        {
            lick_raw = _serialPort.ReadLine();
            string[] lick_list = lick_raw.Split('\t');
            c_1 = int.Parse(lick_list[0]);
            c_2 = int.Parse(lick_list[1]);
            r = int.Parse(lick_list[2]);
            //Debug.Log(lick_raw);

        }
        catch (TimeoutException)
        {
            Debug.Log("lickport timeout");
        }

        
        //Debug.Log(pinValue);
			if (saveData)
			{
				var sw = new StreamWriter (lickFile, true);
				sw.Write (c_1 + "\t" + c_2 + "\t" + transform.position.z + "\t" + r + "\t" + Time.realtimeSinceStartup + "\t" + paramsScript.morph + "\r\n");
				sw.Close ();
               
			}
		
	}

	void OnApplicationQuit()
	{
		if (saveData) {
			File.Copy (lickFile, serverLickFile);
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
