using UnityEngine;
using System.Collections;
using System.IO;

public class SP_2AFC_Train : MonoBehaviour
{

    public bool saveData = true;
    public string mouse;
    public string session;


    // for saving data
    public string localDirectory_pre = "C:/Users/2PRig/VR_Data/2AFC_V2/";
    public string serverDirectory_pre = "Z:/VR/2AFC_V2/";
    public string localDirectory;
    public string serverDirectory;
    private string paramsFile;
    private string serverParamsFile;

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
        localDirectory = localDirectory_pre + mouse;
        serverDirectory = serverDirectory_pre + mouse;

        if (!Directory.Exists(localDirectory))
        {
            Directory.CreateDirectory(localDirectory);
        }
        if (!Directory.Exists(serverDirectory))
        {
            Directory.CreateDirectory(serverDirectory);
        }

        dirCheck = 1;
    }

    void OnApplicationQuit()
    {
    }
}


