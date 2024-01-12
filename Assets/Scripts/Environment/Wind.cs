using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public static Vector3 WindForward;
    private Vector3 WindDirection;
    private Vector3 NewWindDirection;

    public float MaxAnglePerRandom;
    public float WindTransitionSpeed;

    public float MinimumTimeChange = 3.0f;
    public float MaximumTimeChange = 8.0f;

    private void Start()
    {
        StartCoroutine(TimeStep());
    }

    private void Update()
    {
        WindDirection = new Vector3(Mathf.Lerp(WindDirection.x, NewWindDirection.x, Time.deltaTime * WindTransitionSpeed),
                                    Mathf.Lerp(WindDirection.y, NewWindDirection.y, Time.deltaTime * WindTransitionSpeed),
                                    0.0f);
        gameObject.transform.rotation = Quaternion.Euler(WindDirection);
        WindForward = gameObject.transform.forward;
    }


    IEnumerator TimeStep()
    {
        NewWindDirection = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(WindDirection.y - MaxAnglePerRandom, WindDirection.y + MaxAnglePerRandom), 0);

        //Be sure to not make a bigger angle than 180°
        if(Mathf.Abs(WindDirection.y + NewWindDirection.y) > 180)
        {
            NewWindDirection.y %= 180.0f;
        }        

        yield return new WaitForSeconds(Random.Range(MinimumTimeChange, MaximumTimeChange));
        StartCoroutine(TimeStep());
    }
}
