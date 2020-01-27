using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class NodeVisitedTracker : MonoBehaviour
{

    // The dialogue runner that we want to attach the 'visited' function to
    [SerializeField] Yarn.Unity.DialogueRunner dialogueRunner;

    private HashSet<string> _visitedNodes = new HashSet<string>();

    private void Awake()
    {
        LoadSaveData();
    }

    void Start()
    {
        // Register a function on startup called "visited" that lets Yarn
        // scripts query to see if a node has been run before.
        dialogueRunner.RegisterFunction("visited", 1, delegate (Yarn.Value[] parameters)
        {
            var nodeName = parameters[0];
            return _visitedNodes.Contains(nodeName.AsString);
        });

    }

    // Called by the Dialogue Runner to notify us that a node finished
    // running. 
    public void NodeComplete(string nodeName) {
        // Log that the node has been run.
        _visitedNodes.Add(nodeName);
    }

    private string savefile = "yarnvisited.save";


    [Serializable]
    struct YarnVisitedSave
    {

        public HashSet<string> visitedNodes; 
    }

    public void SaveData()
    {
        YarnVisitedSave save = new YarnVisitedSave();
        save.visitedNodes = _visitedNodes;
        BinaryFormatter format = new BinaryFormatter();
        FileStream fs = File.Create(Application.persistentDataPath + savefile);
        //Debug.Log(Application.persistentDataPath + savefile);
        format.Serialize(fs, save);
        fs.Close();
        Debug.Log("Yarn Visited Saved");
    }

    public bool LoadSaveData()
    {
        if (File.Exists(Application.persistentDataPath + savefile))
        {
            BinaryFormatter format = new BinaryFormatter();
            FileStream fs = File.Open(Application.persistentDataPath + savefile, FileMode.Open);
            YarnVisitedSave save = (YarnVisitedSave)format.Deserialize(fs);
            fs.Close();
            _visitedNodes = save.visitedNodes;
            Debug.Log("Yarn Vars loaded");
            return true;
        }
        return false;
    }


    public void DeleteSaveData()
    {
        if (File.Exists(Application.persistentDataPath + savefile))
        {
            File.Delete(Application.persistentDataPath + savefile);
            _visitedNodes.Clear();
        }
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    private void OnDestroy()
    {
        SaveData();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveData();
        }
    }


}
