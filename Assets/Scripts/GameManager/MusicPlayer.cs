using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private List<AudioClip> musicasLista;
    [SerializeField] private List<string> nomeMusicas;
    [SerializeField] private AudioListener globalAudioListener;
    [SerializeField] private AudioSource globalAudioSorcer;
    private Dictionary<string,AudioClip> _musicas;
    
    private Transform _atualTransform;
    
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this);
        _atualTransform = this.GetComponent<Transform>();
        
        _musicas = new Dictionary<string, AudioClip>();
        
        // Inicializando dicionário
        for(int i = 0; i < musicasLista.Count; i++)
        {
            _musicas.Add(nomeMusicas[i], musicasLista[i]);
        }
    }

    private void Update()
    {
        _atualTransform = Camera.main.gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    public void MudarMusica(string nomeMusica, bool loop = false)
    {
        if (!_musicas.ContainsKey(nomeMusica))
        {
            Debug.Log("Musica não encontrada");
            return;
        }
        
        globalAudioSorcer.loop = loop;

        globalAudioSorcer.Stop();
        globalAudioSorcer.clip = _musicas[nomeMusica];
        globalAudioSorcer.Play();
    }

    public void StartMatch()
    {
        globalAudioSorcer.Stop();
        MudarMusica("Prosti", false);
        StartCoroutine(MusicaAcabou());
    }

    IEnumerator MusicaAcabou()
    {
        yield return new WaitForSeconds(40);
        MudarMusica("Devil");
    }
}
