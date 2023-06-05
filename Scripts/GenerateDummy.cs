using System.Collections;
using UnityEngine;

public class GenerateDummy : MonoBehaviour
{
    public GameObject dummy;
    public GameObject dummyPrefab;
    Vector3 position = new Vector3(-5, 0, 0);
    Quaternion rotation = Quaternion.Euler(90f, 0, 0);
    bool isCoroutineActived = false;
    private void Update()
    {
        dummy = GameObject.FindWithTag("Dummy");
        if (dummy == null && !isCoroutineActived)
        {
            StartCoroutine(SpawnDummy());
        }
    }

    private IEnumerator SpawnDummy()
    {
        isCoroutineActived = true;
        yield return new WaitForSeconds(5);
        Debug.Log("Generando nuevo Dummy");
        Instantiate(dummyPrefab, position, rotation);
        isCoroutineActived = false;
    }
}
