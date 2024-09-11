using System.Collections;
using UnityEngine;

public class RandomItemSpawner : MonoBehaviour
{
    public GameObject[] prefabs;  // Spawn edilecek prefabs listesi
    public GameObject targetObject;  // Üzerinde spawn yapılacak GameObject

    private GameObject currentSpawnedObject;

    void Start()
    {
        SpawnObjectOnObjectSurface();
    }

    void SpawnObjectOnObjectSurface()
    {
        // Scriptin bulunduğu nesnenin X ve Z pozisyonunu al
        float xPos = transform.position.x;
        float zPos = transform.position.z;

        // Bu X ve Z pozisyonundaki targetObject yüzeyinin Y yüksekliğini al
        float yPos = targetObject.transform.position.y;

        // Nesneyi bu pozisyonda (X, Y, Z) spawn et
        Vector3 spawnPosition = new Vector3(xPos, yPos, zPos);
        currentSpawnedObject = Instantiate(prefabs[Random.Range(0, prefabs.Length)], spawnPosition, transform.rotation, transform);

        // 5 dakika bekle ve kontrol et
        StartCoroutine(WaitAndCheckObject());
    }

    IEnumerator WaitAndCheckObject()
    {
        // 5 dakika bekle
        yield return new WaitForSeconds(300f);

        // Nesne aktif değilse, yeni bir nesne spawn et
        if (currentSpawnedObject != null && !currentSpawnedObject.activeInHierarchy)
        {
            SpawnObjectOnObjectSurface();
        }
    }
}
