using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Orbbec
{
	public class Demo : MonoBehaviour {

		public Button startBtn = null;

		public Button topBtn = null;
		public Button bottomBtn = null;
		public Button leftBtn = null;
		public Button rightBtn = null;

		public Text infoTxt = null;

		// Use this for initialization
		void Start () {
			startBtn.onClick.AddListener (() =>
			{
				OrbbecSensingManager.Init();
				OrbbecSensingManager.instance.showTrackingUI = true;
				OrbbecSensingManager.instance.playerMode = OrbbecSensingManager.PlayerMode.single;

				OrbbecSensingManager.instance.deviceInitAction = OnDeviceInit;
				OrbbecSensingManager.instance.trackedAction = OnTracked;
				OrbbecSensingManager.instance.unTrackedAction = OnUnTrackedAction;
				OrbbecSensingManager.instance.leftAtkAction = OnLeftAtkAction;
				OrbbecSensingManager.instance.rightAtkAction = OnRightAtkAction;
				OrbbecSensingManager.instance.leftHandRaiseAction = OnLeftHandRaiseAction;
				OrbbecSensingManager.instance.rightHandRaiseAction = OnRightHandRaiseAction;

				OrbbecSensingManager.instance.InitOrbbecDevice();
			});
		}
		
		// Update is called once per frame
		void Update ()
		{
			topBtn.OnDeselect (null);
			bottomBtn.OnDeselect (null);
			leftBtn.OnDeselect (null);
			rightBtn.OnDeselect (null);
			topBtn.GetComponentInChildren<Text> ().text = string.Empty;
			bottomBtn.GetComponentInChildren<Text> ().text = string.Empty;
			leftBtn.GetComponentInChildren<Text> ().text = string.Empty;
			rightBtn.GetComponentInChildren<Text> ().text = string.Empty;

			if (OrbbecSensingManager.instance == null || !OrbbecSensingManager.instance.isActive)
				return;


			if (OrbbecSensingManager.instance.verticalValue > 0.0f)
			{
				topBtn.Select ();
				topBtn.GetComponentInChildren<Text> ().text = string.Format ("{0:0.00}", OrbbecSensingManager.instance.verticalValue);
			}
			else if (OrbbecSensingManager.instance.verticalValue < 0.0f)
			{
				bottomBtn.Select ();
				bottomBtn.GetComponentInChildren<Text> ().text = string.Format ("{0:0.00}", OrbbecSensingManager.instance.verticalValue);
			}

			if (OrbbecSensingManager.instance.horizontalValue > 0.0f)
			{
				rightBtn.Select ();
				rightBtn.GetComponentInChildren<Text> ().text = string.Format ("{0:0.00}", OrbbecSensingManager.instance.horizontalValue);
			}
			else if (OrbbecSensingManager.instance.horizontalValue < 0.0f)
			{
				leftBtn.Select ();
				leftBtn.GetComponentInChildren<Text> ().text = string.Format ("{0:0.00}", OrbbecSensingManager.instance.horizontalValue);
			}

			//是否丢失标定也可以通过 OrbbecSensingManager.instance.isActive 是否为true来判断

		}

		List<string> infoList = new List<string>{"info:"};

		//设备初始化完成
		void OnDeviceInit()
		{
			if (infoList.Count >= 10)
			{
				infoList.RemoveAt (0);
			}
			infoList.Add ("OnDeviceInit");
			RefreshInfoTxt ();
		}

		//标定成功回调
		void OnTracked()
		{
			if (infoList.Count >= 10)
			{
				infoList.RemoveAt (0);
			}
			infoList.Add ("OnTracked");
			RefreshInfoTxt ();
		}

		//标定丢失回调
		void OnUnTrackedAction()
		{
			if (infoList.Count >= 10)
			{
				infoList.RemoveAt (0);
			}
			infoList.Add ("OnUnTrackedAction");
			RefreshInfoTxt ();
		}

		//左拳回调
		void OnLeftAtkAction()
		{
			if (infoList.Count >= 10)
			{
				infoList.RemoveAt (0);
			}
			infoList.Add ("OnLeftAtkAction");
			RefreshInfoTxt ();
		}

		//右拳回调
		void OnRightAtkAction()
		{
			if (infoList.Count >= 10)
			{
				infoList.RemoveAt (0);
			}
			infoList.Add ("OnRightAtkAction");
			RefreshInfoTxt ();
		}

		void OnLeftHandRaiseAction(){
			if (infoList.Count >= 10)
			{
				infoList.RemoveAt (0);
			}
			infoList.Add ("OnLeftHandRaiseAction");
			RefreshInfoTxt ();
		}

		void OnRightHandRaiseAction(){
			if (infoList.Count >= 10)
			{
				infoList.RemoveAt (0);
			}
			infoList.Add ("OnRightHandRaiseAction");
			RefreshInfoTxt ();
		}

		//
		void RefreshInfoTxt()
		{
			infoTxt.text = "";
			for (int i = 0; i < infoList.Count; i++)
			{
				infoTxt.text += infoList[i];
				if (i != infoList.Count - 1)
				{
					infoTxt.text += "\n";
				}
			}
		}
	}
}