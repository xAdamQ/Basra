using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

[Serializable]
public class GameData
{
    public string FacebookAccessToken;

    public DateTime UpdateProfilePicsDate;

    #region untility
    public static GameData I;

    static string DataPath = Application.persistentDataPath + "GD.prog";
    public static void Save()
    {
        var formatter = new BinaryFormatter();
        var stream = new FileStream(DataPath, FileMode.Create);
        formatter.Serialize(stream, I);
        stream.Close();
    }
    public static void Load()
    {
        if (File.Exists(DataPath))
        {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(DataPath, FileMode.Open);

            var data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            I = data;
        }
        else
        {
            FirstEnter();
        }

        OnLoad();
    }//only happen once in app

    public static void Delete()
    {
        if (File.Exists(DataPath))
        {
            File.Delete(DataPath);
            Debug.Log("data deleted successfully");
        }
        else
        {
            Debug.Log("data already deleted");
        }
    }

    static void FirstEnter()
    {
        I = new GameData();
        SetDefaults();
        Save();
    }

    static void SetDefaults()
    {
        I.UpdateProfilePicsDate = DateTime.Now;
    }
    static void OnLoad()
    {
    }
    #endregion

}

//[Serializable]
//public struct NetSprite
//{
//    public byte[] Bytes;
//    public int Width, Height;
//    bool IsDownloaded;

//    public Action Downloaded { get; set; }

//    /// <summary>
//    /// creates sprite from texture bytes
//    /// </summary>
//    public Sprite Sprite
//    {
//        get
//        {
//            var texture = new Texture2D(Width, Height); texture.LoadImage(Bytes);
//            return Sprite.Create(texture, new Rect(0, 0, Width, Height), new Vector2());
//        }
//    }

//    /// <summary>
//    /// auto save
//    /// </summary>
//    public void Download(string url)
//    {
//        AppManger.I.StartCoroutine(DownloadCo(url));
//    }
//    IEnumerator DownloadCo(string url)
//    {
//        var request = UnityWebRequestTexture.GetTexture(url);

//        yield return request.SendWebRequest();

//        if (request.isNetworkError || request.isHttpError)
//        {
//            Debug.Log(request.error);
//        }
//        else
//        {
//            var texture2D = ((DownloadHandlerTexture)request.downloadHandler).texture;

//            //texture2D.EncodeToPNG();

//            Bytes = texture2D.EncodeToPNG();
//            Width = texture2D.width;
//            Height = texture2D.height;

//            Downloaded?.Invoke();

//            //GameData.Save();
//        }
//    }

//}

//struct NetSprite2
//{
//    public string Name;

//}
