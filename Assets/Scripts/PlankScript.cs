///<author> Graham Alexander MacDonald 19/03/2015 </author>

using UnityEngine;
using System.Collections;

public class PlankScript : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnTriggerEnter(Collider c)
	{
		if (c.tag == "Collider")
		{
			this.gameObject.transform.GetComponent<Rigidbody>().useGravity = false;
			this.gameObject.GetComponent<Collider>().isTrigger = false;
			this.GetComponent<Rigidbody>().velocity = Vector3.zero;
			this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		}
	}
}
