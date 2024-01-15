using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float Radius = 15.0f;
    public float waitToDelete = 5.0f;
    void Update()
    {
        RaycastHit[] collisions = Physics.SphereCastAll(transform.position, Radius, Vector3.one);
        foreach (RaycastHit hit in collisions)
        {
            if (hit.transform.tag == "Enemy" )
            {
                Enemy enemy = hit.transform.parent.gameObject.GetComponent<Enemy>();
                enemy.Die(true);
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