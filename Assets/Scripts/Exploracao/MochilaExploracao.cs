using System;
using TMPro;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[DisallowMultipleComponent]
public sealed class MochilaExploracao : MonoBehaviour
{
    [SerializeField, Min(1)] private int capacidade = 6;
    [SerializeField, Min(0.1f)] private float alcance = 1.5f;
    [SerializeField] private ArmarioExploracao[] armarios = Array.Empty<ArmarioExploracao>();
    [SerializeField] private TMP_Text textoMochila;

    private ArmarioExploracao armarioProximo;

    public int Comida { get; private set; }
    public int Materiais { get; private set; }
    public int Total => Comida + Materiais;

    private void Start()
    {
        armarioProximo = EncontrarArmarioProximo();
        AtualizarVisor();
    }

    private void Update()
    {
        ArmarioExploracao alvo = EncontrarArmarioProximo();

        if (!ReferenceEquals(alvo, armarioProximo))
        {
            armarioProximo = alvo;
            AtualizarVisor();
        }

        if (alvo == null || Total >= capacidade || !PressionouInteracao())
        {
            return;
        }

        if (!alvo.TentarRetirarUnidade())
        {
            return;
        }

        if (alvo.Recurso == RecursoDeExploracao.Comida)
        {
            Comida++;
        }
        else
        {
            Materiais++;
        }

        AtualizarVisor();
    }

    private ArmarioExploracao EncontrarArmarioProximo()
    {
        ArmarioExploracao melhorAlvo = null;
        float menorDistanciaAoQuadrado = alcance * alcance;
        Vector2 posicaoJogador = transform.position;

        foreach (ArmarioExploracao armario in armarios)
        {
            if (armario == null || !armario.isActiveAndEnabled)
            {
                continue;
            }

            Vector2 distancia = (Vector2)armario.transform.position - posicaoJogador;
            if (distancia.sqrMagnitude <= menorDistanciaAoQuadrado)
            {
                melhorAlvo = armario;
                menorDistanciaAoQuadrado = distancia.sqrMagnitude;
            }
        }

        return melhorAlvo;
    }

    private static bool PressionouInteracao()
    {
#if ENABLE_INPUT_SYSTEM
        Keyboard teclado = Keyboard.current;
        return teclado != null && teclado.eKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.E);
#endif
    }

    private void AtualizarVisor()
    {
        if (textoMochila == null)
        {
            return;
        }

        textoMochila.text =
            $"Mochila {Total}/{capacidade}  |  Comida {Comida}  |  Materiais {Materiais}\n" +
            ObterInstrucao();
    }

    private string ObterInstrucao()
    {
        if (Total >= capacidade)
        {
            return "Mochila cheia.";
        }

        if (armarioProximo == null)
        {
            return "Aproxime-se de um armário.";
        }

        if (armarioProximo.UnidadesRestantes == 0)
        {
            return "Armário vazio.";
        }

        return $"E para pegar {armarioProximo.NomeDoRecurso}  ·  " +
            $"{armarioProximo.UnidadesRestantes} no armário";
    }
}
