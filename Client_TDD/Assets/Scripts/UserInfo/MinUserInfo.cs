using System;
using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class MinUserInfo
{
    public string Id { get; set; }
    public virtual int Level { get; set; }
    public virtual string Title { get; set; }
    public string DisplayName { get; set; }
    public string PictureUrl { get; set; }
    public Texture2D Picture { get; set; }

    public event Action<Texture2D> PictureLoaded;
    public bool IsPictureLoaded;

    public async UniTask DownloadPicture()
    {
        if (string.IsNullOrEmpty(PictureUrl)) return;

        Picture = await GetRemoteTexture(PictureUrl);
        PictureLoaded?.Invoke(Picture);
        IsPictureLoaded = true;
    }

    public static async UniTask<Texture2D> GetRemoteTexture(string url)
    {
        using (var www = UnityWebRequestTexture.GetTexture(url))
        {
            // begin request:
            var asyncOp = www.SendWebRequest();

            // await until it's done: 
            await asyncOp;

            // read results:
            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                // log error:
#if DEBUG
                Debug.Log($"{www.error}, URL:{www.url}");
#endif

                // nothing to return on error:
                return null;
            }

            // return valid results:
            Debug.Log($"img from url {url} downloaded");
            return DownloadHandlerTexture.GetContent(www);
        }
    }
}