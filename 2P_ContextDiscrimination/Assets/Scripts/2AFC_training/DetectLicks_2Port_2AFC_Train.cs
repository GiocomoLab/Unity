using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class DetectLicks_2Port_2AFC_Train : MonoBehaviour
{


    public string port = "COM8";
    private SerialPort _serialPort;
    private int delay;
    private string lick_raw;


    //public int pinValue;
    private int pinValue;
    public int c_1;
    public int c_2;

    // for saving data
    private SP_2AFC_Train paramsScript;
    private string lickFile;
    private string serverLickFile;

    public int r;
    public int rflag = 1;
    private PC_2AFC_Train pc;

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
        GameObject player = GameObject.Find("Player");
        paramsScript = player.GetComponent<SP_2AFC_Train>();
        pc = player.GetComponent<PC_2AFC_Train>();
        if (paramsScript.saveData)
        {
            string lickFile_pre = paramsScript.localDirectory + "/" + paramsScript.session + "_2AFC_Train_licks.txt";
            string serverLickFile_pre = paramsScript.serverDirectory + "/" + paramsScript.session + "_2AFC_Train_licks.txt";
            while (paramsScript.dirCheck < 1) { }
            // check if file exists
            if (File.Exists(lickFile_pre + ".txt"))
            {
                lickFile = lickFile_pre + "_copy.txt";
                serverLickFile = serverLickFile_pre + "_copy.txt";
                var sw = new StreamWriter(lickFile, true);
                sw.Close();
            }
            else
            {
                lickFile = lickFile_pre + ".txt";
                serverLickFile = serverLickFile_pre + ".txt";
                var sw = new StreamWriter(lickFile, true);
                sw.Close();
            }
        }
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
        if (paramsScript.saveData)
        {
            var sw = new StreamWriter(lickFile, true);
            sw.Write(c_1 + "\t" + c_2 + "\t" + -1f + "\t" + r + "\t" + Time.realtimeSinceStartup + "\t" + -1f + "\r\n");
            sw.Close();

        }

    }

    void OnApplicationQuit()
    {
        if (paramsScript.saveData)
        {
            File.Copy(lickFile, serverLickFile);
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
