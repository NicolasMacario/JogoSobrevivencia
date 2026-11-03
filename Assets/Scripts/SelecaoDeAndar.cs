using TMPro;
using UnityEngine;

public class SelecaoDeAndar : MonoBehaviour
{
    [SerializeField] private TMP_Text textoDetalhesAndar;

    private void Start()
    {
        textoDetalhesAndar.text =
            "Clique em um andar para ver seus moradores.";
    }

    public void SelecionarAndar(int numeroAndar)
    {
        string moradores;

        switch (numeroAndar)
        {
            case 1:
                moradores = "Ana\nBruno";
                break;

            case 2:
                moradores = "Carla\nDiego";
                break;

            case 3:
                moradores = "Eva\nFelipe";
                break;

            default:
                return;
        }

        textoDetalhesAndar.text =
            $"{numeroAndar}º andar\n\n{moradores}";
    }
}