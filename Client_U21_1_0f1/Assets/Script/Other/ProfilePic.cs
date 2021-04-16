// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ProfilePic : MonoBehaviour
// {
//     public static Dictionary<string, ProfilePic> All = new Dictionary<string, ProfilePic>();
//     [SerializeField] string ProfileId;

//     void Awake()
//     {
//         All.Add(ProfileId, this);
//     }
//     void Start()
//     {
//         if (ProfilePicsManger.ProfilePics.ContainsKey(ProfileId))
//             GetComponent<SpriteRenderer>().sprite = ProfilePicsManger.ProfilePics[ProfileId];
//     }
//     void OnDestroy()
//     {
//         All.Remove(ProfileId);
//     }
// }
