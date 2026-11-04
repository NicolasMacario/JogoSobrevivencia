using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GerenciadorDeConflitos : MonoBehaviour
{
    [Header("Gerenciador")]
    [SerializeField] private GameManager gameManager;

    [Header("Janela")]
    [SerializeField] private GameObject painelConflito;
    [SerializeField] private TMP_Text textoConflito;
    [SerializeField] private Button botaoComidaExtra;

    [Header("Textos dentro dos andares")]
    [SerializeField] private TMP_Text textoAndar1;
    [SerializeField] private TMP_Text textoAndar2;
    [SerializeField] private TMP_Text textoAndar3;

    private int[] confianca = { 50, 50, 50 };
    private bool conflitoJaExibido = false;

    public bool EventoPendente { get; private set; }

    private void Start()
    {
        painelConflito.SetActive(false);
        AtualizarTextosDosAndares();
    }

    public void VerificarEvento(int diaEncerrado)
    {
        if (diaEncerrado != 3 || conflitoJaExibido)
        {
            return;
        }

        conflitoJaExibido = true;
        EventoPendente = true;

        textoConflito.text =
            "O PREÇO DO PRIMEIRO ANDAR\n\n" +
            "Os moradores do primeiro andar dizem que " +
            "correm mais riscos por viverem perto da entrada. " +
            "Eles exigem uma porção extra de comida.\n\n" +
            "Entregar: custa 6 de comida. Confiança: " +
            "+15 no primeiro andar e -5 nos demais.\n\n" +
            "Manter porções iguais: confiança " +
            "-15 no primeiro andar e +5 nos demais.";

        botaoComidaExtra.interactable =
            gameManager.TemComida(6);

        painelConflito.SetActive(true);
    }

    public void EntregarComidaExtra()
    {
        if (!EventoPendente)
        {
            return;
        }

        if (!gameManager.TentarGastarComida(6))
        {
            return;
        }

        AlterarConfianca(0, 15);
        AlterarConfianca(1, -5);
        AlterarConfianca(2, -5);

        FinalizarEvento();
    }

    public void ManterPorcoesIguais()
    {
        if (!EventoPendente)
        {
            return;
        }

        AlterarConfianca(0, -15);
        AlterarConfianca(1, 5);
        AlterarConfianca(2, 5);

        FinalizarEvento();
    }

    private void AlterarConfianca(int indice, int valor)
    {
        confianca[indice] =
            Mathf.Clamp(confianca[indice] + valor, 0, 100);
    }

    private void FinalizarEvento()
    {
        EventoPendente = false;
        AtualizarTextosDosAndares();
        painelConflito.SetActive(false);
    }

    private void AtualizarTextosDosAndares()
    {
        textoAndar1.text =
            $"1º andar — 2 moradores\nConfiança: {confianca[0]}/100";

        textoAndar2.text =
            $"2º andar — 2 moradores\nConfiança: {confianca[1]}/100";

        textoAndar3.text =
            $"3º andar — 2 moradores\nConfiança: {confianca[2]}/100";
    }
}