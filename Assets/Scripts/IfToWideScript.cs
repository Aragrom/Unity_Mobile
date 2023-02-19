using UnityEngine;
using System.Collections;

public class IfToWideScript : MonoBehaviour
{
	public GameObject goHouse1;
	public GameObject goSub1;

	public bool bBuild;

	// Use this for initialization
	void Start ()
	{
		LoadResources ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (bBuild)
		{
			BuildPlatform();
			Destroy(this.gameObject);
		}
	}

	void LoadResources()
	{
		goHouse1 = (GameObject)Resources.Load ("Prefabs/House1");
		goSub1 = (GameObject)Resources.Load ("Prefabs/Sub1");

	}

	void BuildPlatform()
	{
		Vector3 v3BuildPosition = this.transform.position;
		int iBuildHeight = (int)v3BuildPosition.y / 5;

		int i = iBuildHeight;
		while(i >= 0)
		{
			if(i == iBuildHeight) 
			{
				Instantiate(goSub1, v3BuildPosition, Quaternion.identity);
				v3BuildPosition.y -= 0.01f;
				v3BuildPosition.y -= 2.5f;
			}
			else
			{
				Instantiate(goHouse1, v3BuildPosition, Quaternion.identity);
				v3BuildPosition.y -= 5.0f;
			}

			i--;
		}
	}
}
