using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class ZumbiExploracao : MonoBehaviour
{
    [SerializeField] private MochilaExploracao jogador;
    [SerializeField] private Transform pontoA;
    [SerializeField] private Transform pontoB;

    [SerializeField] private float velocidadePatrulha = 1.3f;
    [SerializeField] private float velocidadePerseguicao = 2.8f;
    [SerializeField] private float distanciaParaDetectar = 3f;
    [SerializeField] private float distanciaParaDesistir = 5f;
    [SerializeField] private float distanciaParaAlcancar = 0.8f;
    [SerializeField] private Color corDeAlerta = new Color(0.95f, 0.25f, 0.2f);

    private Rigidbody2D corpo;
    private SpriteRenderer visual;
    private Vector2 posicaoInicial;
    private Color corDePatrulha;
    private bool indoParaB;
    private bool perseguindo;

    private void Awake()
    {
        corpo = GetComponent<Rigidbody2D>();
        visual = GetComponent<SpriteRenderer>();
        posicaoInicial = corpo.position;
        corDePatrulha = visual.color;
    }

    private void OnEnable()
    {
        corpo.position = posicaoInicial;
        indoParaB = true;
        perseguindo = false;
        visual.color = corDePatrulha;
    }

    private void FixedUpdate()
    {
        if (jogador == null || pontoA == null || pontoB == null)
        {
            return;
        }

        Vector2 posicao = corpo.position;
        Vector2 posicaoJogador = jogador.transform.position;
        float distancia = Vector2.Distance(posicao, posicaoJogador);

        perseguindo = perseguindo
            ? distancia < distanciaParaDesistir
            : distancia <= distanciaParaDetectar;

        visual.color = perseguindo ? corDeAlerta : corDePatrulha;

        if (perseguindo && distancia <= distanciaParaAlcancar)
        {
            jogador.SerAlcancadoPorZumbi();
            return;
        }

        Transform pontoDePatrulha = indoParaB ? pontoB : pontoA;
        float destinoX = perseguindo ? posicaoJogador.x : pontoDePatrulha.position.x;
        float velocidade = perseguindo ? velocidadePerseguicao : velocidadePatrulha;

        float proximoX = Mathf.MoveTowards(
            posicao.x, destinoX, velocidade * Time.fixedDeltaTime);

        corpo.MovePosition(new Vector2(proximoX, posicao.y));

        if (!perseguindo && Mathf.Approximately(proximoX, destinoX))
        {
            indoParaB = !indoParaB;
        }
    }
}
