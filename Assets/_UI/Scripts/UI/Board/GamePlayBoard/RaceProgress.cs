using GameClient;
using UnityEngine;

namespace GameUI
{
	public class RaceProgress : MonoBehaviour {

		public XSlider Progress;
		public UIImageNumber Rank;

		private int currentRank;//当前排名
		private float lastUpdateRankTime;//上一次更新排名的时间

		public void Init(RectTransform parentRt, int rank)
		{
			Progress.Value = 0;
			Rank.Text = rank;
			RectTransform rt = GetComponent<RectTransform>();
			rt.SetParent(parentRt);
			rt.localScale = Vector3.one;
			rt.anchoredPosition = new Vector2(0,Random.Range(-30,30));
		}

		public void Refresh(float progressVal, int rank)
		{
			if (Client.Game.MatchInfo.Data.GameMode == GameMode.Elimination ||
			    Client.Game.MatchInfo.Data.GameMode == GameMode.EliminationProp)
			{
				Progress.Value = Mathf.MoveTowards(Progress.Value, progressVal, Time.deltaTime * 0.05f);
			}
			else
			{
				Progress.Value = progressVal;
			}
	
			if (currentRank != rank && Time.timeSinceLevelLoad - lastUpdateRankTime > 0.5f)
			{
				Rank.Text = rank;
				currentRank = rank;
				lastUpdateRankTime = Time.timeSinceLevelLoad;
			}
		}

		public void Reset()
		{
			currentRank = 0;
			lastUpdateRankTime = 0;
		}
	}

}

