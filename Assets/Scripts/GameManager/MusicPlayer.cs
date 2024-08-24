using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private List<AudioClip> musicas;
    [SerializeField] private List<string> nomeMusicas;
    [SerializeField] private AudioListener globalAudioListener;
    [SerializeField] private AudioSource globalAudioSorcer;
    private Transform _atualTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        _atualTransform = this.GetComponent<Transform>();
    }

    private void Update()
    {
        _atualTransform = Camera.main.gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void MudarMusica(string nomeMusica, bool loop = false)
    {
        globalAudioSorcer.loop = loop;
        
        foreach (string nome in nomeMusicas)
        {
            if (nome.ToUpper().Contains(nomeMusica.ToUpper()))
            {
                globalAudioSorcer.clip = musicas[nomeMusicas.FindIndex(n => n == nome)];
                globalAudioSorcer.Play();
                return;
            }
        }
    }
}
