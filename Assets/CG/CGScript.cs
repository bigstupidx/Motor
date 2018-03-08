using UnityEngine;
using System.Collections;

public class CGScript : MonoBehaviour
{
	public void CGEnd()
    {
	    new GameObject("HomeKeyEvent", typeof(HomeKeyEvent));
        Application.LoadLevel(1);
	}
}
