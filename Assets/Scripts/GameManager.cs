using System;
using TMPro;
using UnityEngine;

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

    private Morador[] moradores =
    {
        new Morador("Ana", 1),
        new Morador("Bruno", 1),
        new Morador("Carla", 2),
        new Morador("Diego", 2),
        new Morador("Eva", 3),
        new Morador("Felipe", 3)
    };

    public event Action EstadoAtualizado;

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

        // As buscas são resolvidas antes da alimentação.
        comida += comidaColetada;

        int comidaConsumida = Mathf.Min(comida, moradores.Length);
        int moradoresSemComida = moradores.Length - comidaConsumida;

        comida -= comidaConsumida;
        dia++;

        textoRelatorio.text =
            $"Fim do dia {diaEncerrado}\n\n" +
            $"Moradores em busca: {pessoasEmBusca}\n" +
            $"Comida coletada: +{comidaColetada}\n" +
            $"Comida consumida: -{comidaConsumida}\n" +
            $"Estoque final: {comida}";

        if (moradoresSemComida > 0)
        {
            textoRelatorio.text +=
                $"\n\nAtenção: {moradoresSemComida} " +
                "moradores ficaram sem comida.";
        }

        AtualizarInterface();
    }

    private void AtualizarInterface()
    {
        textoDia.text = $"Dia: {dia}";
        textoComida.text = $"Comida: {comida}";
        textoMoradores.text = $"Moradores: {moradores.Length}";

        EstadoAtualizado?.Invoke();
    }
}