using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ManagerEvents",menuName = "Managers/ManagerEvents")]
public class ManagerEvents : ManagerBase
{


	public GameObject prefab;
 
	public GameObject CreatePrefab(Vector3 pos)
	{
		return Instantiate(prefab, pos, Quaternion.identity);
	}


}
