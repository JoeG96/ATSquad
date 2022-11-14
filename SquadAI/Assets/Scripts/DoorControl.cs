using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{

    [System.NonSerialized] GameObject[] collectables;
    [SerializeField] GameObject[] enemies;

    private bool doorOpen = false;

    private void Awake()
    {
        collectables = GameObject.FindGameObjectsWithTag("Collectable");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    private void Update()
    {
        collectables = GameObject.FindGameObjectsWithTag("Collectable");
        //Debug.Log("Collectables Length = " + collectables.Length);
        if (collectables.Length <= 0 && doorOpen == false)
        {
            OpenDoor();

        }
    }

    public void OpenDoor()
    {
        doorOpen = true;
        transform.position = transform.position + new Vector3(0, 10, 0);

    }
}
 