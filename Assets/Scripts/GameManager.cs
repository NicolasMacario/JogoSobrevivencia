using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Estado inicial")]
    [SerializeField] private int dia = 1;
    [SerializeField] private int comida = 30;
    [SerializeField] private int materiais = 6;

    [Header("Buscas")]
    [SerializeField] private int comidaPorBusca = 4;
    [SerializeField] private int materiaisPorBusca = 4;

    [Header("Barricada")]
    [SerializeField] private int resistencia = 80;
    [SerializeField] private int resistenciaMaxima = 100;
    [SerializeField] private int reparoPorMorador = 20;
    [SerializeField] private int custoDoReparo = 2;

    [Header("Interface")]
    [SerializeField] private TMP_Text textoDia;
    [SerializeField] private TMP_Text textoComida;
    [SerializeField] private TMP_Text textoMoradores;
    [SerializeField] private TMP_Text textoRelatorio;
    [SerializeField] private TMP_Text textoMateriais;
    [SerializeField] private TMP_Text textoBarricada;
    [SerializeField] private Slider barraBarricada;

    [Header("Fim da partida")]
    [SerializeField] private int diasParaVencer = 15;
    [SerializeField] private int limiteDiasComFome = 3;
    [SerializeField] private Button botaoEncerrarDia;

    [Header("Conflitos")]
    [SerializeField] private GerenciadorDeConflitos gerenciadorDeConflitos;

    private int diasSeguidosComFome = 0;
    private bool partidaEncerrada = false;

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
            "Distribua tarefas para alimentar os moradores " +
            "e manter a entrada protegida.";

        AtualizarInterface();
    }

    public Morador ObterMorador(int indice)
    {
        return moradores[indice];
    }

    public void DefinirTarefa(int indice, TarefaMorador tarefa)
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

        if (tarefa != TarefaMorador.Descansar && morador.Energia < 25)
        {
            return;
        }

        morador.DefinirTarefa(tarefa);
        EstadoAtualizado?.Invoke();
    }

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

    public void EncerrarDia()
    {
        if (partidaEncerrada || gerenciadorDeConflitos.EventoPendente)
        {
            return;
        }

        int diaEncerrado = dia;
        int comidaColetada = 0;
        int materiaisColetados = 0;
        int resistenciaReparada = 0;
        int reparadoresDescansaram = 0;

        // Guarda as tarefas antes de alguém gastar energia.
        TarefaMorador[] tarefasDoDia =
            new TarefaMorador[moradores.Length];

        for (int i = 0; i < moradores.Length; i++)
        {
            tarefasDoDia[i] = moradores[i].Tarefa;
        }

        // Primeiro: buscas. Os materiais já poderão ser usados hoje.
        for (int i = 0; i < moradores.Length; i++)
        {
            TarefaMorador tarefa = tarefasDoDia[i];

            if (tarefa != TarefaMorador.BuscarComida &&
                tarefa != TarefaMorador.BuscarMateriais)
            {
                continue;
            }

            if (!moradores[i].Trabalhar())
            {
                moradores[i].DefinirTarefa(TarefaMorador.Descansar);
                moradores[i].Descansar();
                continue;
            }

            if (tarefa == TarefaMorador.BuscarComida)
            {
                comidaColetada += comidaPorBusca;
            }
            else
            {
                materiaisColetados += materiaisPorBusca;
            }
        }

        comida += comidaColetada;
        materiais += materiaisColetados;

        // Segundo: reparos, na ordem da lista dos moradores.
        for (int i = 0; i < moradores.Length; i++)
        {
            if (tarefasDoDia[i] != TarefaMorador.RepararBarricada)
            {
                continue;
            }

            Morador morador = moradores[i];

            bool podeReparar =
                resistencia < resistenciaMaxima &&
                materiais >= custoDoReparo &&
                morador.Energia >= 25;

            if (!podeReparar)
            {
                morador.Descansar();
                reparadoresDescansaram++;
                continue;
            }

            if (morador.Trabalhar())
            {
                int reparoReal = Mathf.Min(
                    reparoPorMorador,
                    resistenciaMaxima - resistencia
                );

                materiais -= custoDoReparo;
                resistencia += reparoReal;
                resistenciaReparada += reparoReal;
            }
        }

        // Terceiro: descanso de quem recebeu essa tarefa.
        for (int i = 0; i < moradores.Length; i++)
        {
            if (tarefasDoDia[i] == TarefaMorador.Descansar)
            {
                moradores[i].Descansar();
            }
        }

        // Alimentação.
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

        // Ataque: aumenta a cada bloco de cinco dias.
        int danoNoturno = 10 + ((diaEncerrado - 1) / 5) * 5;
        resistencia = Mathf.Max(0, resistencia - danoNoturno);

        textoRelatorio.text =
            $"Fim do dia {diaEncerrado}\n" +
            $"Comida: +{comidaColetada} / -{comidaConsumida}\n" +
            $"Materiais coletados: +{materiaisColetados}\n" +
            $"Resistência reparada: +{resistenciaReparada}\n" +
            $"Ataque noturno: -{danoNoturno}\n" +
            $"Dias com fome: {diasSeguidosComFome}/{limiteDiasComFome}";

        if (reparadoresDescansaram > 0)
        {
            textoRelatorio.text +=
                $"\nReparadores sem trabalho: {reparadoresDescansaram}" +
                "\nDescansaram por falta de energia, materiais " +
                "ou necessidade de reparo.";
        }

        // Uma invasão ou a fome impedem a vitória naquele dia.
        if (resistencia <= 0)
        {
            FinalizarPartida(false, "A barricada caiu e o prédio foi invadido.");
        }
        else if (diasSeguidosComFome >= limiteDiasComFome)
        {
            FinalizarPartida(false, "A comunidade ficou sem comida suficiente por tempo demais.");
        }
        else if (diaEncerrado >= diasParaVencer)
        {
            FinalizarPartida(true, "");
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

    private void FinalizarPartida(bool venceu, string motivo)
    {
        partidaEncerrada = true;
        botaoEncerrarDia.interactable = false;

        textoRelatorio.text = venceu
            ? $"VITÓRIA!\n\nA comunidade sobreviveu por {diasParaVencer} dias."
            : $"DERROTA\n\n{motivo}\n\nÚltimo dia: {dia}";
    }

    private void AtualizarInterface()
    {
        textoDia.text = $"Dia: {dia}";
        textoComida.text = $"Comida: {comida}";
        textoMoradores.text = $"Moradores: {moradores.Length}";
        textoMateriais.text = $"Materiais: {materiais}";

        textoBarricada.text =
            $"Barricada: {resistencia}/{resistenciaMaxima}";

        barraBarricada.minValue = 0;
        barraBarricada.maxValue = resistenciaMaxima;
        barraBarricada.value = resistencia;

        EstadoAtualizado?.Invoke();
    }
}