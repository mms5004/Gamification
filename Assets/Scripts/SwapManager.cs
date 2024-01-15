using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SwapManager : MonoBehaviour
{
    [SerializeField] private SwapElement[] _swapElements;
    [SerializeField] private GameObject _UI;
    [SerializeField] private TextMeshProUGUI _Text;
    [SerializeField] private SwapElement _currentArm;
    [SerializeField] private SwapElement _currentProjectile;
    private Catapult Player;

    private void Start()
    {
        _swapElements = FindObjectsByType<SwapElement>(FindObjectsSortMode.None);
        Player = FindAnyObjectByType<Catapult>();
        foreach (var element in _swapElements)
        {
            element.Initialize(this);

            element.gameObject.SetActive(element != _currentArm && element != _currentProjectile);
        }
    }

    public void ToggleDisplayUI(bool display, string name)
    {
        if(display) _Text.text = "Swap to " + name;
        _UI.gameObject.SetActive(display);
    }

    public void Swap(SwapElement elementToSwap)
    {

        switch (elementToSwap.Type)
        {
            case ElementType.Arm:
                _currentArm = elementToSwap;
                Player.OnChangeArm(_currentArm.Prefab.GetComponent<Arm>());
                break;
            case ElementType.Projectile:
                _currentProjectile = elementToSwap;
                Player.OnChangeProjectile(_currentProjectile.Prefab.GetComponent<Projectile>().ClassPower);
                break;
        }

        foreach (var element in _swapElements)
            element.gameObject.SetActive(element != _currentArm && element != _currentProjectile);
    }
}
