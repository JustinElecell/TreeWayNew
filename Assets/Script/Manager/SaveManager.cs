using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SaveData
{
    public List<string> skillItemList = new List<string>();
    public int targetCharaterNo;
    public int coin;
}



public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        Load();
    }
    public SaveData saveData = new SaveData();


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("全部刪除");
            PlayerPrefs.DeleteAll();

            if (ES3.FileExists("./Save/TEST.es3"))
            {
                ES3.DeleteFile("./Save/TEST.es3");
            }
            if (ES3.FileExists("./Save/HintsTEST.es3"))
            {
                ES3.DeleteFile("./Save/HintsTEST.es3");
            }
            PlayerPrefs.DeleteAll();
            //PlayerPrefs.HasKey("log")
                
        }

    }


    //public void SaveHints(string NoString,int HideCount)
    //{
    //    ES3.Save<int>(NoString, HideCount, "./Save/HintsTEST.es3");
    //}

    //public int LoadHints(string NoString)
    //{
    //    Debug.Log("save"+ ES3.FileExists("./Save/HintsTEST.es3"));

    //    if (ES3.FileExists("./Save/HintsTEST.es3"))
    //    {
    //        return ES3.Load<int>(NoString, "./Save/HintsTEST.es3");

    //    }
    //    else
    //    {
    //        for (int i = 1; i <= 16; i++)
    //        {
    //            ES3.Save<int>(i.ToString(), 0, "./Save/HintsTEST.es3");
    //        }
            

    //    }
    //    return 0;


    //}


    public void Save()
    {
        Debug.Log("開始Save");

        if (ES3.FileExists("./Save/TEST.es3"))
        {
            ES3.DeleteFile("./Save/TEST.es3");
        }
        ES3.Save("Save", saveData, "./Save/TEST.es3");

    }

    public void SaveCoin(int coin)
    {
        saveData.coin = coin;
        Debug.Log(coin);
        Save();
    }

    public void Load()
    {
        Debug.Log("開始讀取");
        if (ES3.FileExists("./Save/TEST.es3"))
        {
            saveData = ES3.Load<SaveData>("Save", "./Save/TEST.es3");
            Debug.Log(($"讀取 : {saveData.ToString()}"));
        }
        else
        {
            // 沒讀取到資料，初始設定一份並保存
            saveData.targetCharaterNo = 0;
            for(int i=0;i<5;i++)
            {
                saveData.skillItemList.Add("");
            }


            //switch(Application.systemLanguage)
            //{
            //    case SystemLanguage.ChineseSimplified:
            //        saveData.languageType = LanguageType.簡中;
            //        break;
            //    case SystemLanguage.ChineseTraditional:
            //        saveData.languageType = LanguageType.繁中;
            //        break;
            //    case SystemLanguage.English:
            //        saveData.languageType = LanguageType.English;
            //        break;
            //    case SystemLanguage.Spanish:
            //        saveData.languageType = LanguageType.Spanish;
            //        break;
            //    case SystemLanguage.French:
            //        saveData.languageType = LanguageType.French;
            //        break;
            //    case SystemLanguage.Portuguese:
            //        saveData.languageType = LanguageType.Portugal;
            //        break;
            //    case SystemLanguage.German:
            //        saveData.languageType = LanguageType.Deutsch;
            //        break;
            //}

            Save();
        }
    }

}
