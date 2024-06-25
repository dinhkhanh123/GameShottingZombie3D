using UnityEngine;
using System.Collections;

public class WFX_Demo_DeleteAfterDelay : MonoBehaviour
{
	private float delay = 0.6f;
	
	void Update ()
	{
		delay -= Time.deltaTime;
		if(delay < 0f)
			GameObject.Destroy(this.gameObject);
	}
}
