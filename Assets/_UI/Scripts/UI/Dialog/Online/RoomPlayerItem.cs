using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI {
	public class RoomPlayerItem : MonoBehaviour {

		public int Index;

		public GameObject MimeTag;
		public GameObject MasterTag;
		public GameObject KickBtn;
		public Image PlayerIcon;
		public GameObject LockIcon;
		public Text Nickname;
		public GameObject GamingTag;
		public bool HasPlayer { get; set; }


		public void SetAsPlayer(PlayerInfo playerInfo, bool isMine, bool isMaster, bool imMaster,Lobby.PlayerState state) {
			HasPlayer = true;
			this.MimeTag.SetActive(isMine);
			this.MasterTag.SetActive(isMaster);
			if (isMaster) {
				this.KickBtn.SetActive(false);
			} else {
				this.KickBtn.SetActive(imMaster);
			}
			this.LockIcon.SetActive(false);
			this.Nickname.text = playerInfo.NickName;
			this.PlayerIcon.gameObject.SetActive(true);
			this.PlayerIcon.sprite = playerInfo.ChoosedHero.Data.Icon.Sprite;
			this.GamingTag.SetActive(state==Lobby.PlayerState.Gaming);
		}

		public void SetAsLock() {
			HasPlayer = false;
			this.Nickname.text = "";
			this.LockIcon.SetActive(true);
			this.MasterTag.SetActive(false);
			this.MimeTag.SetActive(false);
			this.KickBtn.SetActive(false);
			this.PlayerIcon.gameObject.SetActive(false);
			this.GamingTag.SetActive(false);
		}


		public void SetAsEmpty() {
			HasPlayer = false;
			this.Nickname.text = "";
			this.LockIcon.SetActive(false);
			this.MasterTag.SetActive(false);
			this.MimeTag.SetActive(false);
			this.KickBtn.SetActive(false);
			this.PlayerIcon.gameObject.SetActive(false);
			this.GamingTag.SetActive(false);
		}

		public void __OnKickClick() {
			OnlineRoomBoard.Ins.OnKickClick(this.Index);
		}
	}
}