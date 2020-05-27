using UnityEngine;

public class ZombiAct : MonoBehaviour, ITick
{
    private void Awake()
    {
        ManagerUpdate.AddTo(this);
    }

    public void Tick()
    {
        Debug.Log("Hello World!");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Toolbox.Get<ManagerEvents>().CreatePrefab(Random.insideUnitSphere * Random.Range(-10, 10));
        }
    }
}
