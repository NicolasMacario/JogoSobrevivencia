using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Estado inicial")]
    [SerializeField] private int dia = 1;
    [SerializeField] private int comida = 30;

    [Header("Tarefas")]
    [SerializeField] private int comidaPorBusca = 4;

    [Header("Interface")]
    [SerializeField] private TMP_Text textoDia;
    [SerializeField] private TMP_Text textoComida;
    [SerializeField] private TMP_Text textoMoradores;
    [SerializeField] private TMP_Text textoRelatorio;
    [Header("Conflitos")]
    [SerializeField] private GerenciadorDeConflitos gerenciadorDeConflitos;

    private Morador[] moradores =
    {
        new Morador("Ana", 1),
        new Morador("Bruno", 1),
        new Morador("Carla", 2),
        new Morador("Diego", 2),
        new Morador("Eva", 3),
        new Morador("Felipe", 3)
    };
    public bool TemComida(int quantidade)
{
    return comida >= quantidade;
}

public bool TentarGastarComida(int quantidade)
{
    if (partidaEncerrada || quantidade < 0 || comida < quantidade)
    {
        return false;
    }

    comida -= quantidade;
    AtualizarInterface();

    return true;
}

    public event Action EstadoAtualizado;

    [Header("Fim da partida")]
    [SerializeField] private int diasParaVencer = 15;
    [SerializeField] private int limiteDiasComFome = 3;
    [SerializeField] private Button botaoEncerrarDia;

    private int diasSeguidosComFome = 0;
    private bool partidaEncerrada = false;
    private void Start()
    {
        textoRelatorio.text =
            "Selecione um andar e distribua as tarefas.\n\n" +
            $"Cada busca traz {comidaPorBusca} unidades de comida.";

        AtualizarInterface();
    }

    public Morador ObterMorador(int indice)
    {
        return moradores[indice];
    }

    public void DefinirTarefa(int indice, bool buscarComida)
{
    if (partidaEncerrada || gerenciadorDeConflitos.EventoPendente)
{
    return;
}
    if (indice < 0 || indice >= moradores.Length)
    {
        return;
    }

    Morador morador = moradores[indice];

    if (buscarComida && morador.Energia < 25)
    {
        return;
    }

    morador.BuscandoComida = buscarComida;
    EstadoAtualizado?.Invoke();
    
}

    public void EncerrarDia()
{
    if (partidaEncerrada || gerenciadorDeConflitos.EventoPendente)
{
    return;
}

    int diaEncerrado = dia;
    int pessoasEmBusca = 0;
    int comidaColetada = 0;

    foreach (Morador morador in moradores)
    {
        if (morador.BuscandoComida && morador.Energia >= 25)
        {
            pessoasEmBusca++;
        }

        comidaColetada += morador.ResolverDia(comidaPorBusca);
    }

    comida += comidaColetada;

    int comidaConsumida = Mathf.Min(comida, moradores.Length);
    int moradoresSemComida = moradores.Length - comidaConsumida;

    comida -= comidaConsumida;

    if (moradoresSemComida > 0)
    {
        diasSeguidosComFome++;
    }
    else
    {
        diasSeguidosComFome = 0;
    }

    textoRelatorio.text =
        $"Fim do dia {diaEncerrado}\n\n" +
        $"Moradores em busca: {pessoasEmBusca}\n" +
        $"Comida coletada: +{comidaColetada}\n" +
        $"Comida consumida: -{comidaConsumida}\n" +
        $"Estoque final: {comida}\n" +
        $"Dias seguidos com fome: " +
        $"{diasSeguidosComFome}/{limiteDiasComFome}";

    // A sobrevivência é verificada antes da vitória.
    if (diasSeguidosComFome >= limiteDiasComFome)
    {
        FinalizarPartida(false);
    }
    else if (diaEncerrado >= diasParaVencer)
    {
        FinalizarPartida(true);
    }
    else
    {
        dia++;
    }

    AtualizarInterface();
    if (!partidaEncerrada)
{
    gerenciadorDeConflitos.VerificarEvento(diaEncerrado);
}
}
private void FinalizarPartida(bool venceu)
{
    partidaEncerrada = true;
    botaoEncerrarDia.interactable = false;

    if (venceu)
    {
        textoRelatorio.text =
            $"VITÓRIA!\n\n" +
            $"A comunidade sobreviveu por {diasParaVencer} dias.\n\n" +
            $"Comida restante: {comida}";
    }
    else
    {
        textoRelatorio.text =
            $"DERROTA\n\n" +
            $"A comunidade passou {limiteDiasComFome} dias " +
            "consecutivos sem comida suficiente.\n\n" +
            $"Último dia: {dia}";
    }
}

    private void AtualizarInterface()
    {
        textoDia.text = $"Dia: {dia}";
        textoComida.text = $"Comida: {comida}";
        textoMoradores.text = $"Moradores: {moradores.Length}";

        EstadoAtualizado?.Invoke();
    }
}