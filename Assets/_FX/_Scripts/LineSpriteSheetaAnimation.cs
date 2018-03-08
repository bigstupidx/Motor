using UnityEngine;
using System.Collections;

public class LineSpriteSheetaAnimation : MonoBehaviour {

	public LineRenderer Trail;
	public Vector2 SheetSize;
	public int FPS;
	public bool randomStarting = true;
	
	private float timer;
	private float SPF;
	private Vector2 frame;
	
	void OnEnable () 
	{
		timer = 0;
		SPF = 1f / FPS;
		if (randomStarting)
		{
			frame = new Vector2(Random.Range(0, (int)SheetSize.x), Random.Range(0, (int)SheetSize.y));
		}
		else
			frame = Vector2.zero;
		Trail.material.SetTextureScale("_MainTex", new Vector2(1f / SheetSize.x, 1f / SheetSize.y));
	}
	
	void Update () 
	{
		//if (Trail.Emit)
		{
			timer += Time.deltaTime;
			if (timer >= SPF)
			{
				timer -= SPF;
				if (++frame.x >= SheetSize.x)
				{
					frame.x = 0;
					if (++frame.y >= SheetSize.y)
					{
						frame.y = 0;
					}
				}
				Trail.material.SetTextureOffset("_MainTex", new Vector2(frame.x / SheetSize.x, frame.y / SheetSize.y));
			}
		}
	}
}
