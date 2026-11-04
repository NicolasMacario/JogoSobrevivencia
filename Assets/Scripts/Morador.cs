public enum TarefaMorador
{
    Descansar,
    BuscarComida,
    BuscarMateriais,
    RepararBarricada
}

public class Morador
{
    public string Nome { get; private set; }
    public int Andar { get; private set; }
    public int Energia { get; private set; }
    public TarefaMorador Tarefa { get; private set; }

    public Morador(string nome, int andar)
    {
        Nome = nome;
        Andar = andar;
        Energia = 100;
        Tarefa = TarefaMorador.Descansar;
    }

    public void DefinirTarefa(TarefaMorador novaTarefa)
    {
        Tarefa = novaTarefa;
    }

    public string NomeDaTarefa()
    {
        switch (Tarefa)
        {
            case TarefaMorador.BuscarComida:
                return "Buscar comida";

            case TarefaMorador.BuscarMateriais:
                return "Buscar materiais";

            case TarefaMorador.RepararBarricada:
                return "Reparar barricada";

            default:
                return "Descansar";
        }
    }

    public bool Trabalhar()
    {
        if (Energia < 25)
        {
            return false;
        }

        Energia -= 25;

        if (Energia < 25)
        {
            Tarefa = TarefaMorador.Descansar;
        }

        return true;
    }

    public void Descansar()
    {
        Energia = System.Math.Min(100, Energia + 50);
    }
}