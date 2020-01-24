using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Yarn.Unity;

public class PlayerManager : MonoBehaviour
{

    public int trustDonna;
    private string savefile = "player_stats.save";


    void Awake()
    {
        LoadStats();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void increaseTrustDonna(int increaseBy)
    {
        trustDonna += Mathf.Abs(increaseBy);
    }
    
    public void decreaseTrustDonna(int decreaseBy)
    {
        trustDonna -= Mathf.Abs(decreaseBy);
        if (trustDonna < 0) trustDonna = 0;
    }

    [YarnCommand("IncreaseTrust")]
    public void increaseTrustDonna(string increaseBy)
    {
        trustDonna += Mathf.Abs(int.Parse(increaseBy));
    }

    [YarnCommand("DecreaseTrust")]
    public void decreaseTrustDonna(string decreaseBy)
    {
        trustDonna -= Mathf.Abs(int.Parse(decreaseBy));
        if (trustDonna < 0) trustDonna = 0;
    }

    public int getTrustDonna() { return trustDonna; }

    public void ResetSaveStats()
    {
        DeleteSaveData();
        trustDonna = 0;
    }

    public void SaveStats()
    {
        PlayerStats save = new PlayerStats();
        save.trustDonna = trustDonna;
        BinaryFormatter format = new BinaryFormatter();
        FileStream fs = File.Create(Application.persistentDataPath + savefile);
        format.Serialize(fs, save);
        fs.Close();
        Debug.Log("Player Stats Saved");
    }

    public bool LoadStats()
    {
        if (File.Exists(Application.persistentDataPath + savefile))
        {
            BinaryFormatter format = new BinaryFormatter();
            FileStream fs = File.Open(Application.persistentDataPath + savefile, FileMode.Open);
            PlayerStats save = (PlayerStats) format.Deserialize(fs);
            fs.Close();
            trustDonna = save.trustDonna;
            Debug.Log("Player stats loaded");
            return true;
        }
        return false;
    }

    public void DeleteSaveData()
    {
        if (File.Exists(Application.persistentDataPath + savefile))
        {
            File.Delete(Application.persistentDataPath + savefile);
        }
    }

    private void OnApplicationPause(bool pause)
    {
        SaveStats();
    }

    private void OnApplicationQuit()
    {
        SaveStats();
    }

    private void OnDestroy()
    {
        SaveStats();
    }


}
