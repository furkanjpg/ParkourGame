using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimations : MonoBehaviour
{
    // Animator bileþenini tanýmla
    private Animator animator;

    // Katana'nýn durumunu kontrol eden bool deðiþken
    public bool Katana;

    void Start()
    {
        // Animator bileþenini al
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Eðer Katana deðiþkeni true ise ve sol fare týkýna basýldýysa animasyonu tetikle
        if (Katana && Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Katana_Hit_Animation");
        }
    }
}
