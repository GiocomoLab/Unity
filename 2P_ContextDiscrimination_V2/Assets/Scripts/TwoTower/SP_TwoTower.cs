using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;

public class SP_TwoTower : MonoBehaviour
{

    //public bool saveData = true;
    public string mouse;
    public string session;

    public bool ClickOn = true;

    public float mrd = 175.0f; // minimum reward distance
    public float ard = 150.0f; // additional reward distance
    

    public int numRewards = 0;
    public int numRewards_manual = 0;
    public int rewardFlag = 0;

    public int numTraversals = 0;
    public int numTrialsTotal;
    public int maxRewards = 200;


    public float morph = 0;
    public float rDur = 2;

    // for saving data
    public string localDirectory_pre = "C:/Users/2PRig/VR_Data/TwoTower/";
    public string serverDirectory_pre = "Z:/VR/TwoTower/";
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

    public string posFile;
    public string serverPosFile;

    public string timeSyncFile;
    public string serverTimeSyncFile;

    public string trialOrderFile;
    public string serverTrialOrderFile;

    public string TOFile;
    public string serverTOFile;

    public string TStartFile;
    public string serverTStartFile;

    public string TEndFile;
    public string serverTEndFile;

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


        posFile = nc.Recurse(localPrefix + "_Pos") + ".txt";
        serverPosFile = nc.Recurse(serverPrefix + "_Pos") + ".txt";
        var pf = new StreamWriter(posFile, true);
        pf.Write(""); pf.Close();


        manRewardFile = nc.Recurse(localPrefix + "_ManRewards") + ".txt";
        serverManRewardFile = nc.Recurse(serverPrefix + "ManRewards") + ".txt";
        var mrf = new StreamWriter(manRewardFile, true);
        mrf.Write(""); mrf.Close();


        timeSyncFile = nc.Recurse(localPrefix + "_TimeSync") + ".txt";
        serverTimeSyncFile = nc.Recurse(serverPrefix + "_TimeSync") + ".txt";
        var tsf = new StreamWriter(timeSyncFile, true);
        tsf.Write(""); tsf.Close();

        trialOrderFile = nc.Recurse(localPrefix + "_TrialOrder") + ".txt";
        serverTrialOrderFile = nc.Recurse(serverPrefix + "_TrialOrder") + ".txt";
        var tof = new StreamWriter(trialOrderFile, true);
        tof.Write(""); tof.Close();

        TOFile = nc.Recurse(localPrefix + "_Timeout") + ".txt";
        serverTOFile = nc.Recurse(serverPrefix + "_Timeout") + ".txt";
        var TOF = new StreamWriter(TOFile, true);
        TOF.Write(""); TOF.Close();

        TStartFile = nc.Recurse(localPrefix + "_TStart") + ".txt";
        serverTStartFile = nc.Recurse(serverPrefix + "_TStart") + ".txt";
        var tstart = new StreamWriter(TStartFile, true);
        tstart.Write(""); tstart.Close();

        TEndFile = nc.Recurse(localPrefix + "_TEnd") + ".txt";
        serverTEndFile = nc.Recurse(serverPrefix + "_TEnd") + ".txt";
        var tend = new StreamWriter(TEndFile, true);
        tend.Write(""); tend.Close();

    }

    void OnApplicationQuit()
    {
     
        File.Copy(rewardFile, serverRewardFile, true);
        File.Copy(lickFile, serverLickFile, true);
        File.Copy(posFile, serverPosFile, true);
        File.Copy(manRewardFile, serverManRewardFile, true);
        File.Copy(timeSyncFile, serverTimeSyncFile, true);
        File.Copy(trialOrderFile, serverTrialOrderFile, true);
        File.Copy(TOFile, serverTOFile, true);
        File.Copy(TStartFile, serverTStartFile, true);
        File.Copy(TEndFile, serverTEndFile, true);
     
    }

}
