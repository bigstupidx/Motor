using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Game;
using GameClient;
using LTHUtility;
using XPlugin.Update;

namespace GameUI {

	public class ModelShow : Singleton<ModelShow> {
		public enum CameraPos {
			MainMenu, HeroBoard, BikeBoard, PrepareBoard, MotorUpdateBoard, OnlinePrepareBoard, ChampionPrepareBoard, Free
		}

		private CameraPos _currentPos = CameraPos.MainMenu;

		public Camera Camera;
		public Transform CameraPosMainMenu;
		public Transform CameraPosHeroBoard;
		public Transform CameraPosBikeBoard;
		public Transform CameraPoGamePrepareBoard;
		public Transform CameraPosGameOnlineBoard;
		public Transform CameraPosGameChampBoard;
		public Transform CameraPosMotorUpdate;
		public Transform CameraPosFree;
		public Transform BikeShowPos;
		public Transform HeroShowPos;
		public ParticleSystem HeroEffect;
		public ParticleSystem BikeEffect;
		public ParticleSystem FastLvUpEffect;
		private bool isMoving;
		private int nextid;
		public Vector3 leftPosition;
		public Vector3 rightPosition;
		[System.NonSerialized]
		public DriverShow Hero;
		[System.NonSerialized]
		public GameObject Bike;

		private Dictionary<string, DriverShow> HeroList = new Dictionary<string, DriverShow>();
		private Dictionary<string, GameObject> BikeList = new Dictionary<string, GameObject>();

		private Transform targetPos = null;
		private float _lerpT = 0;
		private const float LerpTime = 0.3f;
		private Vector3 _lerpFromPos;
		private Quaternion _lerpFromRot;
		private float _lerpHeroRotT = 0;

		public Vector3 HeroRotOnMenuBoard;
		public Vector3 HeroRotOnHeroBoard;

		public void SetActive(bool active) {
			this.Camera.enabled = active;
			//			ProjectorShadow.ProjectorShadow.Ins.SetEnable (active);
		}


		private void Update() {
			if (Input.GetMouseButtonDown(0)) {
				if (!UIUtility.IsTouchUI()) {
					RaycastHit hit;
					var ray = this.Camera.ScreenPointToRay(Input.mousePosition);
					if (Physics.Raycast(ray, out hit)) {
						if (hit.transform.CompareTag(Tags.Ins.Player)) {
							switch (this._currentPos) {
								case CameraPos.MainMenu:
									HeroBoard.Show(Client.User.UserInfo.ChoosedHeroID);
									break;
								case CameraPos.HeroBoard:
									/*string name = HeroBoard.Ins.CurrentShowHero.Info.Data.Prefab;
                                    ModMenu.Ins.Cover(UICommonItem.TOP_BOARD);
                                    ShowHero(name);
                                    ChangeCameraPos(CameraPos.Free);*/
									break;
								case CameraPos.PrepareBoard:
									HeroBoard.Show(Client.User.UserInfo.ChoosedHeroID);
									break;
							}
						}
						if (hit.transform.CompareTag(Tags.Ins.Bike)) {
							switch (this._currentPos) {
								case CameraPos.MainMenu:
									GarageBoard.Show(Client.User.UserInfo.ChoosedBikeID);
									break;
								case CameraPos.BikeBoard:
									//进入自由观看模式
									string name = GarageBoard.Ins.CurrentShowBike.Info.Data.Prefab;
									ModMenu.Ins.Cover(new string[] { UICommonItem.TOP_BOARD_BACK });
									ShowBike(name);
									ChangeCameraPos(CameraPos.Free);
									break;
								case CameraPos.PrepareBoard:
									GarageBoard.Show(Client.User.UserInfo.ChoosedBikeID);
									break;

							}
						}
					}
				}
			}

			if (this._currentPos == CameraPos.BikeBoard) {
				//if (this.Bike != null) {
				//	this.Bike.transform.Rotate (Vector3.up, 20 * Time.unscaledDeltaTime);
				//}
			}
			if (this.targetPos != null && this._lerpT < LerpTime) {
				this._lerpT += Time.deltaTime;
				this.Camera.transform.rotation = Quaternion.Lerp(this._lerpFromRot, this.targetPos.rotation, this._lerpT / LerpTime);
				this.Camera.transform.position = Vector3.Lerp(this._lerpFromPos, this.targetPos.position, this._lerpT / LerpTime);
			}
			if (this._lerpHeroRotT < LerpTime) {

				this._lerpHeroRotT += Time.deltaTime;
				//this.HeroShowPos.rotation = Quaternion.Lerp (this._heroPosFromRot, this._heroPosTargetRot, this._lerpHeroRotT / LerpTime);
			}
		}

		private Coroutine _moveToFreePos = null;
		public void ChangeCameraPos(CameraPos pos) {
			if (!this.Camera.enabled) {
				SetActive(true);
			}
			this._currentPos = pos;
			if (this._currentPos != CameraPos.Free) {
				if (this.Bike != null) {
					this.Bike.transform.localRotation = Quaternion.identity;
				}
				RotateAroundCamera.Ins.Target = null;
				if (this._moveToFreePos != null) {
					StopCoroutine(this._moveToFreePos);
				}
			} else {
				var changeTime = 0.3f;
				this._moveToFreePos = StartCoroutine(MoveToFreePos(changeTime));
			}
			this.targetPos = null;
			this._lerpT = 0;
			this._lerpHeroRotT = 0;
			this._lerpFromPos = this.Camera.transform.position;
			this._lerpFromRot = this.Camera.transform.rotation;
			switch (pos) {
				case CameraPos.MainMenu:
					this.targetPos = this.CameraPosMainMenu;
					break;
				case CameraPos.HeroBoard:
					this.targetPos = this.CameraPosHeroBoard;
					break;
				case CameraPos.BikeBoard:
					this.targetPos = this.CameraPosBikeBoard;
					break;
				case CameraPos.PrepareBoard:
					this.targetPos = this.CameraPoGamePrepareBoard;
					break;
				case CameraPos.MotorUpdateBoard:
					this.targetPos = this.CameraPosMotorUpdate;
					break;
				case CameraPos.OnlinePrepareBoard:
					this.targetPos = this.CameraPosGameOnlineBoard;
					break;
				case CameraPos.ChampionPrepareBoard:
					this.targetPos = this.CameraPosGameChampBoard;
					break;
				case CameraPos.Free:

					this.targetPos = this.CameraPosFree;
					break;
			}
		}

		IEnumerator MoveToFreePos(float time) {
			yield return new WaitForSeconds(time);
			RotateAroundCamera.Ins.Target = this.BikeShowPos;
			this._moveToFreePos = null;
		}

		public void ShowMainMenuModel(string hero, string bike) {
			ShowBike(bike);
			ShowHero(hero);
		}


		public DriverShow ShowHero(string hero) {
			SetActive(true);
			DriverShow temp;
			this.HeroList.TryGetValue(hero, out temp);
			if (this.Hero != null && temp == this.Hero) {
				return temp;
			}
			if (temp == null) {
				var prefab = UResources.Load<GameObject>(hero + "_show");
				if (prefab == null) {
					Debug.LogError("==>hero prefab not found " + hero);
					return null;
				}
				HideHero();
				this.Hero = ((GameObject)Instantiate(prefab, this.HeroShowPos, false)).GetComponent<DriverShow>();
				this.Hero.gameObject.SetLayerRecursion(Layers.Ins.Player);
				if (!this.HeroList.ContainsKey(hero)) {
					this.HeroList.Add(hero, this.Hero);
				}
				this.Hero.transform.localPosition = Vector3.zero;
			} else {
				HideHero();
				this.Hero = temp;
			}

			if (!this.Hero.gameObject.activeSelf) {
				this.Hero.gameObject.SetActive(true);
			}
			return this.Hero;
		}

		public void HideHero() {
			if (this.Hero != null && this.Hero.gameObject.activeSelf) {
				this.Hero.gameObject.SetActive(false);
				this.Hero = null;
			}
		}

		public void OnHeroClick() {
			if (this.Hero != null) {
				this.Hero.PlayIdleAnim();
				this.Hero.PlayTalkAudio(HeroBoard.Ins.CurrentShowHero.Info.Data.ID);
			}
		}

		public void OnBikeClick() {
			string name = GarageBoard.Ins.CurrentShowBike.Info.Data.Prefab;
			ModMenu.Ins.Cover(new string[] { UICommonItem.TOP_BOARD_BACK });
			ShowBike(name);
			ChangeCameraPos(CameraPos.Free);
		}

		public void ShowBike(string bikeName) {
			SetActive(true);
			GameObject temp;
			this.BikeList.TryGetValue(bikeName, out temp);
			if (this.Bike != null && temp == this.Bike) {
				return;
			}
			if (temp == null) {
				var prefab = UResources.Load<GameObject>(bikeName + "_show");
				if (prefab == null) {
					Debug.LogError("==>bike prefab not found " + bikeName);
					return;
				}
				HideBike();
				this.Bike = (GameObject)Instantiate(prefab, this.BikeShowPos, false);
				this.Bike.SetLayerRecursion(Layers.Ins.Player);
				if (!this.BikeList.ContainsKey(bikeName)) {
					this.BikeList.Add(bikeName, this.Bike);
				}
			} else {
				HideBike();
				this.Bike = temp;
			}
			if (!this.Bike.activeSelf) {
				this.Bike.SetActive(true);
			}
			this.Bike.transform.localPosition = Vector3.zero;
			this.Bike.transform.localRotation = Quaternion.identity;
		}

		public void HideBike() {
			if (this.Bike != null && this.Bike.activeSelf) {
				this.Bike.SetActive(false);
				this.Bike = null;
			}
		}

		public void ShowNext(int CurrentId) {
			if (isMoving) {
				return;
			}
			if (GarageBoard.Ins._data.Count <= 1) { return; }
			isMoving = true;
			SfxManager.Ins.PlayOneShot(SfxType.SFX_Scorll_view);
			this.MoveOutToLeft(this.Bike);
			nextid = CurrentId;
			this.DelayInvoke(MoveNext, 0.2f);
		}

		public void ShowPre(int CurrentId) {
			if (isMoving) {
				return;
			}
			if (GarageBoard.Ins._data.Count <= 1) { return; }
			isMoving = true;
			SfxManager.Ins.PlayOneShot(SfxType.SFX_Scorll_view);
			this.MoveOutToRight(this.Bike);
			nextid = CurrentId;
			this.DelayInvoke(MovePre, 0.2f);
		}

		private void MoveOutToLeft(GameObject motor) {
			motor.GetComponent<TweenPosition>().from = Vector3.zero;
			motor.GetComponent<TweenPosition>().to = this.leftPosition;
			motor.GetComponent<TweenPosition>().ResetToBeginning();
			motor.GetComponent<TweenPosition>().PlayForward();
		}

		private void MoveInFromLeft(GameObject motor) {
			motor.SetActive(true);
			motor.GetComponent<TweenPosition>().from = this.leftPosition;
			motor.GetComponent<TweenPosition>().to = Vector3.zero;
			motor.GetComponent<TweenPosition>().ResetToBeginning();
			motor.GetComponent<TweenPosition>().PlayForward();
		}

		private void MoveOutToRight(GameObject motor) {
			motor.GetComponent<TweenPosition>().from = Vector3.zero;
			motor.GetComponent<TweenPosition>().to = this.rightPosition;
			motor.GetComponent<TweenPosition>().ResetToBeginning();
			motor.GetComponent<TweenPosition>().PlayForward();
		}

		private void MoveInFromRight(GameObject motor) {
			motor.SetActive(true);
			motor.GetComponent<TweenPosition>().from = this.rightPosition;
			motor.GetComponent<TweenPosition>().to = Vector3.zero;
			motor.GetComponent<TweenPosition>().ResetToBeginning();
			motor.GetComponent<TweenPosition>().PlayForward();
		}

		public void ShowUpgradeEffect(bool isHero) {
			if (isHero) {
				HeroEffect.Play();
			} else {
				BikeEffect.Play();
			}
		}

		public void ShowFastUpgradeEffect() {
			FastLvUpEffect.Play();
		}

		public void MoveNext() {
			nextid = nextid >= GarageBoard.Ins._data.Count - 1 ? 0 : nextid + 1;
			ShowBike(GarageBoard.Ins._data[nextid].Info.Data.Prefab);
			this.MoveInFromRight(this.Bike);
			this.DelayInvoke(ClosePre, 0.2f);
		}

		public void MovePre() {
			nextid = nextid <= 0 ? GarageBoard.Ins._data.Count - 1 : nextid - 1;
			ShowBike(GarageBoard.Ins._data[nextid].Info.Data.Prefab);
			this.MoveInFromLeft(this.Bike);
			this.DelayInvoke(ClosePre, 0.2f);
		}

		public void ClosePre() {
			isMoving = false;
			GarageBoard.Ins.ShowBike(nextid);
		}
	}

}

