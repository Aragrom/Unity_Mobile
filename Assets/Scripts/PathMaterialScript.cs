using UnityEngine;
using System.Collections;

public class PathMaterialScript : MonoBehaviour
{
	public Material matStone1x1;
	public Material matStone2x1;
	public Material matStone3x1;

	// Use this for initialization
	void Start ()
	{
		LoadResources ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void LoadResources()
	{
		matStone1x1 = (Material)Resources.Load ("Material/Stone1x1");
		matStone2x1 = (Material)Resources.Load ("Material/Stone2x1");
		matStone3x1 = (Material)Resources.Load ("Material/Stone3x1");
	}
}
