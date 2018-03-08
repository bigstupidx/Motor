using UnityEngine;

public class ScreenClickTest : MonoBehaviour
{
	public Camera Cam;

	void Start () {
	
	}

	void Update ()
	{
		if (Cam == null) return;

		var worldPos = Cam.ScreenToWorldPoint(Input.mousePosition);
        var viewPos = Cam.ScreenToViewportPoint(Input.mousePosition);

        //var rate = Screen.width * 1f / Screen.height;
		//var	pos = worldPos/rate* (16f / 9f);

        var pos = Cam.ViewportToWorldPoint(viewPos);

		if (Input.GetMouseButtonDown(0))
		{
			Debug.Log("<Color=red>[Screen Test]</Color> Mouse :" 
				+ Input.mousePosition + "\t World : " + worldPos.ToString("f2") + " \t " + pos.ToString("f2") + "\t View : " + viewPos.ToString("f4"));
		} 
	}
}
