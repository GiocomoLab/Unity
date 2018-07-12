using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Threading;




public class PC_MovingWall : MonoBehaviour
{
    

    private GameObject player;
    private GameObject reward;
    private GameObject blackCam;
    private GameObject panoCam;

    private Rigidbody rb;
    private AudioSource sound;

    private SP_MovingWall sp;
    private DL_MovingWall_CapBased dl;
    private RR_MovingWall rotary;

    private bool reward_dir;


    private Vector3 initialPosition;
    private Vector3 movement;

    private int r;
    public int cmd = 0;
    private float LastRewardTime;
    
    public void Awake()
    {
        GameObject player = GameObject.Find("Player");
        sp = player.GetComponent<SP_MovingWall>();
        rotary = player.GetComponent<RR_MovingWall>();
        dl = player.GetComponent<DL_MovingWall_CapBased>();
        blackCam = GameObject.Find("Black Camera");
        panoCam = GameObject.Find("panoCamera");
        panoCam.transform.eulerAngles = new Vector3(0.0f, -90.0f, 0.0f);
        reward = GameObject.Find("Reward");
        initialPosition = new Vector3(0f, 6f, -50.0f);
    }

    void Start()
    {
       
        
        // put the mouse in the dark tunnel
        StartCoroutine(LightsOff());
        reward.transform.position = reward.transform.position + new Vector3(0.0f, 0.0f, sp.mrd ); ;
        LastRewardTime = Time.realtimeSinceStartup;
    }


    void Update()
    {
        // make sure rotation angle is 0
        transform.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);

        // end game after appropriate number of trials
        if (sp.numTraversals >= sp.numTrialsTotal | sp.numRewards>= sp.maxRewards)
        {
            UnityEditor.EditorApplication.isPlaying = false;
            //Application.Quit();
            
        }

        if (dl.r > 0 ) { StartCoroutine(DeliverReward(dl.r)); sp.numRewards++; dl.r = 0; }; // deliver appropriate reward

        // manual rewards and punishments
        if (Input.GetKeyDown(KeyCode.Q) | Input.GetMouseButtonDown(0)) // reward left
        {
            StartCoroutine(DeliverReward(4));

            
            var sw = new StreamWriter(sp.manRewardFile, true);
            sw.Write(Time.realtimeSinceStartup + "\r\n");
            sw.Close();
            
        }

       
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag == "Start")
        {
            StartCoroutine(LightsOn());
        }

        else if (other.tag == "Reward")
        {
            
            var sw = new StreamWriter(sp.rewardFile, true);
            sw.Write(transform.position.z + "\t" + Time.realtimeSinceStartup + "\r\n");
            sw.Close();

            StartCoroutine(RewardSequence());
            StartCoroutine(MoveReward());

        }
        else if (other.tag == "Teleport")
        {
            sp.numTraversals += 1;
            StartCoroutine(LightsOff());
            transform.position = initialPosition;
            reward.transform.position = new Vector3(0.0f, 0f, sp.mrd);
            LastRewardTime = Time.realtimeSinceStartup; // to avoid issues with teleports
        }

    }

    IEnumerator MoveReward()
    {
        float CurrRewardTime = Time.realtimeSinceStartup;

        if (!sp.fixedRewardSchedule)
        {

            if (CurrRewardTime - LastRewardTime > 20.0f)
            {
                sp.mrd = Mathf.Max(sp.MinTrainingDist, sp.mrd - 10f);

            }
            else
            {
                sp.mrd = Mathf.Min(sp.MaxTrainingDist, sp.mrd + 10f);
            }
        }
        reward.transform.position = reward.transform.position + new Vector3(0f, 0f, sp.mrd);
        LastRewardTime = CurrRewardTime;
        yield return null;
    }

    void OnApplicationQuit()
    {
        
        blackCam.SetActive(true);
        panoCam.SetActive(false);

    }

    IEnumerator RewardSequence()
    {   // water reward

        cmd = 7; // turn LED on
        yield return new WaitForSeconds(.5f);
        cmd = 1;
        yield return new WaitForSeconds(sp.rDur);
        cmd = 2;
        yield return new WaitForSeconds(.5f);
        cmd = 0;

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

        yield return null;
    }


    IEnumerator DeliverReward(int r)
    { // deliver
       if (r == 4)
        {
            cmd = 4;
            yield return new WaitForSeconds(0.01f);
            cmd = 0;
        }
    }

}
