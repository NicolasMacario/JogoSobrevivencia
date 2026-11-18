using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D))]
public class MovimentoExploracao : MonoBehaviour
{
    [SerializeField] private float velocidade = 4f;

    private Rigidbody2D corpo;
    private float direcaoHorizontal;

    private void Awake()
    {
        corpo = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        direcaoHorizontal = 0f;

#if ENABLE_INPUT_SYSTEM
        Keyboard teclado = Keyboard.current;

        if (teclado == null)
        {
            return;
        }

        if (teclado.aKey.isPressed || teclado.leftArrowKey.isPressed)
        {
            direcaoHorizontal -= 1f;
        }

        if (teclado.dKey.isPressed || teclado.rightArrowKey.isPressed)
        {
            direcaoHorizontal += 1f;
        }
#else
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            direcaoHorizontal -= 1f;
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            direcaoHorizontal += 1f;
        }
#endif
    }

    private void FixedUpdate()
    {
#if UNITY_6000_0_OR_NEWER
        Vector2 movimento = corpo.linearVelocity;
        movimento.x = direcaoHorizontal * velocidade;
        corpo.linearVelocity = movimento;
#else
        Vector2 movimento = corpo.velocity;
        movimento.x = direcaoHorizontal * velocidade;
        corpo.velocity = movimento;
#endif
    }

    private void OnDisable()
    {
        direcaoHorizontal = 0f;
    }
}