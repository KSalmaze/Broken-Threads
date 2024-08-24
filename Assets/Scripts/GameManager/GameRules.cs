using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GameRules : MonoBehaviour
{
    [SerializeField] private float duracaoDaPartida;
    [SerializeField] private TMP_Text timer;
    [SerializeField] private TMP_Text textoPontuacao;
    [SerializeField] private TMP_Text textoPontuacaoOponente;

    [SerializeField] private GameObject endCanvas;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private GameObject loseCanvas;
    
    private int pontuacao, pontuacaoOponente;
    private float tempo = 0;
    private MusicPlayer musicPlayer;
    
    // Start is called before the first frame update
    void Start()
    {
        tempo = duracaoDaPartida;
        musicPlayer = GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        tempo -= Time.deltaTime;
        timer.text = tempo.ToString();
        
        if (tempo <= duracaoDaPartida)
        {
            if (pontuacao == pontuacaoOponente)
            {
                Empate();
            }
            else
            {
                if (pontuacao > pontuacaoOponente)
                {
                    FimDePartida(true);
                }
                else
                {
                    FimDePartida(false);
                }
            }
        }
    }

    void Empate()
    {
        // Entrar no Modo de 
    }
    
    void FimDePartida(bool resultado)
    {
        endCanvas.SetActive(true);
        
        if (resultado)
        {
            winCanvas.SetActive(true);
        }
        else
        {
            loseCanvas.SetActive(true);
        }
    }
}
