using UnityEngine;

public class Temp : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


// マスタデータ用のクラス例
[System.Serializable]
public class CharacterStatus
{
    public int id;
    public string name;
    public int hp;
    public int attack;
    public int defense;
    public string description;
}
    
[System.Serializable]
public class StoryData
{
    public int chapterId;
    public string chapterTitle;
    public string content;
    public string characterName;
    public string backgroundImage;
}