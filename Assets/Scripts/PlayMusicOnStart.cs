using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusicOnStart : MonoBehaviour
{
    [SerializeField] private string musica;
    void Start()
    {
        GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>().MudarMusica(musica, true);
    }
}
