using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class GameManager : MonoBehaviour
{
    //Controle AR
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;
    private ARPlane plano;
    private GameObject cursor;
    public GameObject alvo;
    public GameObject alvoErrado;
    public Image mira;

    //Controle do jogo
    private int pontuacao;
    public int numeroAlvos = 10;
    private bool acabou = false;
    public TextMeshProUGUI msg;
    public TextMeshProUGUI pontuacaotxt;
    public float probInimigo = 0.5f;
    public float distTempoSpawn = 0.8f;
    //public float tempoAtivoAlvo = 4f;
    public LayerMask alvoLayer;
    public PlayRandomPitchSounds tocaSom;
    private float tempoRestante = 10;
    public float tempoInicial = 10;
    private bool tempoAtivo = false;
    public float tempoAcerto = 1.5f;
    public float pontoAcerto = 10;


    void Start()
    {
        cursor = transform.Find("Cursor").gameObject;
        msg.text = "Mapeie o chão e toque na tela para começar";
    }

    void Update()
    {
        if (!acabou)
        {
            TratarPlanos();
            TratarMira();
            TempoDiminui();
            pontuacaotxt.text = "Pontuação: " + pontuacao;
        }
        else
        {
            TratarFimJogo();
        }
    }

    private void TempoDiminui()
    {
        if (tempoAtivo) {
            if (tempoRestante > 0)
            {
                tempoRestante -= Time.deltaTime;

                float minutos = Mathf.FloorToInt(tempoRestante / 60);
                float segundos = Mathf.FloorToInt(tempoRestante % 60);

                msg.text = string.Format("{0:00}:{1:00}", minutos, segundos);
                //MostraTempo(timeRemaining);
            }
            else
            {
                Acabar();
                tempoRestante = tempoInicial;
                tempoAtivo = false;
            }
        }
    }

    private void TratarPlanos() {
        if (plano != null) {
            return;
        }

        var posicaoTela = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));

        var raycastHits = new List<ARRaycastHit>();

        raycastManager.Raycast(posicaoTela, raycastHits, TrackableType.PlaneWithinBounds);

        if (raycastHits.Count > 0) {
            transform.position = raycastHits[0].pose.position;
            transform.rotation = raycastHits[0].pose.rotation;

            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began) {
                ComecarJogo(raycastHits[0]);
            }
        }
    }

    private void ComecarJogo(ARRaycastHit raycastHit) {
        msg.text = "";
        var idPlano = raycastHit.trackableId;
        plano = planeManager.GetPlane(idPlano);

        cursor.SetActive(false);
        planeManager.enabled = false;

        StartCoroutine(criarAlvo());
        tempoAtivo = true;
    }

    private Vector3 ObterPontoAleatorio() {
        var x = Random.Range(-1f, 1f);
        var z = Random.Range(-1f, 1f);
        var y = Random.Range(.1f, 2f);

        var vetorAleatorio = new Vector3(x, y, z);

        var posicaoAleatoria = plano.transform.TransformPoint(vetorAleatorio);

        return posicaoAleatoria;
    }

    IEnumerator criarAlvo()
    {
        while (!acabou)
        {
            var pontoAleatorio = ObterPontoAleatorio();
            Quaternion rotacaoZ180 = Quaternion.Euler(0, 0, Random.Range(0, 360));
            GameObject target = Random.value < probInimigo ?
                GameObject.Instantiate(alvoErrado, pontoAleatorio, Quaternion.identity * rotacaoZ180) :
                GameObject.Instantiate(alvo, pontoAleatorio, Quaternion.identity * rotacaoZ180);

            target.transform.LookAt(Camera.main.transform);
            yield return new WaitForSeconds(distTempoSpawn);
        }
    }
    private void TratarMira()
    {
        RaycastHit hitInfo;

        var camera = Camera.main.transform;
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            tocaSom.Play(0);
        }
        if (Physics.Raycast(camera.position, camera.forward, out hitInfo, 10000))
        {

            if (hitInfo.transform.CompareTag("Alvo"))
            {
                mira.color = Color.red;
                if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Timer tempoDoAlvo = hitInfo.transform.GetComponent<Timer>();
                    float bonus = tempoDoAlvo.tempoAtual / tempoDoAlvo.tempoMaximo;
                    bonus = (1 - bonus) * pontoAcerto;
                    TratarAcerto(bonus);
                    Destroy(hitInfo.transform.gameObject);
                }
            }
            else if (hitInfo.transform.CompareTag("AlvoErrado"))
            {
                mira.color = Color.red;
                if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Destroy(hitInfo.transform.gameObject);
                    TratarErro();
                }

            }
            else mira.color = Color.white;
        }
        else mira.color = Color.white;
    }

    private void TratarAcerto(float tempoBonus)
    {
        tocaSom.Play(2);
        pontuacao += (int)(10 + tempoBonus);

        numeroAlvos--;

        tempoRestante += tempoAcerto;
        if (numeroAlvos > 0)
        {
        }
        else
        {
            Acabar();
            msg.text = "Fim de jogo! \n\nSua pontuação é de " + pontuacao;
        }
    }

    private void TratarErro()
    {
        tocaSom.Play(1);
        pontuacao = Mathf.Max(0, pontuacao - 5);
    }

    private void TratarFimJogo()
    {
        if (Input.touchCount == 1
            && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void Acabar()
    {
        acabou = true;
        msg.text = "Fim de jogo.\n\nSua pontuação é de " + pontuacao;

    }
}
