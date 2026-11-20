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
    [SerializeField] private Transform escadaDeRetorno;

    private Action<MochilaExploracao> aoVoltarAoAbrigo;
    private bool pertoDaEscada;

    public bool TemEscadaDeRetorno => escadaDeRetorno != null;

    public void VincularRetorno(Action<MochilaExploracao> acao)
    {
        aoVoltarAoAbrigo = acao;
        pertoDaEscada = EscadaAoAlcance();
        AtualizarVisor();
    }

    public void Esvaziar()
    {
        Comida = 0;
        Materiais = 0;
        AtualizarVisor();
    }

    private bool EscadaAoAlcance()
    {
        if (aoVoltarAoAbrigo == null || escadaDeRetorno == null)
        {
            return false;
        }

        Vector2 distancia = (Vector2)(escadaDeRetorno.position - transform.position);
        return distancia.sqrMagnitude <= alcance * alcance;
    }

    private void Start()
    {
        armarioProximo = EncontrarArmarioProximo();
        AtualizarVisor();
    }

    private void Update()
    {
        ArmarioExploracao alvo = EncontrarArmarioProximo();
        bool podeVoltar = EscadaAoAlcance();

        if (!ReferenceEquals(alvo, armarioProximo) || podeVoltar != pertoDaEscada)
        {
            armarioProximo = alvo;
            pertoDaEscada = podeVoltar;
            AtualizarVisor();
        }

        if (!PressionouInteracao())
        {
            return;
        }

        if (pertoDaEscada)
        {
            aoVoltarAoAbrigo(this);
            return;
        }

        if (alvo == null || Total >= capacidade || !alvo.TentarRetirarUnidade())
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
        if (pertoDaEscada)
            {
                return "E para voltar ao abrigo";
            }
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
