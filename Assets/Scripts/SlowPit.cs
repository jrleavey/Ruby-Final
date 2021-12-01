using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowPit : MonoBehaviour
{
    // Start is called before the first frame update
    void OnTriggerStay2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            controller.ChangeSpeed();
        }
    }
}
