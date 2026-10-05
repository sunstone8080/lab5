using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;

    void Update()
    {
        Keyboard kb = Keyboard.current;

        if (kb == null)
        {
            return;
        }

        float x = 0f;
        float z = 0f;

        if (kb.dKey.isPressed)
        {
            x += 1f;
        }

        if (kb.aKey.isPressed)
        {
            x -= 1f;
        }

        if (kb.wKey.isPressed)
        {
            z += 1f;
        }

        if (kb.sKey.isPressed)
        {
            z -= 1f;
        }

        transform.position += new Vector3(x, 0f, z).normalized * speed * Time.deltaTime;
    }
}