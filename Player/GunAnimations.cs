using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimations : MonoBehaviour
{
    // Animator bile�enini tan�mla
    private Animator animator;

    // Katana'n�n durumunu kontrol eden bool de�i�ken
    public bool Katana;

    void Start()
    {
        // Animator bile�enini al
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // E�er Katana de�i�keni true ise ve sol fare t�k�na bas�ld�ysa animasyonu tetikle
        if (Katana && Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Katana_Hit_Animation");
        }
    }
}
