public class Morador
{
    public string Nome { get; private set; }
    public int Andar { get; private set; }
    public int Energia { get; private set; }
    public bool BuscandoComida { get; set; }

    public Morador(string nome, int andar)
    {
        Nome = nome;
        Andar = andar;
        Energia = 100;
        BuscandoComida = false;
    }

    public string NomeDaTarefa()
    {
        return BuscandoComida ? "Buscar comida" : "Descansar";
    }

    public int ResolverDia(int comidaPorBusca)
    {
        if (BuscandoComida && Energia >= 25)
        {
            Energia -= 25;

            if (Energia < 25)
            {
                BuscandoComida = false;
            }

            return comidaPorBusca;
        }

        BuscandoComida = false;
        Energia = System.Math.Min(100, Energia + 50);

        return 0;
    }
}