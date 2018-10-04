using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using System.Data;
using Mono.Data.Sqlite;

public class SP_LEDCue : MonoBehaviour
{


    public string mouse;

    public int numRewards = 0;
    public int numRewards_manual = 0;
    public int rewardFlag = 0;
   
    public float rDur = 2f; // timeout duration between available rewards


    // for saving data
    public string localDirectory_pre = "C:/Users/markp/VR_Data/TwoTower/";
    public string serverDirectory_pre = "G:/My Drive/VR_Data/TwoTower/";
    private string localDirectory;
    private string serverDirectory;
    private string localPrefix;
    private string serverPrefix;
    public string sceneName;

    private GameObject player;
    private RR_FlashLED rr;
    private DL_LEDCue dl;
    private PC_LEDCue pc;

    public int session;
    private DateTime today;
    private IDbConnection _connection;
    private IDbCommand _command;

    public int scanning = 0;


    public int dirCheck = 0;

    public void Awake()
    {

        player = GameObject.Find("Player");
        rr = player.GetComponent<RR_FlashLED>();
        dl = player.GetComponent<DL_LEDCue>();
        pc = player.GetComponent<PC_LEDCue>();
        

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
        _command.CommandText = "create table data (time REAL, dz REAL, lick INT, reward INT, manrewards INT)";
        _command.ExecuteNonQuery();
    }

    void LateUpdate()
    {

        _command.CommandText = "insert into data (time , dz, lick, reward, manrewards) values (" + Time.realtimeSinceStartup + "," + 
            rr.true_delta_z + "," + dl.c_1 + "," + dl.r + "," +  pc.mRewardFlag + ")";
        _command.ExecuteNonQuery();



    }

    void OnApplicationQuit()
    {
        _command.Dispose();
        _command = null;

        _connection.Close();
        _connection = null;

        File.Copy(localPrefix + ".sqlite", serverPrefix + ".sqlite", true);
        
        string sess_connectionString = "Data Source=G:\\My Drive\\VR_Data\\TwoTower\\behavior.sqlite;Version=3;";
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