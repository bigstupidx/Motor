//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
using UnityEngine;
using Task = GameClient.StoryTaskQueue;

namespace GameClient {
	public class StoryGuide_19_WeaponBoard : StoryGuideBase {
		public override void Enable() {
			base.Enable();
			Client.EventMgr.AddListener(EventEnum.UI_EnterMenu, OnEvent);
		}

		public override void Disable() {
			base.Disable();
			Client.EventMgr.RemoveListener(EventEnum.UI_EnterMenu, OnEvent);
		}

		public override void OnEvent(EventEnum eventType, params object[] args) {
			base.OnEvent(eventType, args);
			if ((string)args[0] == "OnWeaponBoardInited") {
                var id = int.Parse(GetType().Name.Split('_')[1]);
                Client.EventMgr.SendEvent(EventEnum.Guide_Start, id);
                StartGuide();
			}
		}

		public void StartGuide() {
			Task.Reset();
			Task.Append(() => {
				Guide.ActiveDarkHole(true);
				Guide.ActiveFinger(true);

				Client.Guide.Log("指向武器列表，棒球");
				var pathWeaponList = "UI/Modules/Menu/Group/WeaponBoard(Clone)/WeaponList/List/List/Container/weaponItem1";
				var heroList = GameObject.Find(pathWeaponList);
				Guide.SetFingerPos(heroList.transform, new Vector3(0.74f, 0.5f, 0f));
				//0.6644, 0.2573, 0.0000
				Guide.SetWordPos(heroList.transform, new Vector3(1f, 0.3f, 0f), (LString.GAMECLIENT_STORYGUIDE_19_WEAPONBOARD_STARTGUIDE).ToLocalized());

				ClearFingerClick(heroList);
			});

			Task.Append(() => {
				Client.Guide.Log("指向购买按钮");
				var pathBuyBtn = "UI/Modules/Menu/Group/WeaponBoard(Clone)/Weapon/Right/Buy";
				var buyBtm = GameObject.Find(pathBuyBtn);
				Guide.SetFingerPos(buyBtm.transform);
				Guide.SetWordPos(buyBtm.transform, new Vector3(0.3f, 0.7f, 0), (LString.GAMECLIENT_STORYGUIDE_19_WEAPONBOARD_STARTGUIDE_1).ToLocalized());

				// 补充不足的货币
				var weapon = Client.Weapon[5002];
				var priceID = weapon.Currency.ID;
				var priceAmount = weapon.CurrencyAmount;
				var item = Client.Item.GetItem(priceID);
				var dis = item.Amount - priceAmount;

				if (dis < 0) {
					item.ChangeAmount(Mathf.Abs(dis));
				}
				ClearFingerClick(buyBtm, Disable);
			});

			Task.Append(() => {
				Client.Guide.Log("指向装备按钮");
				var pathEquipeBtn = "UI/Modules/Menu/Group/WeaponBoard(Clone)/Weapon/Right/Select";
				var equipeBtn = GameObject.Find(pathEquipeBtn);
				Guide.SetFingerPos(equipeBtn.transform);
				Guide.SetWordPos(equipeBtn.transform, new Vector3(0.3f, 0.7f, 0), (LString.GAMECLIENT_STORYGUIDE_19_WEAPONBOARD_STARTGUIDE_2).ToLocalized());
				ClearFingerClick(equipeBtn);
			});

			Task.End += () => {
				Guide.Close();
			};

			Task.Excute();
		}
	}
}

