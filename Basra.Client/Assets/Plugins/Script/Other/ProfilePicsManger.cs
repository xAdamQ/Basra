// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.Networking;

// /*todo
//  * how to implemet this
//  * cache downloaded into dictionary
//  * update pics on period
//  * download maybe failed for some pics and the game is closed
//  * new friends may be added dpwnload thier pics
//  */

// class ProfilePicsManger
// {
//     public static Dictionary<string, Sprite> ProfilePics;

//     static string ProfilePicsPath;

//     public static void Init()
//     {
//         ProfilePics = new Dictionary<string, Sprite>();
//         AppManger.I.FirstEnterEvent += OnFirstEnter;
//         ProfilePicsPath = Path.Combine(Application.persistentDataPath, "ProfilePic");
//         DownLoadAllProfilePics();
//     }

//     static void OnFirstEnter()
//     {
//         Directory.CreateDirectory(ProfilePicsPath);
//     }

//     static void DownLoadAllProfilePics()
//     {
//         //if (GameData.I.UpdateProfilePicsDate.Subtract(DateTime.Now).Days == 0) return;

//         GameData.I.UpdateProfilePicsDate = DateTime.Now;

//         DownloadImage(AppManger.I.FirebaseUser.PhotoUrl + "?height=250", "User");
//         //update friends
//     }

//     public static void DownloadImage(string url, string name)
//     {
//         Debug.Log(url);
//         var request = UnityWebRequestTexture.GetTexture(url);

//         request.SendWebRequest().completed += obj =>
//         {
//             if (request.isNetworkError || request.isHttpError)
//             {
//                 Debug.Log(request.error);//todo what to do?
//             }
//             else
//             {
//                 var newTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;

//                 var sprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), Vector2.zero);

//                 ProfilePics.Add(name, sprite);
//                 ProfilePic.All[name].GetComponent<UnityEngine.UI.Image>().sprite = sprite;
//             }
//         };
//     }
//     public static void DownloadProfilePic(string id)
//     {
//         DownloadImage(string.Concat("https://graph.facebook.com/", id, "/picture" + "?height=250"), id.ToString());
//     }

//     #region prev attempt
//     //File.WriteAllBytes(Application.persistentDataPath + "/" + name + ".png", texture2D.EncodeToPNG());

//     //public static void SetTexture(UnityEngine.UI.Image image, string name)
//     //{
//     //    var path = Path.Combine(ProfilePicsPath, name + ".png");
//     //    if (!File.Exists(path)) return;

//     //    var newTexture = new Texture2D(0, 0);
//     //    newTexture.LoadImage(File.ReadAllBytes(path));
//     //    image.sprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), Vector2.zero);
//     //}

//     //static void CacheProfilePics()
//     //{
//     //    ProfilePics = new Dictionary<string, Sprite>();

//     //    var all = new DirectoryInfo(ProfilePicsPath).GetFiles();
//     //    foreach (var file in all)
//     //    {
//     //        var newTexture = new Texture2D(0, 0);
//     //        newTexture.LoadImage(File.ReadAllBytes(file.FullName));
//     //        var sprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), Vector2.zero);

//     //        ProfilePics.Add(file.Name.Remove(file.Name.Length - 4), sprite);
//     //    }
//     //}

//     #endregion
// }
