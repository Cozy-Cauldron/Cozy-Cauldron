using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveData
{
    //completed tasks
    public bool[] completedPages;
    //current scene
    public string currentScene;
    //character location
    public float playerX, playerY, playerZ;
    //inventory
    public List<string> itemNames = new List<string>();
    public List<int> itemCounts = new List<int>();
    public List<Sprite> itemSprites = new List<Sprite>();
    public List<string> itemDescriptions = new List<string>();

}

public class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/save.json";

    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game saved to " + savePath);
    }

    public static SaveData LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Game loaded from " + savePath);
            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found at " + savePath);
            return null;
        }
    }
}
