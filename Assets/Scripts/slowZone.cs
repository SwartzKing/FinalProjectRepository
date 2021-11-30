using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slowZone : MonoBehaviour
{

    void OnTriggerStay2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
                controller.speed = 1;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
                controller.speed = 3;
        }
    }
}
