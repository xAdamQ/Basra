using System;
using UnityEngine;

public class PublicMinUserInfo
{
    public string Id { get; set; }
    public virtual int Level { get; set; }
    public virtual string Title { get; set; }
    public string DisplayName { get; set; }
    public Texture2D Picture { get; set; }
}