///<author> Graham Alexander MacDonald 04/02/2015 </author>

using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
    public MobileInputScript mis;
    public PlayerMovementScript pms;
    public PlayerOnGroundScript pogs;
    public PlayerInventoryScript pis;
    public PlayerCombatScript pcs;
    public PlayerGUIScript pgs;

    public LevelGeneratorScript lgs;

	void Awake()
	{
		LinkComponents();
	}

    /// <summary> Use this for initialization </summary>
	void Start ()
    {
		//LinkComponents();
	}

    /// <summary> Links components </summary>
    void LinkComponents()
    {
        mis = this.gameObject.GetComponent<MobileInputScript>();
        pms = this.gameObject.GetComponent<PlayerMovementScript>();
        pogs = this.gameObject.transform.Find("OnGroundController").GetComponent<PlayerOnGroundScript>();
        pis = this.gameObject.GetComponent<PlayerInventoryScript>();
        pcs = this.gameObject.GetComponent<PlayerCombatScript>();
        pgs = this.gameObject.GetComponent<PlayerGUIScript>();

		lgs = GameObject.Find("LevelGenerator").GetComponent<LevelGeneratorScript>();
    }
}
