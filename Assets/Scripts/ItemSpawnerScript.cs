using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSpawnerScript : MonoBehaviour
{
	public List<GameObject> LgoItemTypes;
	public GameObject goHealth;
	public GameObject goCharge;
	public GameObject goInstantCharge;

	// Use this for initialization
	void Start ()
	{
		LoadResources ();
		GenerateItemTypeList ();
		int i = (int)Random.Range (0.0f, LgoItemTypes.Count - 1);
		Instantiate (LgoItemTypes [i], this.gameObject.transform.position + Vector3.up, Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void LoadResources()
	{
		goHealth = (GameObject)Resources.Load("Prefabs/Health");
		goCharge = (GameObject)Resources.Load("Prefabs/Charge");
		goInstantCharge = (GameObject)Resources.Load("Prefabs/InstantCharge");
	}

	void GenerateItemTypeList()
	{
		LgoItemTypes.Add (goHealth);
		LgoItemTypes.Add (goCharge);
		LgoItemTypes.Add (goInstantCharge);
	}
}
