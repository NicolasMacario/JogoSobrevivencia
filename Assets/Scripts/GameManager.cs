using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Estado inicial")]
    [SerializeField] private int dia = 1;
    [SerializeField] private int comida = 30;
    [SerializeField] private int moradores = 6;

    [Header("Interface")]
    [SerializeField] private TMP_Text textoDia;
    [SerializeField] private TMP_Text textoComida;
    [SerializeField] private TMP_Text textoMoradores;
    [SerializeField] private TMP_Text textoRelatorio;

    private void Start()
    {
        AtualizarInterface();

        textoRelatorio.text =
            "Cada morador precisa de 1 unidade de comida por dia.";
    }

    public void EncerrarDia()
    {
        int diaEncerrado = dia;
        int comidaConsumida = Mathf.Min(comida, moradores);
        int moradoresSemComida = moradores - comidaConsumida;

        comida -= comidaConsumida;
        dia++;

        textoRelatorio.text =
            $"Fim do dia {diaEncerrado}\n\n" +
            $"Comida consumida: {comidaConsumida}\n" +
            $"Comida restante: {comida}";

        if (moradoresSemComida > 0)
        {
            textoRelatorio.text +=
                $"\n\nAtenção: {moradoresSemComida} moradores " +
                "ficaram sem comida.";
        }

        AtualizarInterface();
    }

    private void AtualizarInterface()
    {
        textoDia.text = $"Dia: {dia}";
        textoComida.text = $"Comida: {comida}";
        textoMoradores.text = $"Moradores: {moradores}";
    }
}