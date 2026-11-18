using System;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MontarColetaExploracao
{
    private const string NomeDaMontagem = "Montar coleta de teste";
    private const string NomeDaRaiz = "ColetaExploracao";
    private const string NomeDoCanvas = "CanvasColeta";

    [MenuItem("Ultimo Andar/Montar coleta de teste")]
    private static void Montar()
    {
        Scene cena = SceneManager.GetActiveScene();
        if (EditorApplication.isPlayingOrWillChangePlaymode || cena.name != "ExploracaoTeste")
        {
            EditorUtility.DisplayDialog(NomeDaMontagem,
                "Abra a cena ExploracaoTeste e saia do Play.", "OK");
            return;
        }

        GameObject jogador = EncontrarObjeto(cena, "JogadorExploracao");
        GameObject chao = EncontrarObjeto(cena, "Chao");

        if (jogador == null || chao == null ||
            !chao.TryGetComponent(out SpriteRenderer visualDoChao) || visualDoChao.sprite == null ||
            !chao.TryGetComponent(out BoxCollider2D piso))
        {
            EditorUtility.DisplayDialog(NomeDaMontagem,
                "Confira se a cena tem JogadorExploracao e Chao. O Chao precisa de um sprite e um Box Collider 2D.",
                "OK");
            return;
        }

        if (jogador.GetComponent<MochilaExploracao>() != null ||
            EncontrarObjeto(cena, NomeDaRaiz) != null || EncontrarObjeto(cena, NomeDoCanvas) != null)
        {
            EditorUtility.DisplayDialog(NomeDaMontagem,
                "A coleta já foi montada nesta cena. Confira ColetaExploracao na Hierarchy.", "OK");
            return;
        }

        if (TMP_Settings.instance == null || TMP_Settings.defaultFontAsset == null)
        {
            EditorUtility.DisplayDialog(NomeDaMontagem,
                "Importe os recursos de texto em Window > TextMeshPro > Import TMP Essential Resources.",
                "OK");
            return;
        }

        Undo.IncrementCurrentGroup();
        int grupoUndo = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName(NomeDaMontagem);

        try
        {
            MontarObjetos(jogador, piso, visualDoChao);
        }
        catch (Exception erro)
        {
            Undo.RevertAllDownToGroup(grupoUndo);
            Debug.LogException(erro);
            return;
        }

        Undo.CollapseUndoOperations(grupoUndo);
        EditorSceneManager.MarkSceneDirty(cena);
        Selection.activeGameObject = jogador;
    }

    private static void MontarObjetos(GameObject jogador, BoxCollider2D piso, SpriteRenderer visualDoChao)
    {
        GameObject raiz = CriarObjeto(NomeDaRaiz, null);
        Bounds limitesDoPiso = piso.bounds;
        float altura = limitesDoPiso.max.y + 0.75f;
        float profundidade = jogador.transform.position.z;
        float xComida = Mathf.Lerp(limitesDoPiso.min.x, limitesDoPiso.max.x, 0.45f);
        float xMateriais = Mathf.Lerp(limitesDoPiso.min.x, limitesDoPiso.max.x, 0.75f);

        ArmarioExploracao comida = CriarArmario(raiz.transform, visualDoChao,
            "ArmarioComida", RecursoDeExploracao.Comida,
            new Vector3(xComida, altura, profundidade), new Color32(198, 137, 71, 255));

        ArmarioExploracao materiais = CriarArmario(raiz.transform, visualDoChao,
            "ArmarioMateriais", RecursoDeExploracao.Materiais,
            new Vector3(xMateriais, altura, profundidade), new Color32(85, 158, 176, 255));

        TMP_Text visor = CriarVisor();

        MochilaExploracao mochila = Undo.AddComponent<MochilaExploracao>(jogador);
        var dadosDaMochila = new SerializedObject(mochila);
        SerializedProperty campoArmarios = dadosDaMochila.FindProperty("armarios");
        campoArmarios.arraySize = 2;
        campoArmarios.GetArrayElementAtIndex(0).objectReferenceValue = comida;
        campoArmarios.GetArrayElementAtIndex(1).objectReferenceValue = materiais;
        dadosDaMochila.FindProperty("textoMochila").objectReferenceValue = visor;
        dadosDaMochila.ApplyModifiedProperties();
    }

    private static GameObject EncontrarObjeto(Scene cena, string nome)
    {
        foreach (GameObject raiz in cena.GetRootGameObjects())
        {
            foreach (Transform objeto in raiz.GetComponentsInChildren<Transform>(true))
            {
                if (objeto.name == nome)
                {
                    return objeto.gameObject;
                }
            }
        }

        return null;
    }

    private static ArmarioExploracao CriarArmario(Transform pai, SpriteRenderer visualDoChao,
        string nome, RecursoDeExploracao recurso, Vector3 posicao, Color cor)
    {
        GameObject objeto = CriarObjeto(nome, pai, typeof(SpriteRenderer), typeof(ArmarioExploracao));
        objeto.transform.position = posicao;

        Vector3 tamanhoDoSprite = visualDoChao.sprite.bounds.size;
        objeto.transform.localScale = new Vector3(1.25f / tamanhoDoSprite.x, 1.5f / tamanhoDoSprite.y, 1f);

        SpriteRenderer visual = objeto.GetComponent<SpriteRenderer>();
        visual.sprite = visualDoChao.sprite;
        visual.sharedMaterial = visualDoChao.sharedMaterial;
        visual.color = cor;
        visual.sortingOrder = -1;

        ArmarioExploracao armario = objeto.GetComponent<ArmarioExploracao>();
        var dadosDoArmario = new SerializedObject(armario);
        dadosDoArmario.FindProperty("recurso").enumValueIndex = (int)recurso;
        dadosDoArmario.ApplyModifiedPropertiesWithoutUndo();

        return armario;
    }

    private static TMP_Text CriarVisor()
    {
        GameObject objetoCanvas = CriarObjeto(NomeDoCanvas, null,
            typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
        Canvas canvas = objetoCanvas.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        CanvasScaler scaler = objetoCanvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject painel = CriarObjeto("VisorMochila", objetoCanvas.transform, typeof(RectTransform), typeof(Image));
        RectTransform rectPainel = painel.GetComponent<RectTransform>();
        rectPainel.anchorMin = new Vector2(0f, 1f);
        rectPainel.anchorMax = rectPainel.anchorMin;
        rectPainel.pivot = rectPainel.anchorMin;
        rectPainel.anchoredPosition = new Vector2(24f, -24f);
        rectPainel.sizeDelta = new Vector2(815f, 106f);

        Image fundo = painel.GetComponent<Image>();
        fundo.color = new Color32(20, 25, 31, 230);
        fundo.raycastTarget = false;

        GameObject objetoTexto = CriarObjeto("TextoMochila", rectPainel, typeof(RectTransform), typeof(TextMeshProUGUI));
        RectTransform rectTexto = objetoTexto.GetComponent<RectTransform>();
        rectTexto.anchorMin = Vector2.zero;
        rectTexto.anchorMax = Vector2.one;
        rectTexto.offsetMin = new Vector2(20f, 8f);
        rectTexto.offsetMax = new Vector2(-20f, -8f);

        TMP_Text texto = objetoTexto.GetComponent<TextMeshProUGUI>();
        texto.font = TMP_Settings.defaultFontAsset;
        texto.fontSize = 29f;
        texto.alignment = TextAlignmentOptions.Left;
        texto.color = new Color32(246, 244, 238, 255);
        texto.raycastTarget = false;
        texto.text = "Mochila 0/6  |  Comida 0  |  Materiais 0\nAproxime-se de um armário.";

        return texto;
    }

    private static GameObject CriarObjeto(string nome, Transform pai, params Type[] componentes)
    {
        var objeto = new GameObject(nome, componentes);
        Undo.RegisterCreatedObjectUndo(objeto, NomeDaMontagem);

        if (pai != null)
        {
            Undo.SetTransformParent(objeto.transform, pai, NomeDaMontagem);
        }

        Undo.RegisterFullObjectHierarchyUndo(objeto, NomeDaMontagem);
        if (pai != null)
        {
            objeto.transform.localPosition = Vector3.zero;
            objeto.transform.localRotation = Quaternion.identity;
            objeto.transform.localScale = Vector3.one;
        }

        return objeto;
    }
}
