using UnityEngine;

public enum RecursoDeExploracao
{
    Comida,
    Materiais
}

[DisallowMultipleComponent]
public sealed class ArmarioExploracao : MonoBehaviour
{
    [SerializeField] private RecursoDeExploracao recurso;
    [SerializeField, Min(0)] private int unidadesRestantes = 4;

    public RecursoDeExploracao Recurso => recurso;
    public int UnidadesRestantes => unidadesRestantes;
    public string NomeDoRecurso => recurso == RecursoDeExploracao.Comida
        ? "comida"
        : "materiais";

    public bool TentarRetirarUnidade()
    {
        if (unidadesRestantes <= 0)
        {
            return false;
        }

        unidadesRestantes--;
        return true;
    }
}
