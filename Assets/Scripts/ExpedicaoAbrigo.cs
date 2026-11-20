using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExpedicaoDoAbrigo : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Button botaoExplorar;
    [SerializeField] private string nomeDaCena = "ExploracaoTeste";

    private readonly List<GameObject> objetosDoAbrigo = new List<GameObject>();
    private readonly List<GameObject> objetosDoAndar = new List<GameObject>();

    private Scene cenaDoAndar;
    private MochilaExploracao mochilaAtiva;
    private bool expedicaoEmAndamento;

    private bool PodeSair()
    {
        return !expedicaoEmAndamento &&
               gameManager != null &&
               gameManager.PodeIniciarExpedicao();
    }

    private void Update()
    {
        if (botaoExplorar != null)
        {
            botaoExplorar.interactable = PodeSair();
        }
    }

    public void IniciarExpedicao()
    {
        if (!PodeSair())
        {
            return;
        }

        AsyncOperation carregamento = null;

        if (!cenaDoAndar.isLoaded)
        {
            if (SceneManager.GetSceneByName(nomeDaCena).isLoaded)
            {
                Debug.LogError("Inicie o Play com apenas a cena do abrigo aberta.");
                return;
            }

            if (!Application.CanStreamedLevelBeLoaded(nomeDaCena))
            {
                Debug.LogError($"Adicione a cena {nomeDaCena} à lista de cenas da Unity.");
                return;
            }

            carregamento = SceneManager.LoadSceneAsync(nomeDaCena, LoadSceneMode.Additive);
            if (carregamento == null)
            {
                return;
            }
        }

        expedicaoEmAndamento = true;
        StartCoroutine(MostrarAndar(carregamento));
    }

    private IEnumerator MostrarAndar(AsyncOperation carregamento)
    {
        PausarCena(gameObject.scene, objetosDoAbrigo, gameObject);

        if (carregamento != null)
        {
            yield return carregamento;
            cenaDoAndar = SceneManager.GetSceneByName(nomeDaCena);
            mochilaAtiva = EncontrarMochila();

            if (mochilaAtiva == null || !mochilaAtiva.TemEscadaDeRetorno)
            {
                if (cenaDoAndar.isLoaded)
                {
                    yield return SceneManager.UnloadSceneAsync(cenaDoAndar);
                }

                MostrarAbrigo();
                expedicaoEmAndamento = false;
                Debug.LogError("Ligue a escada à mochila e salve ExploracaoTeste.");
                yield break;
            }
        }

        SceneManager.SetActiveScene(cenaDoAndar);
        ReativarObjetos(objetosDoAndar);
        SceneManager.SetActiveScene(cenaDoAndar);
        ReativarObjetos(objetosDoAndar);
        mochilaAtiva.ReposicionarNaEntrada();
        mochilaAtiva.VincularRetorno(VoltarAoAbrigo);
        mochilaAtiva.VincularRetorno(VoltarAoAbrigo);
    }

    private MochilaExploracao EncontrarMochila()
    {
        if (!cenaDoAndar.isLoaded)
        {
            return null;
        }

        foreach (GameObject objeto in cenaDoAndar.GetRootGameObjects())
        {
            MochilaExploracao mochila = objeto.GetComponentInChildren<MochilaExploracao>();
            if (mochila != null && mochila.isActiveAndEnabled)
            {
                return mochila;
            }
        }

        return null;
    }

    private void VoltarAoAbrigo(MochilaExploracao mochila)
    {
        if (!expedicaoEmAndamento || mochila != mochilaAtiva)
        {
            return;
        }

        int comidaColetada = mochila.Comida;
        int materiaisColetados = mochila.Materiais;

        mochila.VincularRetorno(null);
        mochila.Esvaziar();

        PausarCena(cenaDoAndar, objetosDoAndar);
        MostrarAbrigo();

        try
        {
            gameManager.ReceberColetaDaExpedicao(comidaColetada, materiaisColetados);
        }
        finally
        {
            expedicaoEmAndamento = false;
        }
    }

    private void MostrarAbrigo()
    {
        SceneManager.SetActiveScene(gameObject.scene);
        ReativarObjetos(objetosDoAbrigo);
    }

    private static void PausarCena(
        Scene cena, List<GameObject> objetos, GameObject excecao = null)
    {
        foreach (GameObject objeto in cena.GetRootGameObjects())
        {
            if (objeto != excecao && objeto.activeSelf)
            {
                objetos.Add(objeto);
                objeto.SetActive(false);
            }
        }
    }

    private static void ReativarObjetos(List<GameObject> objetos)
    {
        foreach (GameObject objeto in objetos)
        {
            if (objeto != null)
            {
                objeto.SetActive(true);
            }
        }

        objetos.Clear();
    }
}