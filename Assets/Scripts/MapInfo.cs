using UnityEngine;
using System.Collections;

public class MapInfo : MonoBehaviour {

	public float timeToFinish;
	public int mapMoney;
	public Transform startLocation;
	public GameObject endFlag;

	void Awake()
	{
		if (!startLocation && transform.FindChild ("StartLocation"))
			startLocation = transform.FindChild ("StartLocation");
		if (!endFlag && transform.FindChild ("EndFlag"))
			endFlag = transform.FindChild ("EndFlag").gameObject;
	}


}
