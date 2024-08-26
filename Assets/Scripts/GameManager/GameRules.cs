using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private float tempo;
    private MusicPlayer musicPlayer;
    
    // Start is called before the first frame update
    void Start()
    {
        tempo = duracaoDaPartida;
        musicPlayer = GameObject.Find("MusicPlayer").GetComponent<MusicPlayer>();
        musicPlayer.StartMatch();
    }

    // Update is called once per frame
    void Update()
    {
        textoPontuacao.text = pontuacao.ToString();
        textoPontuacaoOponente.text = pontuacaoOponente.ToString();
        
        tempo -= Time.deltaTime;
        if (tempo <= 0)
        {
            timer.text = "0.00";
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
        else  //so atualiza o tempo se for maior que zero
        {
            timer.text = tempo.ToString("F2");
            switch (tempo)
            {
                case <= 30f and >  20f:
                    timer.color = Color.Lerp(Color.white, Color.yellow, (30f - tempo) / 10f); break;
                case <=20f and >10f:
                    timer.color = Color.Lerp(Color.yellow, Color.red,   (20f - tempo) / 10f); break;
            }
        }
        
        if (tempo <= tempo1)
        {
            Debug.Log("Trigger 1");
            TriggerA();
            tempo1 = -50;
        }
        if (tempo <= tempo2)
        {
            Debug.Log("Trigger 2");
            TriggerB();
            tempo2 = -50;
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
        Application.Quit();
        SceneManager.LoadScene(0);
    }
}
