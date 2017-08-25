using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class ReadRotaryLicks_2AFC : MonoBehaviour {
	
	public float speed = 1;
	private float realSpeed;
	public string port = "COM5";
	private int pulses;
	private SerialPort _serialPort;
	private int delay;
	private SessionParams_2AFC paramsScript;
	private float speedGain;
	private SynchronizeComputers_2AFC syncScript;
	private bool recordingStarted_local = false;
	private PC_2AFC playerScript;
    //private BlankLaser laserControl;

    private string raw;
    private int lick_L;
    private int lick_R;
    private string lickFile;
    private bool savedata = true;
    private string mouse;
    private string session;
    private bool saveData = true;
   
    private string localDirectory;
    private string serverDirectory;

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
		connect (port, 57600, true, 4);
		Debug.Log ("Connected to rotary encoder serial port");

		// set speed
		realSpeed = 0;

		// connect to playerController script
		GameObject player = GameObject.Find ("Player");
		playerScript = player.GetComponent<PC_2AFC> ();
		paramsScript = player.GetComponent<SessionParams_2AFC> ();
        speedGain = 1.0f; //paramsScript.speedGain;

		GameObject gameControl = GameObject.Find ("GameControl");
		syncScript = gameControl.GetComponent<SynchronizeComputers_2AFC> ();

        mouse = paramsScript.mouse;
        session = paramsScript.session;
        //saveData = paramsScript.saveData;
        localDirectory = paramsScript.localDirectory;
        serverDirectory = paramsScript.serverDirectory;
        lickFile = localDirectory + "/" + mouse + "/" + session + "_licks.txt";
    }

	void Update() 
	{
        if (syncScript.recordingStarted & syncScript.sync_pins & playerScript.pcntrl_pins & !recordingStarted_local)
        {
            realSpeed = speedGain * 0.0447f;
            

            recordingStarted_local = true;
        }

        if (playerScript.pause_player)
        {
            realSpeed = 0.0f;

        } else
        {
            realSpeed = speedGain * 0.0447f;
        }


        // read quadrature encoder and move player accordingly
        
		_serialPort.Write("\n");
		//try {
            raw = _serialPort.ReadLine();
            Debug.Log(raw);
            string[] raw_list = raw.Split('\t');
            lick_L = int.Parse(raw_list[0]);
            lick_R = int.Parse(raw_list[1]);
            pulses = int.Parse(raw_list[2]);
            Debug.Log (pulses);
            float delta_z = -1f * pulses * realSpeed;
            Vector3 movement = new Vector3(0.0f, 0.0f, delta_z);
            transform.position = transform.position + movement;


        //} catch(TimeoutException) {
		//    Debug.Log ("rotary timeout");
		//}
        
        //if (saveData)
        //{
        //    var sw = new StreamWriter(lickFile, true);
        //   sw.Write(lick_L + "\t" + lick_R + "\t" + transform.position.z + "\t" + Time.realtimeSinceStartup + "\t" + paramsScript.morph + "\n");
        //  sw.Close();

        //}

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
		Close ();		
	}
	
	void OnDestroy()
	{				
		Disconnect();
	}

}
