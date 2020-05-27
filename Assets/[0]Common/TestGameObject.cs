using System.Collections.Specialized;
using System.Collections.Generic;
using UnityEngine;

public class TestGameObject : MonoBehaviour, ITick
{
    private float movementSpeed = 0.05f;
    private bool note = true; 

    private List<GameObject> test = new List<GameObject>();

    private void Awake()
    {
        ManagerUpdate.AddTo(this);
    }

    public void Tick()
    {
        if(note)
        {
            Debug.Log("Press Space, Keypad 2, 8, 5, 6, 7, 1");
            note = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Press Space");

            test.Add(Toolbox.Get<ManagerEvents>().CreatePrefab(Random.insideUnitSphere * Random.Range(-10, 10)));
        }

        if (Input.GetKey(KeyCode.Keypad4) 
            || Input.GetKey(KeyCode.Keypad6)
            || Input.GetKey(KeyCode.Keypad8)
            || Input.GetKey(KeyCode.Keypad2)
            || Input.GetKey(KeyCode.Keypad7)
            || Input.GetKey(KeyCode.Keypad1))
        {


            Debug.Log("Press Keypad");
            Debug.Log(Toolbox.Get<ManagerEvents>().prefab.transform.position);
            foreach (var e in test)
                e.transform.position = e.transform.position 
                    + new Vector3(
                        0 + (Input.GetKey(KeyCode.Keypad6) ? movementSpeed : Input.GetKey(KeyCode.Keypad4) ? -movementSpeed : 0),
                        0 + (Input.GetKey(KeyCode.Keypad7) ? movementSpeed : Input.GetKey(KeyCode.Keypad1) ? -movementSpeed : 0),
                        0 + (Input.GetKey(KeyCode.Keypad8) ? movementSpeed : Input.GetKey(KeyCode.Keypad2) ? -movementSpeed : 0));
        }
    }
}
