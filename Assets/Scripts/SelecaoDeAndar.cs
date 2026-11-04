using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelecaoDeAndar : MonoBehaviour
{
    [Header("Gerenciador")]
    [SerializeField] private GameManager gameManager;

    [Header("Informações")]
    [SerializeField] private TMP_Text textoDetalhesAndar;

    [Header("Botões dos moradores")]
    [SerializeField] private Button botaoMorador1;
    [SerializeField] private Button botaoMorador2;
    [SerializeField] private TMP_Text textoBotaoMorador1;
    [SerializeField] private TMP_Text textoBotaoMorador2;

    [Header("Botões das tarefas")]
    [SerializeField] private Button botaoBuscarComida;
    [SerializeField] private Button botaoDescansar;

    private int andarSelecionado = 0;
    private int indiceMoradorSelecionado = -1;

    private void OnEnable()
    {
        gameManager.EstadoAtualizado += AtualizarPainel;
    }

    private void OnDisable()
    {
        gameManager.EstadoAtualizado -= AtualizarPainel;
    }

    private void Start()
    {
        AtualizarPainel();
    }

    public void SelecionarAndar(int numeroAndar)
    {
        if (numeroAndar < 1 || numeroAndar > 3)
        {
            return;
        }

        andarSelecionado = numeroAndar;
        indiceMoradorSelecionado = -1;

        AtualizarPainel();
    }

    public void SelecionarMorador(int posicao)
    {
        if (andarSelecionado == 0 || posicao < 0 || posicao > 1)
        {
            return;
        }

        indiceMoradorSelecionado =
            (andarSelecionado - 1) * 2 + posicao;

        AtualizarPainel();
    }

    public void BuscarComida()
    {
        if (indiceMoradorSelecionado < 0)
        {
            return;
        }

        gameManager.DefinirTarefa(indiceMoradorSelecionado, true);
    }

    public void Descansar()
    {
        if (indiceMoradorSelecionado < 0)
        {
            return;
        }

        gameManager.DefinirTarefa(indiceMoradorSelecionado, false);
    }

    private void AtualizarPainel()
    {
        bool temAndar = andarSelecionado > 0;
        bool temMorador = indiceMoradorSelecionado >= 0;

        botaoMorador1.interactable = temAndar;
        botaoMorador2.interactable = temAndar;

        botaoBuscarComida.interactable = temMorador;
        botaoDescansar.interactable = temMorador;

        if (!temAndar)
        {
            textoDetalhesAndar.text = "Selecione um andar.";
            textoBotaoMorador1.text = "Morador 1";
            textoBotaoMorador2.text = "Morador 2";
            return;
        }

        int primeiroIndice = (andarSelecionado - 1) * 2;

        Morador primeiro = gameManager.ObterMorador(primeiroIndice);
        Morador segundo = gameManager.ObterMorador(primeiroIndice + 1);

        textoBotaoMorador1.text =
            $"{primeiro.Nome} — {primeiro.NomeDaTarefa()}";

        textoBotaoMorador2.text =
            $"{segundo.Nome} — {segundo.NomeDaTarefa()}";

        if (temMorador)
        {
            Morador selecionado =
                gameManager.ObterMorador(indiceMoradorSelecionado);

            textoDetalhesAndar.text =
                $"{selecionado.Andar}º andar | {selecionado.Nome}\n" +
                $"Tarefa: {selecionado.NomeDaTarefa()}";
        }
        else
        {
            textoDetalhesAndar.text =
                $"{andarSelecionado}º andar\nSelecione um morador.";
        }
    }
}