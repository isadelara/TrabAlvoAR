using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public GameObject pai;
    public float tempoMaximo = 4;
    public float tempoAtual = 0;
    public bool tempoAtivo = false;
    public Animator anim;


    private void Start()
    {
        tempoAtivo = true;
        if (Random.value < .5f)
        {
            anim.SetTrigger("move1");
        }
        else
        {
            anim.SetTrigger("move2");
        }
    }

    void Update()
    {
        if (tempoAtivo)
        {
            if (tempoAtual < tempoMaximo)
            {
                tempoAtual += Time.deltaTime;
            }
            else
            {
                tempoAtual = 0;
                tempoAtivo = false;
                anim.SetTrigger("end");

            }
        }
    }
    public void end() {
        Destroy(pai);
    }
}