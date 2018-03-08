using System.Collections.Generic;
using HeavyDutyInspector;
using LTHUtility;
using UnityEngine;

namespace Game {
	public class PropSpawner : MonoBehaviour {

		public NormalPrefab Prefab;


#if UNITY_EDITOR
		public bool ShowGizmos = true;
		public Color GizmosColor = Color.magenta;

		[SerializeField]
		[Button("转为标记点", "__ConvertToTransform")]
		private bool __btn_convertToTransfrom;
		void __ConvertToTransform() {

			List<Transform> childList = new List<Transform>();
			foreach (Transform child in transform) {
				childList.Add(child);
			}
			foreach (Transform child in childList) {
				Transform newTrans = new GameObject("Pos").transform;
				newTrans.SetParent(transform);
				newTrans.position = child.position;
				newTrans.rotation = child.rotation;
				DestroyImmediate(child.gameObject);
			}
		}

		[SerializeField]
		[Button("转为物体", "__ConvertToPrefab")]
		private bool __btn_convertToPrefab;
		void __ConvertToPrefab() {
			List<Transform> childList = new List<Transform>();
			foreach (Transform child in transform) {
				childList.Add(child);
			}
			foreach (Transform child in childList) {
				GameObject ins = (GameObject)Instantiate(this.Prefab.Prefab, this.transform, false);
				ins.transform.position = child.position;
				ins.transform.rotation = child.rotation;

				DestroyImmediate(child.gameObject);
			}
		}

		[SerializeField]
		[Button("对其地面10米 (时间检查点)", "__Align3")]
		private bool __btn_Align3;
		void __Align3() {
			foreach (Transform child in transform)
			{
				child.position = WaypointMath.AttachRoadPoint(child.position) + Vector3.up * 10f;
			}
		}

		[SerializeField]
		[Button("对其地面2米 (道具金币)", "__Align2")]
		private bool __btn_Align2;
		void __Align2() {
			foreach (Transform child in transform) {
				child.position = WaypointMath.AttachRoadPoint(child.position) + Vector3.up * 2f;
			}
		}

		[SerializeField]
		[Button("对其地面1米 (道具金币)", "__Align1")]
		private bool __btn_Align1;
		void __Align1() {
			foreach (Transform child in transform) {
				child.position = WaypointMath.AttachRoadPoint(child.position) + Vector3.up * 1f;
			}
		}

		[SerializeField]
		[Button("对其地面0.2米 (加速带)", "__Align0_2")]
		private bool __btn_Align0_2;
		void __Align0_2() {
			foreach (Transform child in transform) {
				child.position = WaypointMath.AttachRoadPoint(child.position) + Vector3.up * 0.2f;
			}
		}

        [SerializeField]
        [Button("对其地面4米 (封闭岔路)", "__Align4")]
        private bool __btn_Align4;
        void __Align4()
        {
            foreach (Transform child in transform)
            {
                child.position = WaypointMath.AttachRoadPoint(child.position) + Vector3.up * 4f;
            }
        }

        void OnDrawGizmos() {
			if (this.ShowGizmos) {
				Gizmos.color=this.GizmosColor;
				foreach (Transform child in transform) {
					Gizmos.DrawCube(child.position, Vector3.one * 2);
				}
				Gizmos.color=Color.white;
			}
		}



#endif


		void Awake() {//TODO 考虑协程
			StopWatchUtil.Start(gameObject.name);
			foreach (Transform child in transform) {
				GameObject ins = (GameObject)Instantiate(this.Prefab.Prefab, child, false);
				ins.transform.localPosition = Vector3.zero;
				ins.transform.localRotation = Quaternion.identity;
			}
			StopWatchUtil.Stop(gameObject.name);
		}





	}
}