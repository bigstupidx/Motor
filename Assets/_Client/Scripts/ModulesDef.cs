using UnityEngine;

namespace GameClient {
	public partial class Client {
		/// <summary>
		/// 系统模块
		/// </summary>
		public static ModSystem System {
			get { return Ins.system; }
		}

		private ModSystem system;

		/// <summary>
		/// 系统模块
		/// </summary>
		public static ModIcon Icon {
			get { return Ins.icon; }
		}

		private ModIcon icon;

		/// <summary>
		/// 游戏模块
		/// </summary>
		public static ModGame Game {
			get { return Ins.game; }
		}

		private ModGame game;

		/// <summary>
		/// 设置模块
		/// </summary>
		public static ModSetting Setting {
			get { return Ins.setting; }
		}

		private ModSetting setting;

		/// <summary>
		/// 玩家模块
		/// </summary>
		public static ModUser User {
			get { return Ins.user; }
		}

		private ModUser user;

		public static ModStamina Stamina {
			get { return Ins.stamina; }
		}

		private ModStamina stamina;

		/// <summary>
		/// 物品模块
		/// </summary>
		public static ModItem Item {
			get { return Ins.item; }
		}

		private ModItem item;

		/// <summary>
		/// 人物模块
		/// </summary>
		public static ModHero Hero {
			get { return Ins.hero; }
		}

		private ModHero hero;

		/// <summary>
		/// 赛车模块
		/// </summary>
		public static ModBike Bike {
			get { return Ins.bike; }
		}

		private ModBike bike;

		/// <summary>
		/// 道具模块
		/// </summary>
		public static ModProp Prop {
			get { return Ins.prop; }
		}

		private ModProp prop;

		/// <summary>
		/// 武器模块
		/// </summary>
		public static ModWeapon Weapon {
			get { return Ins.weapon; }
		}

		private ModWeapon weapon;

		/// <summary>
		/// 挑战模块
		/// </summary>
		public static ModChallenge Challenge {
			get { return Ins.challenge; }
		}

		private ModChallenge challenge;

		/// <summary>
		/// 关卡模块
		/// </summary>
		public static ModMatch Match {
			get { return Ins.match; }
		}
		private ModMatch match;

		/// <summary>
		/// 商店模块
		/// </summary>
		public static ModIAP IAP {
			get { return Ins.iap; }
		}
		private ModIAP iap;

		/// <summary>
		/// 礼包模块
		/// </summary>
		public static ModSpree Spree {
			get { return Ins.spree; }
		}
		private ModSpree spree;

		/// <summary>
		/// 签到模块
		/// </summary>
		public static ModSign Sign {
			get { return Ins.sign; }
		}
		private ModSign sign;

		public static ModSensorSpree SensorSpree {
			get { return Ins.sensorSpree; }
		}
		private ModSensorSpree sensorSpree;

		/// <summary>
		/// 剧情模块
		/// </summary>
		public static ModStory Story {
			get { return Ins.story; }
		}
		private ModStory story;

		/// <summary>
		/// 配置模块
		/// </summary>
		public static ModConfig Config {
			get { return Ins.config; }
		}

		private ModConfig config;

		/// <summary>
		/// 任务模块
		/// </summary>
		public static ModTask Task {
			get { return Ins.task; }
		}

		private ModTask task;

		/// <summary>
		/// 昵称模块
		/// </summary>
		public static ModNickName Nickname {
			get { return Ins.nickName; }
		}

		private ModNickName nickName;


		/// <summary>
		/// Online模块
		/// </summary>
		public static ModOnline Online {
			get { return Ins.online; }
		}

		private ModOnline online;

		/// <summary>
		/// 锦标赛模块
		/// </summary>
		public static ModChampionship Championship {
			get { return Ins.championship; }
		}

		private ModChampionship championship;

		/// <summary>
		/// Guide模块
		/// </summary>
		public static ModGuide Guide {
			get { return Ins.guide; }
		}

		private ModGuide guide;

		/// <summary>
		/// 场景模块
		/// </summary>
		public static ModScene Scene {
			get { return Ins.scene; }
		}

		private ModScene scene;

		/// <summary>
		/// 提示模块
		/// </summary>
		public static ModHint Hint {
			get { return Ins.hint; }
		}

		private ModHint hint;

		/// <summary>
		/// 敏感字过滤模块
		/// </summary>
		public static ModDFA DFA {
			get { return Ins.dfa; }
		}

		private ModDFA dfa;

		/// <summary>
		/// 事件管理器
		/// </summary>
		public static ModEventManager EventMgr {
			get { return Ins.eventMgr; }
		}
		private ModEventManager eventMgr;

		/// <summary>
		/// 数据统计模块
		/// </summary>
		public static ModAnalytics Analytics {
			get { return Ins.analytics; }
		}
		private ModAnalytics analytics;

		public static ModRank Rank {
			get { return Ins.rank; }
		}
		private ModRank rank;

		void FindModules() {
			FindModule(out system);
			FindModule(out icon);
			FindModule(out item);
			FindModule(out config);
			FindModule(out setting);
			FindModule(out task);
			FindModule(out scene);
			FindModule(out game);
			FindModule(out stamina);
			FindModule(out eventMgr);
			FindModule(out hero);
			FindModule(out bike);
			FindModule(out prop);
			FindModule(out weapon);
			FindModule(out match);
			FindModule(out nickName);
			FindModule(out online);
			FindModule(out championship);
			FindModule(out guide);
			FindModule(out iap);
			FindModule(out spree);
			FindModule(out user);
			FindModule(out sign);
			FindModule(out sensorSpree);
			FindModule(out story);
			FindModule(out analytics);
			FindModule(out hint);
			FindModule(out dfa);
			FindModule(out challenge);
			FindModule(out rank);
		}

		void FindModule<T>(out T mod) where T : ClientModule {
			mod = GetComponentInChildren<T>();
			if (mod == null) {
				Debug.LogError("Client module [" + typeof(T) + "] missing!");
			} else {
				if (modulesList.IndexOf(mod) >= 0) {
					Debug.LogError("Client module [" + typeof(T) + "] is already exist!");
				} else {
					modulesList.Add(mod);
				}
			}
		}
	}
}
