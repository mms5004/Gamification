using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeArm : MonoBehaviour
{

    public Text _Text;
    public ArmEnum ArmType;
    public string ChangeText = "Press E to change arm";

    private Catapult Player;

    private List<Arm> armList;

    public Arm armPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player = other.GetComponent<Catapult>();

            if (Player._currentArmClass == ArmType)
            {
                Player = null;
                return;
            }

            _Text.gameObject.SetActive(true);
            _Text.text = ChangeText;

          
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player" && Player != null)
        {
            Player = null;
            _Text.gameObject.SetActive(false);

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Player != null)
        {
            if (Input.GetKeyDown(KeyCode.E)) {

                Player._currentArmClass = ArmType;
                _Text.gameObject.SetActive(false);
                Player.OnChangeArm(armPrefab);
                
            
            
            
            }
        }
        
    }
}
