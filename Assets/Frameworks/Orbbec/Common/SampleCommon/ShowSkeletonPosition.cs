using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using Orbbec;

public class ShowSkeletonPosition : MonoBehaviour 
{
	public UserSkeletonPositionData[] UserDatas = null;

	public float[] realResolution = new float[2];

	public RawImage RealWorldImage = null;

	bool IsUserDataInited = false;

	public Camera SkeletonCamera = null;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (OrbbecManager.Instance == null)
			return;

		if (!OrbbecManager.Instance.IsInited)
		{
			return;
		}

		if (!IsUserDataInited)
		{
			UserDatas = new UserSkeletonPositionData[OrbbecManager.Instance.MaxTrackingUserNum];

			if (RealWorldImage == null 
				|| SkeletonCamera == null)
			{
				return;
			}

			for (int i = 0; i < UserDatas.Length; ++i)
			{
				UserDatas[i] = UserSkeletonPositionData.Create(i, SkeletonCamera);
				UserDatas[i].gameObject.SetActive(false);

				UserDatas[i].transform.position = Vector3.zero;

				UserDatas[i].ScreenOffset[0] = RealWorldImage.rectTransform.rect.center.x / (0.5f * RealWorldImage.canvas.rootCanvas.pixelRect.width / RealWorldImage.canvas.scaleFactor);
				UserDatas[i].ScreenOffset[1] = RealWorldImage.rectTransform.rect.center.y / (0.5f * RealWorldImage.canvas.rootCanvas.pixelRect.height / RealWorldImage.canvas.scaleFactor);

				UserDatas[i].ResolutionScale[0] = RealWorldImage.rectTransform.rect.width  / RealWorldImage.canvas.rootCanvas.pixelRect.width * RealWorldImage.canvas.scaleFactor;
				UserDatas[i].ResolutionScale[1] = RealWorldImage.rectTransform.rect.height / RealWorldImage.canvas.rootCanvas.pixelRect.height * RealWorldImage.canvas.scaleFactor;
			}

			IsUserDataInited = true;
		}
		
	}
}
