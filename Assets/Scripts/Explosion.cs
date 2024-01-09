using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float Radius = 15.0f;
    public float waitToDelete = 5.0f;
    void Start()
    {
        RaycastHit[] collisions = Physics.SphereCastAll(transform.position, Radius, Vector3.one);
        foreach (RaycastHit hit in collisions)
        {
            if (hit.transform.tag == "Enemy" )
            {
                EnemyBehaviour Ennemy = hit.transform.parent.gameObject.GetComponent<EnemyBehaviour>();
                Ennemy.Die();
            }            
        }
        StartCoroutine(KillYourself());
    }
    IEnumerator KillYourself() 
    {
        yield return new WaitForSeconds(waitToDelete);
        Destroy(gameObject);
    }
}