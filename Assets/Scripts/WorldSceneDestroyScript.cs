using UnityEngine;
using System.Collections;

public class WorldSceneDestroyScript : MonoBehaviour
{
	void OnTriggerStay(Collider c)
	{
		if (c.transform.position.x > this.transform.position.x)
			Destroy (this.gameObject);
	}
}
