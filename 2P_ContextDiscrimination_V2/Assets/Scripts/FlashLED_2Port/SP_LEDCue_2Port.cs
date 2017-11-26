using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;

public class SP_LEDCue_2Port : MonoBehaviour
{

    public bool saveData = true;
    public string mouse;
    public string session;

    public int numRewards = 0;
    public int numRewards_manual = 0;
    public int rewardFlag = 0;

    public float rDur = 1.5f; // timeout duration between available rewards


    // for saving data
    public string localDirectory_pre = "C:/Users/2PRig/VR_Data/2AFC_V3/";
    public string serverDirectory_pre = "Z:/VR/2AFC_V3/";
    public string localDirectory;
    public string serverDirectory;
    public string localPrefix;
    public string serverPrefix;
    private string paramsFile;
    private string serverParamsFile;
    public string sceneName;

    public string rewardFile;
    public string serverRewardFile;

    public string manRewardFile;
    public string serverManRewardFile;

    public string lickFile;
    public string serverLickFile;

    public string timeSyncFile;
    public string serverTimeSyncFile;

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

        nc = GetComponent<NameCheck>();
        sceneName = SceneManager.GetActiveScene().name;

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


        localPrefix = localDirectory + "/" + sceneName + "_" + session + "_";
        serverPrefix = serverDirectory + "/" + sceneName + "_" + session + "_";

        rewardFile = nc.Recurse(localPrefix + "_Rewards") + ".txt";
        serverRewardFile = nc.Recurse(serverPrefix + "_Rewards") + ".txt";
        var rf = new StreamWriter(rewardFile, true);
        rf.Write(""); rf.Close();


        lickFile = nc.Recurse(localPrefix + "_Licks") + ".txt";
        serverLickFile = nc.Recurse(serverPrefix + "_Licks") + ".txt";
        var lf = new StreamWriter(lickFile, true);
        lf.Write(""); lf.Close();


        manRewardFile = nc.Recurse(localPrefix + "_ManRewards") + ".txt";
        serverManRewardFile = nc.Recurse(serverPrefix + "ManRewards") + ".txt";
        var mrf = new StreamWriter(manRewardFile, true);
        mrf.Write(""); mrf.Close();


        timeSyncFile = nc.Recurse(localPrefix + "_TimeSync") + ".txt";
        serverTimeSyncFile = nc.Recurse(serverPrefix + "_TimeSync") + ".txt";
        var tsf = new StreamWriter(timeSyncFile, true);
        tsf.Write(""); tsf.Close();


    }


    void OnApplicationQuit()
    {
        if (saveData)
        {
            File.Copy(rewardFile, serverRewardFile, true);
            File.Copy(lickFile, serverLickFile, true);
            File.Copy(manRewardFile, serverManRewardFile, true);
            File.Copy(timeSyncFile, serverTimeSyncFile, true);
        }
    }

}
