public class Morador
{
    public string Nome { get; private set; }
    public int Andar { get; private set; }
    public bool BuscandoComida { get; set; }

    public Morador(string nome, int andar)
    {
        Nome = nome;
        Andar = andar;
        BuscandoComida = false;
    }

    public string NomeDaTarefa()
    {
        return BuscandoComida ? "Buscar comida" : "Descansar";
    }
}