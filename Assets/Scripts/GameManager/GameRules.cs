using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private GameObject drawCanvas;

    [Header("Triggers")] 
    [SerializeField] private float tempo1;
    [SerializeField] private float tempo2;
    
    public int pontuacao, pontuacaoOponente;
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

        if (tempo <= tempo1)
        {
            Debug.Log("Trigger 1");
            TriggerA();
            tempo1 = 0;
        }
        if(tempo <= tempo2)
        {
            Debug.Log("Trigger 2");
            TriggerB();
            tempo2 = 0;
        }
    }

    void TriggerA()
    {
        
    }

    void TriggerB()
    {
        
    }
    
    void Empate()
    {
        StartCoroutine(EndGame()); 
        drawCanvas.SetActive(true);
    }
    
    void FimDePartida(bool resultado)
    {
       // endCanvas.SetActive(true);
       StartCoroutine(EndGame()); 
       
        if (resultado)
        {
            winCanvas.SetActive(true);
        }
        else
        {
            loseCanvas.SetActive(true);
        }
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(6);
        ConnectionSingleton.Instance.Connection.Quit();
        SceneManager.LoadScene(0);
    }
}
