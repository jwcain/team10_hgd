using UnityEngine;
using System.Collections;

public class MapInfo : MonoBehaviour {

	public float timeToFinish;
	public int mapMoney;
	public Color backColor;
	public Color particleColor1;
	public Color particleColor2;
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
