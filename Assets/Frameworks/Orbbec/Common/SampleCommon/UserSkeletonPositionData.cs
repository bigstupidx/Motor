using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Orbbec;

using OrbbecGestures;

public class UserSkeletonPositionData : MonoBehaviour
{
	static Color[] ballColors = new Color[] { Color.red, Color.green, Color.blue, Color.yellow };
	public static UserSkeletonPositionData Create(int Index, Camera renderCamera)
	{
		if (OrbbecManager.Instance == null)
			return null;

		if (!OrbbecManager.Instance.IsInited)
			return null;

		GameObject parentObj = new GameObject(string.Format("User {0} data", Index));

		UserSkeletonPositionData data = parentObj.AddComponent<UserSkeletonPositionData>();

		SkeletonType[] SkeletonTypeArray = OrbbecManager.Instance.GetAvailableJointArray();

		data.BallObjs = new GameObject[SkeletonTypeArray.Length];
		data.BallMeshRenderers = new MeshRenderer[SkeletonTypeArray.Length];

		for (int i = 0; i < data.BallObjs.Length; ++i)
		{
			GameObject ballObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			ballObj.name = SkeletonTypeArray[i].ToString();

			MeshRenderer refMeshRenderer = ballObj.GetComponent<MeshRenderer>();

			if (refMeshRenderer != null && refMeshRenderer.material != null)
				refMeshRenderer.material.color = ballColors[Index % ballColors.Length];

			ballObj.transform.parent = data.transform;
			ballObj.transform.localPosition = Vector3.zero;
			ballObj.transform.localScale = Vector3.one * 0.25f;

			data.BallObjs[i] = ballObj;
			data.BallMeshRenderers[i] = refMeshRenderer;
		}

		data.renderCamera = renderCamera;
		data.m_Index = Index;

		return data;
	}

	int m_Index = 0;

	public float[] ScreenOffset		= new float[2];
	public float[] ResolutionScale	= new float[2];

	int DepthXSize = 0;
	int DepthYSize = 0;
	int DepthZSize = 0;

	public void OnStart()
	{
		
	}

	public void SetUser(OrbbecUser user)
	{
		User = user;
		if (User == null
			|| !User.IsInConfidence())
		{
			gameObject.SetActive(false);
			return;
		}

		gameObject.SetActive(true);
	}

	Color Temp = new Color();
	void Update()
	{
		if (OrbbecManager.Instance == null)
			return;

		OrbbecManager.Instance.GetDepthSize(out DepthXSize, out DepthYSize, out DepthZSize);

		if (User == null
		|| !User.IsInConfidence()
		|| renderCamera == null)
		{
			gameObject.SetActive(false);
			return;
		}

		for (int i = 0; i < BallObjs.Length; ++i)
		{
			
			Vector3 pos = User.BoneScreenPos[i];

			pos.x = pos.x / DepthXSize;
			pos.y = (DepthYSize - pos.y) / DepthYSize;
			pos.z = 0.0f;

			pos.x += ScreenOffset[0];
			pos.y += ScreenOffset[1];
			pos.x *= ResolutionScale[0];
			pos.y *= ResolutionScale[1];

			if (	float.IsInfinity(pos.x) ||float.IsNaN(pos.x)
				||	float.IsInfinity(pos.y) ||float.IsNaN(pos.y))
			{
				continue;
			}
				
			try
			{
				Vector3 worldPos = renderCamera.ViewportToWorldPoint(pos);

				worldPos.z = 0.0f;

				BallObjs[i].transform.position = worldPos;

				if (GestureManager.Instance != null && GestureManager.Instance.Param.IsGetVelocityData)
				{
					Point3D Velocity = GestureManager.Instance.BindPlayers[m_Index].SkeletonVelocityInfo[i];
					Temp.r = Velocity.x / 5.0f + 0.5f;
					Temp.g = Velocity.y / 5.0f + 0.5f;
					Temp.b = Velocity.z / 5.0f + 0.5f;
					Temp.a = 1.0f;
					BallMeshRenderers[i].material.color = Temp;
						//
				}
			}
			catch(System.Exception ex)
			{
				continue;//Log.Print(Log.Level.Error, ex.Message);
			}
			
		}
	}

	public OrbbecUser User = null;
	Camera renderCamera = null;
	public GameObject[] BallObjs = null;
	public MeshRenderer[] BallMeshRenderers = null;

}