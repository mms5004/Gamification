using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCollider : MonoBehaviour
{
    public Text _Text;
    public ProjectileClass ProjectileType;
    public string StringText = "Press E to use ";

    private Catapult Player;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player = other.GetComponent<Catapult>();
            print("COUCOU");
            //canvas.SetActive(true);

            if(Player._currentProjectileClass == ProjectileType)
            {
                Player = null;
                return;
            }

            _Text.gameObject.SetActive(true);
            _Text.text = StringText;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && Player != null)
        {
            Player = null;
            _Text.gameObject.SetActive(false);
            print("bye");
        }
    }
}
