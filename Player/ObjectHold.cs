using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHold : MonoBehaviour
{
    public List<GameObject> Objects; // Toplanan nesneleri tutan liste
    public Transform PlayerTransform; // Oyuncunun transformunu referans al�r
    public float range = 3f; // Nesneleri alg�lamak i�in ���n menzili
    public float Go = 100f; // Sa�lanan kod i�inde kullan�lmaz
    public Camera Camera; // Kameraya referans al�r

    void Start()
    {
        // �htiya� duyulursa ba�latma kodlar� buraya gelebilir
    }

    void Update()
    {
        // Nesneleri toplamak i�in fare d��mesine bas�l�p bas�lmad���n� kontrol et
        if (Input.GetMouseButtonDown(0))
        {
            StartPickUp();
        }

        // Nesneleri b�rakmak i�in fare d��mesinin b�rak�l�p b�rak�lmad���n� kontrol et
        if (Input.GetMouseButtonUp(0))
        {
            Drop();
        }
    }

    void StartPickUp()
    {
        // Kameran�n konumundan ba�layarak ileri do�ru bir ���n g�nder
        RaycastHit hit;
        if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hit, range))
        {
            // Debug.Log(hit.transform.name); // Hata ay�klama i�in vurulan nesnenin ad�n� kaydet

            // Vurulan nesnenin "Tasinabilir_Kutular" etiketine sahip olup olmad���n� kontrol et
            if (hit.transform.CompareTag("Tasinabilir_Kutular"))
            {
                // Etiket do�ru ise, vurulan nesneyi i�eren PickUp metodunu �a��r
                PickUp(hit.transform.gameObject);
            }
        }
    }

    void PickUp(GameObject obj)
    {
        // Nesnenin Rigidbody'sini kinematik yaparak yerinde tut
        obj.GetComponent<Rigidbody>().isKinematic = true;

        // Nesneyi oyuncunun ebeveyni olarak ayarla, b�ylece onunla birlikte hareket eder
        obj.transform.SetParent(PlayerTransform);

        // Nesneyi toplanan nesneler listesine ekle
        Objects.Add(obj);
    }

    void Drop()
    {
        // Toplanan nesneler listesinde dola�
        foreach (GameObject obj in Objects)
        {
            // Nesneyi oyuncudan ��karmak i�in ebeveyni null yap
            obj.transform.parent = null;

            // Nesnenin Rigidbody'sini etkilemek i�in kinematik yapma
            obj.GetComponent<Rigidbody>().isKinematic = false;
        }

        // Nesneleri b�rakt�ktan sonra toplanan nesneler listesini temizle
        Objects.Clear();
    }
}
