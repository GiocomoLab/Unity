using UnityEngine;
using System.Collections;
using System.IO;

public class SessionParams_2AFC : MonoBehaviour {

	public bool saveData = true;
	public string mouse;
	public string session;
	private float speedVR;
	private ReadRotary_2AFC rotaryScript;

	// for saving data
	public string localDirectory = "C:/Users/2PRig/VR_Data";
	public string serverDirectory = "Z:/VR";
	private string paramsFile;
	private string serverParamsFile;

    // more session params
    public float morph = 1;
	public int numTrialsTotal = 100;
//	public bool manipSession = false;
//	public bool cueRemovalSession = false;
//	public int numTrialsA = 15;
//	public int numTrialsB = 10;
	public float speedGain = 1.0f;

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

    void Start () 
	{

		GameObject player = GameObject.Find ("Player");
		rotaryScript = player.GetComponent<ReadRotary_2AFC> ();
		speedVR = rotaryScript.speed;
		paramsFile = localDirectory + "/" + mouse + "/" + session + "_params.txt";
		serverParamsFile = serverDirectory + "/" + mouse + "/" + session + "_params.txt";

		if (saveData) 
		{
			var sw = new StreamWriter (paramsFile, true);
			sw.WriteLine("3600");
			sw.WriteLine(speedVR);
			sw.WriteLine("finite7");
//			if (manipSession)
//			{
//				sw.WriteLine ("block");
//			} else {
				sw.WriteLine ("normal");
//			}
			sw.Close ();
		}
	}

	void OnApplicationQuit() 
	{
		if (saveData) 
		{
			File.Copy (paramsFile, serverParamsFile);
		}
	}
}

