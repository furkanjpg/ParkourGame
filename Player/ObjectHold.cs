using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHold : MonoBehaviour
{
    public List<GameObject> Objects; // Toplanan nesneleri tutan liste
    public Transform PlayerTransform; // Oyuncunun transformunu referans alýr
    public float range = 3f; // Nesneleri algýlamak için ýþýn menzili
    public float Go = 100f; // Saðlanan kod içinde kullanýlmaz
    public Camera Camera; // Kameraya referans alýr

    void Start()
    {
        // Ýhtiyaç duyulursa baþlatma kodlarý buraya gelebilir
    }

    void Update()
    {
        // Nesneleri toplamak için fare düðmesine basýlýp basýlmadýðýný kontrol et
        if (Input.GetMouseButtonDown(0))
        {
            StartPickUp();
        }

        // Nesneleri býrakmak için fare düðmesinin býrakýlýp býrakýlmadýðýný kontrol et
        if (Input.GetMouseButtonUp(0))
        {
            Drop();
        }
    }

    void StartPickUp()
    {
        // Kameranýn konumundan baþlayarak ileri doðru bir ýþýn gönder
        RaycastHit hit;
        if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hit, range))
        {
            // Debug.Log(hit.transform.name); // Hata ayýklama için vurulan nesnenin adýný kaydet

            // Vurulan nesnenin "Tasinabilir_Kutular" etiketine sahip olup olmadýðýný kontrol et
            if (hit.transform.CompareTag("Tasinabilir_Kutular"))
            {
                // Etiket doðru ise, vurulan nesneyi içeren PickUp metodunu çaðýr
                PickUp(hit.transform.gameObject);
            }
        }
    }

    void PickUp(GameObject obj)
    {
        // Nesnenin Rigidbody'sini kinematik yaparak yerinde tut
        obj.GetComponent<Rigidbody>().isKinematic = true;

        // Nesneyi oyuncunun ebeveyni olarak ayarla, böylece onunla birlikte hareket eder
        obj.transform.SetParent(PlayerTransform);

        // Nesneyi toplanan nesneler listesine ekle
        Objects.Add(obj);
    }

    void Drop()
    {
        // Toplanan nesneler listesinde dolaþ
        foreach (GameObject obj in Objects)
        {
            // Nesneyi oyuncudan çýkarmak için ebeveyni null yap
            obj.transform.parent = null;

            // Nesnenin Rigidbody'sini etkilemek için kinematik yapma
            obj.GetComponent<Rigidbody>().isKinematic = false;
        }

        // Nesneleri býraktýktan sonra toplanan nesneler listesini temizle
        Objects.Clear();
    }
}
