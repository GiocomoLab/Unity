using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using System.Data;
using Mono.Data.Sqlite;

public class SP_RunTrain : MonoBehaviour
{

    
    public string mouse;
    

    public float mrd = 30.0f; // minimum reward distance
    public float ard = 10.0f; // additional reward distance
    public bool fixedRewardSchedule = false; // proportion of trials with towers on both sides
    public float MinTrainingDist = 10f;
    public float MaxTrainingDist = 300f;
    
    public int numRewards = 0;
    public int numRewards_manual = 0;
    public int rewardFlag = 0;

    public int numTraversals = 0;
    public int numTrialsTotal;
    public int maxRewards = 100;

    public float rDur = 2f; // timeout duration between available rewards


    // for saving data
    public string localDirectory_pre = "C:/Users/markp/VR_Data/TwoTower/";
    public string serverDirectory_pre = "Z:/VR/TwoTower/";
    public string localDirectory;
    public string serverDirectory;
    public string localPrefix;
    public string serverPrefix;
    public string sceneName;

    private GameObject player;
    private RR_RunTrain rr;
    private DL_RunTrain dl;
    private PC_RunTrain pc;
    private SbxTTLs_RunTrain ttls;


    public int session;
    private DateTime today;
    private IDbConnection _connection;
    private IDbCommand _command;

    public int scanning = 0;


    public int dirCheck = 0;

    public void Awake()
    {

        player = GameObject.Find("Player");
        rr = player.GetComponent<RR_RunTrain>();
        dl = player.GetComponent<DL_RunTrain>();
        pc = player.GetComponent<PC_RunTrain>();
        ttls = player.GetComponent<SbxTTLs_RunTrain>();

        today = DateTime.Today;
        Debug.Log(today.ToString("dd_MM_yyyy"));
        sceneName = SceneManager.GetActiveScene().name;
        localDirectory = localDirectory_pre + mouse + '/' + today.ToString("dd_MM_yyy") + '/';
        serverDirectory = serverDirectory_pre + mouse + '/' + today.ToString("dd_MM_yyy") + '/';
        if (!Directory.Exists(localDirectory))
        {
            Directory.CreateDirectory(localDirectory);
        }
        if (!Directory.Exists(serverDirectory))
        {
            Directory.CreateDirectory(serverDirectory);
        }



        

        bool nameFlag = true;
        session = 1;
        while (nameFlag)
        {
            localPrefix = localDirectory + "/" + sceneName + "_" + session.ToString();
            serverPrefix = serverDirectory + "/" + sceneName + "_" + session.ToString();
            if (File.Exists(localPrefix + ".sqlite"))
            {
                session++;
            }
            else
            {
                nameFlag = false;
                SqliteConnection.CreateFile(localPrefix + ".sqlite");
            }
        }

        string connectionString = "Data Source=" + localPrefix + ".sqlite;Version=3;";
        _connection = (IDbConnection)new SqliteConnection(connectionString);
        _connection.Open();
        _command = _connection.CreateCommand();
        _command.CommandText = "create table data (time REAL, trialnum INT, pos REAL, dz REAL, lick INT, reward INT," +
        "tstart INT, teleport INT, scanning NUMERIC, manrewards INT)";
        _command.ExecuteNonQuery();
    }

    void LateUpdate()
    {

        _command.CommandText = "insert into data (time , trialnum, pos, dz, lick, reward," +
        "tstart, teleport, scanning, manrewards) values (" + Time.realtimeSinceStartup + ","  + numTraversals +
        "," + transform.position.z + "," + rr.true_delta_z + "," + dl.c_1 + "," + dl.r + "," + pc.tstartFlag + "," + pc.tendFlag + "," +
        ttls.scanning + "," + pc.mRewardFlag + ")";
        _command.ExecuteNonQuery();



    }

    void OnApplicationQuit()
    {
        _command.Dispose();
        _command = null;

        _connection.Close();
        _connection = null;


        string sess_connectionString = "Data Source=Z:\\VR\\TwoTower\\behavior.sqlite;Version=3;";
        IDbConnection db_connection;
        db_connection = (IDbConnection)new SqliteConnection(sess_connectionString);
        db_connection.Open();
        IDbCommand db_command = db_connection.CreateCommand();
        string tmp_date = today.ToString("dd_MM_yyyy");
        db_command.CommandText = "insert into sessions (MouseName, DateFolder, SessionNumber, Track, RewardCount, Imaging) values ('" + mouse + "', '" + tmp_date + "', "
            + session + ",'" + sceneName + "', " + numRewards + ", " + scanning + ")";

        Debug.Log(db_command.CommandText);

        db_command.ExecuteNonQuery();


        db_command.Dispose();
        db_command = null;

        db_connection.Close();
        db_connection = null;


    }
}
