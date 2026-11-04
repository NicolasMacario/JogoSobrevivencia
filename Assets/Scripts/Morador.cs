public enum TarefaMorador
{
    Descansar,
    BuscarComida
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

            default:
                return "Descansar";
        }
    }

    public int ResolverDia(int comidaPorBusca)
    {
        if (Tarefa == TarefaMorador.BuscarComida && Energia >= 25)
        {
            Energia -= 25;

            if (Energia < 25)
            {
                Tarefa = TarefaMorador.Descansar;
            }

            return comidaPorBusca;
        }

        Tarefa = TarefaMorador.Descansar;
        Energia = System.Math.Min(100, Energia + 50);

        return 0;
    }
}