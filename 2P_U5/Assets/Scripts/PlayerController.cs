using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using Uniduino;
using System.IO;

//// for sending email when session is done
//using System.Net;
//using System.Net.Mail;
//using System.Net.Security;
//using System.Security.Cryptography.X509Certificates;

public class PlayerController : MonoBehaviour
{

    public Arduino arduino;
    private Vector3 initialPosition;
    private static int numRewards = 0;
    private int numRewards_manual = 0;
    private int rewardFlag = 0;
    private int landmarkFlag = 0;
    public int numTraversals = 0;
    private int percentOfTrialsRewardOmitted = 0;
    public int numTrialsTotal;


    // for saving data
    private string localDirectory;
    private string serverDirectory;
    private SessionParams paramsScript;
    private string mouse;
    private string session;
    private bool saveData;

    // lights, for visual landmarks
    public Light light1;
    public Light light2;
    private Color initialBackgroundColor;
    private Color initialAmbientLight;

    private Vector3 movement;

    private GameObject player;
    private GameObject reward;
    private GameObject teleport;
    private GameObject panoCam;
    private GameObject blackCam;
    //private GameObject reward;
    private GameObject[] rewards;

    public bool pcntrl_pins = false;
    private bool move_rewards;
    private Scene curr_scene;
    private AudioSource sound;

    private static bool created = false;

    public void Awake()
    {
        //sound = GameObject.Find("basic_maze").GetComponent<AudioSource>();
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
            //this.sound = GameObject.Find("basic_maze").GetComponent<AudioSource>();
        }
    }

    void Start()
    {
        initialPosition = new Vector3(0f, 0.5f, -5.0f);


        // initialize arduino
        arduino = Arduino.global;
        //arduino.Connect();
        arduino.Setup(ConfigurePins);


        // turn off lights
        panoCam = GameObject.Find("panoCamera");
        blackCam = GameObject.Find("Black Camera");

        StartCoroutine(LightsOff());

        // get session parameters from SessionParams script
        GameObject player = GameObject.Find("Player");
        paramsScript = player.GetComponent<SessionParams>();
        numTrialsTotal = paramsScript.numTrialsTotal;


        // find reward and teleport objects
        rewards = GameObject.FindGameObjectsWithTag("Reward");
            
        teleport = GameObject.Find("Teleport");

        curr_scene = SceneManager.GetActiveScene();
        Debug.Log(curr_scene.name);
        move_rewards = curr_scene.name == "visualFlow" ? true : false;

        //sound = GameObject.Find("basic_maze").GetComponent<AudioSource>();
        // for saving data
        //mouse = paramsScript.mouse;
        //session = paramsScript.session;
        //saveData = paramsScript.saveData;
        //localDirectory = paramsScript.localDirectory;
        //serverDirectory = paramsScript.serverDirectory;
    }

    void ConfigurePins()
    {
        arduino.pinMode(11, PinMode.OUTPUT);
        arduino.pinMode(12, PinMode.OUTPUT);
        Debug.Log("Pins configured (player controller)");
        pcntrl_pins = true;
    }

    void Update()
    {

        // make sure rotation angle is 0
        transform.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            numRewards_manual += 1;
            Debug.Log(numRewards_manual);
            StartCoroutine(Reward());
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            StartCoroutine(Punish());
        }
        // end game after appropriate number of trials
        if (transform.position.z <= 0 & numTraversals == numTrialsTotal)
        {
            //StartCoroutine (sendEmail ()); // send email notification that session is done
            UnityEditor.EditorApplication.isPlaying = false;
        }

    }

    void OnTriggerEnter(Collider other)
    {
        sound = GameObject.Find("basic_maze").GetComponent<AudioSource>();
        Debug.Log(other.tag);
        if (other.tag == "Start" && landmarkFlag == 0)
        {
           // if (!sound.isPlaying)
            //{
                sound.Play();
            //}
        

            //if (!GetComponent<AudioSource>().isPlaying)
            //{
            //    GetComponent<AudioSource>().Play();
            //}
            StartCoroutine(LightsOn());

        }
        else if (other.tag == "Reward")
        {   
            StartCoroutine(Reward());
            numRewards += 1;

            if (move_rewards) {
                movement = new Vector3(0.0f, 0.0f, 10.0f);
                other.gameObject.transform.position= other.gameObject.transform.position + movement;
                teleport.transform.position = teleport.transform.position + movement;
            } else
            {
                other.enabled = false;
            }
                
            //Debug.Log ("Rewards: " + numRewards);

        }
        else if (other.tag == "Teleport")
        {
            numTraversals += 1;
            StartCoroutine(LightsOff());
            sound.Stop();
            transform.position = initialPosition;

            if (move_rewards)
            {
                reward.transform.position = new Vector3(0.0f, 0.0f, 5.0f);
                teleport.transform.position = reward.transform.position + new Vector3(0.0f, 0.0f, 10.0f);
            } else
            {
                // enable all rewards
                StartCoroutine(EnableAllRewards());
                SceneManager.LoadScene("CueRich2"); // for debugging, 
                
            }
            //other.enabled = true;
                
        }

    }

    IEnumerator LightsOff()
    {
        // switch to black out cam
        blackCam.SetActive(true);
        panoCam.SetActive(false);
        yield return null;
    }

    IEnumerator LightsOn()
    {
        // switch to pano cam to active and make sure lights are on
        panoCam.SetActive(true);
        blackCam.SetActive(false);
        //light1.enabled = true;
        //light2.enabled = true;
        RenderSettings.ambientLight = initialAmbientLight;
        yield return null;
    }

    IEnumerator Reward()
    {   // water reward
        arduino.digitalWrite(12, Arduino.HIGH);
        yield return new WaitForSeconds(0.1f);
        arduino.digitalWrite(12, Arduino.LOW);
    }

    IEnumerator Punish()
    {   // air puff punishment
        arduino.digitalWrite(11, Arduino.HIGH);
        yield return new WaitForSeconds(0.1f);
        arduino.digitalWrite(11, Arduino.LOW);
    }

    IEnumerator EnableAllRewards()
    {
        foreach(GameObject item in GameObject.FindGameObjectsWithTag("Reward"))
        {
            Collider collider = item.GetComponent<Collider>();
            collider.enabled = true;
        }
        yield return null; 
    }

    //	IEnumerator sendEmail()
    //	{
    //		MailMessage mail = new MailMessage ();
    //
    //		mail.From = new MailAddress ("giocomo.lab.vr.rig@gmail.com");
    //		mail.To.Add ("malcgcamp@gmail.com");
    //		mail.Subject = "VR session complete";
    //		mail.Body = "";
    //
    //		SmtpClient smtpServer = new SmtpClient ("smtp.gmail.com");
    //		smtpServer.Port = 587;
    //		smtpServer.Credentials = new System.Net.NetworkCredential ("giocomo.lab.vr.rig", "entorhinal!1234") as ICredentialsByHost;
    //		smtpServer.EnableSsl = true;
    //		ServicePointManager.ServerCertificateValidationCallback = 
    //			delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
    //			return true;
    //		};
    //		smtpServer.Send (mail);
    //
    //		yield return null;
    //	}


    // save manipulation data to server
    void OnApplicationQuit()
    {
        if (saveData)
        {
            //if (manipSession)
            //{
            //	File.Copy (manipFile,serverManipFile);
            //}
        }
    }

}
