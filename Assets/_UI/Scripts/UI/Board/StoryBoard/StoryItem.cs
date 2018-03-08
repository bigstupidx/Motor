using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
	public class StoryItem : MonoBehaviour
	{
		public Image Icon;
		public Image IconBg;
		public Text MsgPuppet;
		public Text Msg;
		public TypewriterEffect Typewriter;
		public VerticalLayoutGroup Vlg;
		public GameObject LeftArrows;
		public GameObject RightArrows;
		public Vector2 LeftPos;
		public Vector2 RightPos;
		public RectTransform ChatBox;

		private StoryData storyData;

		public void SetData(StoryData data)
		{
			storyData = data;

			if (data.Icon == null)
			{
				Vlg.childAlignment = TextAnchor.UpperCenter;
				IconBg.gameObject.SetActive(false);
				LeftArrows.gameObject.SetActive(false);
				RightArrows.gameObject.SetActive(false);
				ChatBox.pivot = new Vector2(0.5f,0.5f);
			} else if (data.IsLeft)
			{
				Vlg.childAlignment = TextAnchor.UpperLeft;
				IconBg.gameObject.SetActive(true);
				IconBg.GetComponent<RectTransform>().anchoredPosition = LeftPos;
				Icon.sprite = storyData.Icon.Sprite;
				LeftArrows.gameObject.SetActive(true);
				RightArrows.gameObject.SetActive(false);
				ChatBox.pivot = new Vector2(0f, 0.5f);
			}
			else
			{
				Vlg.childAlignment = TextAnchor.UpperRight;
				IconBg.gameObject.SetActive(true);
				IconBg.GetComponent<RectTransform>().anchoredPosition = RightPos;
				Icon.sprite = storyData.Icon.Sprite;
				LeftArrows.gameObject.SetActive(false);
				RightArrows.gameObject.SetActive(true);
				ChatBox.pivot = new Vector2(1f, 0.5f);
			}
			
			MsgPuppet.text = storyData.Msg;
			Msg.text = storyData.Msg;
			Typewriter.ResetToBeginning();
			Typewriter.onFinished.Add(new EventDelegate(this,"OnFinish"));
		}

		public void OnFinish()
		{
			StoryBoard.Ins.OnFinish(storyData.IndexInMatch);
		}

	}

}

