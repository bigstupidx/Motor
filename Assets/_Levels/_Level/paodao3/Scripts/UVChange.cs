using UnityEngine;
using System.Collections;

public class UVChange : MonoBehaviour
{

	public float Rate;
	public Vector2 Offset;

	public string PropertyName = "_MainTex";
	public Renderer Renderer;
	public bool SharedMaterial = true;

	private Vector2 _offset;
	private float _timer = 0f;
	private int _flag = 1;

	void Awake() {
		if (this.Renderer == null)
		{
			this.Renderer = GetComponent<Renderer>();
		}
		_timer = 0f;
		_flag = 1;
	}

	// Update is called once per frame
	void Update()
	{
		_timer += Time.deltaTime;
		if (_timer > Rate)
		{
			Material mat;
#if UNITY_EDITOR
			mat = this.Renderer.material;
#else
		mat = this.SharedMaterial ? this.Renderer.sharedMaterial : this.Renderer.material;
#endif
			this._offset += _flag* Offset;
			mat.SetTextureOffset(this.PropertyName, this._offset);
			_flag *= -1;
			_timer = 0f;
		}


	}
}
