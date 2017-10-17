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

    public string posFile;
    public string serverPosFile;

    private NameCheck nc;

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

        nc = GameObject(this).GetComponent<NameCheck>();
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

        rewardFile = nc.Recurse(localPrefix + "_Rewards") + ".txt";
        serverRewardFile = nc.Recurse(serverPrefix + "_Rewards") + ".txt";

        rf = new StreamWriter(rewardFile,true);
        rf.Write(); rf.Close();

        lickFile = localPrefix + "_Licks.txt";
        serverLickFile = serverPrefix + "_Licks.txt";
        lf = new StreamWriter(lickFile,true);
        lf.Write(); lf.Close();

        posFile = localPrefix + "_Pos.txt";
        serverPosFile = serverPrefix + "_Pos.txt";
        pf = new StreamWriter(posFile,true);
        pf.Write(); pf.Close();

        //dirCheck = 1;

    }

    //void Start()
    //{   // to debug pasted all contents to awake (beginning line 54)

    //}

    void OnApplicationQuit() {
        if (saveData) {
            File.Copy(rewardFile, serverRewardFile,true);
            File.Copy(lickFile, serverLickFile,true);
            File.Copy(posFile, serverPosFile,true);
          }
    }

}
