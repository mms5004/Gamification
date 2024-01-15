using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum ElementType
{
    Arm, Projectile
}

public class SwapElement : MonoBehaviour
{
    public string Name;
    public ElementType Type;
    public GameObject Prefab;
    private SwapManager _swapElementManager;
    private bool _canSwap;

    public void Initialize(SwapManager inSwapManager)
    {
        _swapElementManager = inSwapManager;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        _canSwap = true;
        _swapElementManager.ToggleDisplayUI(_canSwap, Name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player") return;

        _canSwap = false;
        _swapElementManager.ToggleDisplayUI(_canSwap, "");
    }

    void Update()
    {
        if (!_canSwap) return;

        if (!Input.GetKeyDown(KeyCode.E)) return;

        _canSwap = false;
        _swapElementManager.ToggleDisplayUI(_canSwap, "");
        _swapElementManager.Swap(this);
    }
}
