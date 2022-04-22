using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCollectable : MonoBehaviour
{
    public AudioClip collectedClip;

    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        controller.ChangeCogs(10);
        Destroy(gameObject);
        controller.PlaySound(collectedClip);
    }      
        

}
