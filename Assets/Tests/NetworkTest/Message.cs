using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Message
{
    public readonly string Tag;
    public readonly string User;
    public readonly byte[] Content;
    
    public Message(string tag, byte[] content)
    {
        Tag = tag;
        Content = content;
        User = ConnectionSingleton.Instance.Player_Name;
    }
}
