using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCollider : MonoBehaviour
{
    public GameObject canvas;
    public GameObject projectile;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            print("COUCOU");
            canvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        print("TSCHUSS");
        canvas.SetActive(false);
    }
}
