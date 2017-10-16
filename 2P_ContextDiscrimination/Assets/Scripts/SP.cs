using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;

public class SP : MonoBehaviour
{

    public bool saveData = true;
    public string mouse;
    public string session;


    // for saving data
    public string localDirectory_pre = "C:/Users/2PRig/VR_Data/2AFC_V2/" ;
    public string serverDirectory_pre = "Z:/VR/2AFC_V2/"  ;
    public string localDirectory;
    public string serverDirectory;
    public string localPrefix;
    public string serverPrefix;
    private string paramsFile;
    private string serverParamsFile;
    public string sceneName;

    public string rewardFile;
    public string serverRewardFile;

    public string lickFile;
    public string serverLickFile;

    public string 

    private static bool created = false;

    public int dirCheck = 0;

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
        sceneName = SceneManager.GetActiveScene().name;

        localDirectory = localDirectory_pre + mouse;
        serverDirectory = serverDirectory_pre +  mouse;

        if (! Directory.Exists(localDirectory))
        {
            Directory.CreateDirectory(localDirectory);
        }
        if (! Directory.Exists(serverDirectory))
        {
            Directory.CreateDirectory(serverDirectory);
        }
        

        localPrefix = localDirectory + "/" + sceneName + "_" + session + "_";
        serverPrefix = serverDirectory + "/" + sceneName + "_" + session + "_";

        

        dirCheck = 1;

    }

    void OnApplicationQuit()
    {
    }
}

