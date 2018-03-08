using UnityEngine;
using System.Collections.Generic;
using HeavyDutyInspector;

namespace Game {
	public class TimeCheckerManager : MonoBehaviour {
		public WaypointManager WaypointManager;

		public List<TimeCheckerSpawner> TimeCheckPointList = new List<TimeCheckerSpawner>();

		[SerializeField] [HideInInspector] private bool _isInstance;

		void Start() {
			if (RaceManager.Ins.RaceMode != RaceMode.Timing) {
				Destroy(gameObject);
//				gameObject.SetActive(false);
			}
		}

		public List<TimeCheckPoint> GetCheckPointList() {
			var list = new List<TimeCheckPoint>();
			foreach (var checker in TimeCheckPointList) {
				list.Add(checker.Checker);
			}
			return list;
		}

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
				var spawner = child.GetComponent<TimeCheckerSpawner>();
				if (spawner != null) spawner.DestoryChecker();
				else {
					var ins = new GameObject("TimeCheckerSpawner").transform;
					ins.position = child.position;
					ins.rotation = child.rotation;
					ins.SetParent(transform);
					ins.gameObject.GetOrAddComponent<TimeCheckerSpawner>();
					DestroyImmediate(child.gameObject);
				}
			}

			_isInstance = false;
		}

		[SerializeField]
		[Button("生成 检查点", "__ConvertToPrefab")]
		private bool __btn_convertToPrefab;
		void __ConvertToPrefab() {
			List<Transform> childList = new List<Transform>();
			foreach (Transform child in transform) {
				childList.Add(child);
			}
			foreach (Transform child in childList) {
				var spawner = child.GetComponent<TimeCheckerSpawner>();
				if (spawner != null) spawner.Spawn();
			}
			//__Align10();

			_isInstance = true;
		}


		[SerializeField]
		[Button("对其地面15米并匹配路径点 (时间检查点)", "__Align10")]
		private bool __btn_Align10;

		private void __Align10() {
			if (WaypointManager == null) {
				Debug.LogError("WaypointManager is null !!!");
				return;
			}
			TimeCheckPointList.Clear();
			foreach (Transform child in transform) {
				var node = WaypointManager.FindNearestNode(child.position);
				child.position = WaypointMath.AttachRoadPoint(node.Position) + Vector3.up * 15f;
				child.rotation = Quaternion.LookRotation(node.NextPoint.transform.position - node.transform.position);
				var checker = child.GetComponent<TimeCheckerSpawner>();
				if (checker != null) {
					checker.WayPoint = node;
					TimeCheckPointList.Add(checker);
				}
			}
		}

		private void OnDrawGizmos() {
			if (this.ShowGizmos) {
				Gizmos.color = this.GizmosColor;
				foreach (Transform child in transform) {
					Gizmos.DrawCube(child.position, Vector3.one * 2);
				}
				Gizmos.color = Color.white;
			}
		}
#endif

		void Awake() {//TODO 考虑协程
					  //StopWatchUtil.Start(gameObject.name);
					  // if(_isInstance) return;
					  //			__ConvertToTransform();
					  //			__ConvertToPrefab();
					  //			__Align10();
					  //			foreach (Transform child in transform)
					  //			{
					  //				GameObject ins = (GameObject)Instantiate(this.Prefab.Prefab, child, false);
					  //				ins.transform.localPosition = Vector3.zero;
					  //				ins.transform.localRotation = Quaternion.identity;
					  //			}
			foreach (var spawner in TimeCheckPointList) {
				spawner.Spawn();
			}
			//StopWatchUtil.Stop(gameObject.name);

		}

		public void HideTimeChecker() {
			this.gameObject.SetActive(false);
		}
	}

}
