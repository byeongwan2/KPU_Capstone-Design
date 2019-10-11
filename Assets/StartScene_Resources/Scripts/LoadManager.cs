using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class LoadManager : MonoBehaviour
{
    public static int stageLevel = 0;

    [Serializable]
    public class LoadData
    {
        public int stageLevel;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Load();           
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F5))
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.F9))
        {
            stageLevel++;
        }
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/LoadData.dat");

        LoadData data = new LoadData();

        data.stageLevel = stageLevel;

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        BinaryFormatter bf = new BinaryFormatter();        
        FileStream file = File.Open(Application.persistentDataPath + "/LoadData.dat", FileMode.Open);

        if(file != null && file.Length > 0)
        {
            LoadData data = (LoadData)bf.Deserialize(file);

            stageLevel = data.stageLevel;
        }

        file.Close();
    }

    public int GetStageLevel()
    {
        return stageLevel;
    }

    public void SetStageLevel()
    {
        stageLevel++;
    }
}
