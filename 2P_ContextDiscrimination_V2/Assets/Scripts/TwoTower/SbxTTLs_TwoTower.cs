using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class SbxTTLs_TwoTower : MonoBehaviour
{

    private static int localPort;

    // prefs 

    private static string IP = "171.65.17.36";  // define in init
    private static int port = 7000;  // define in init

    // "connection" things
    IPEndPoint remoteEndPoint;
    UdpClient client;


    private PC_TwoTower pc;
    private int numTraversals_local = -1;
    //private int numTraversals;

    private SP_TwoTower sp;
    private string timesyncFile;
    private string serverTimesyncFile;
    public bool scanning = false;

    public float p1_x = 0;
    public float p1_y = 0;
    public float p1_z = 0;

    public float p2_x = 0;
    public float p2_y = 0;
    public float p2_z = 0;

    private float dx;
    private float dy;
    private float dz;

    private static bool created = false;

    public void Awake()
    {
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();

        dx = p1_x - p2_x; dy = p1_y - p2_y; dz = p1_z - p2_z;
    }

    void Start()
    {

        // for saving data
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP_TwoTower>();
        pc = player.GetComponent<PC_TwoTower>();
        Debug.Log(sp.numTraversals);

    }


    // sendData
    private void sendString(string message)
    {
        try
        {
            if (message != "")
            {

                // get UTF8 encoding of string
                byte[] data = Encoding.UTF8.GetBytes(message);

                // send message
                client.Send(data, data.Length, remoteEndPoint);
            }
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N) & !scanning)
        {
            StartCoroutine(set_filenames());
        }

        if (Input.GetKeyDown(KeyCode.S) & !scanning)
        {
            
            StartCoroutine(ScannerStart());
            Debug.Log("start");
        };



        if (Input.GetKeyDown(KeyCode.T) & !scanning)
        {
            StartCoroutine(ScannerToggle());
            Debug.Log("toggle");

        };

        if (numTraversals_local != sp.numTraversals)
        {
            numTraversals_local++;
            if ((dx != 0) | (dy!=0) | (dz!=0))
            {
                if (numTraversals_local > 0)
                {
                    if (numTraversals_local % 2 == 1)
                    {
                        move_laser(dx, dy, dz);
                    }
                    else
                    {
                        move_laser(-dx, -dy, -dz);
                    }
                }
            }

        }

    }

    void OnApplicationQuit()
    {

    }

    IEnumerator ScannerToggle()
    {
        pc.cmd = 8;
        yield return new WaitForSeconds(.01f);
        pc.cmd = 0;
    }

    IEnumerator ScannerStart()
    {

        //start first trial ttl1

        scanning = true; sp.scanning = 1;
        
        yield return new WaitForSeconds(2f);
        pc.cmd = 8;
        yield return new WaitForSeconds(.01f);
        pc.cmd = 0;
        yield return new WaitForSeconds(10f);
        pc.cmd = 9;
        yield return new WaitForSeconds(.01f);
        pc.cmd = 0;
        Debug.Log("Press G to Start!");


    }

    IEnumerator set_filenames()
    {
        DateTime today = DateTime.Today;
        // set base directory
        sendString("D" + "D:/mplitt/" + sp.mouse + "/" + today.ToString("dd_MM_yyyy") + '/');
        yield return new WaitForSeconds(1.5f);
        // set first field/final directory
        sendString("A" + sp.sceneName);
        yield return new WaitForSeconds(1.5f);
        // set second field
        sendString("U" + sp.session.ToString());
        yield return null;


    }

    void move_laser(float dx, float dy, float dz)
    {
        sendString("Px" + dx.ToString());
        sendString("Py" + dy.ToString());
        sendString("Pz" + dz.ToString());

    }

}
