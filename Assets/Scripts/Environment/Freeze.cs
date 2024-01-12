using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : MonoBehaviour
{
    public float Radius = 15.0f;
    public float waitToDelete = 15.0f;

    private void Start()
    {
        StartCoroutine(KillYourself());
    }
    void Update()
    {
        RaycastHit[] collisions = Physics.SphereCastAll(transform.position, Radius, -Vector3.up * 100);
        foreach (RaycastHit hit in collisions)
        {
            if (hit.transform.tag == "Enemy")
            {
                Enemy enemy = hit.transform.parent.gameObject.GetComponent<Enemy>();
                enemy.Freeze();
            }           
        }
        
    }
    IEnumerator KillYourself()
    {
        yield return new WaitForSeconds(waitToDelete);
        Destroy(gameObject);
    }
}