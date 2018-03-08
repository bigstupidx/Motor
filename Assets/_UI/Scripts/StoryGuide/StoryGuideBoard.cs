//
// Author:
// [LiuSui]
//
// Copyright (C) 2016 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using Game;
using UnityEngine;
using UnityEngine.UI;
using GameUI;
using HeavyDutyInspector;
using XUI;

public class StoryGuideBoard : SingleUIStackBehaviour<StoryGuideBoard> {
	public const string UIPrefabPath = "UI/Board/StoryGuideBoard";

	public static void Show() {
		string[] UIPrefabNames ={
			UIPrefabPath
		};
		ModHelp.Ins.Cover(UIPrefabNames, "StoryGuideBoard");
	}

	public static Sprite daihaoL;
	private Camera UICam;

	[ComplexHeader("遮罩", Style.Box, Alignment.Left, ColorEnum.Yellow, ColorEnum.White)]
	public Image BlackBack;
	public HelpOnClick HelpOnClickScreen;
	public DarkHole DarkHole;

	[ComplexHeader("手指", Style.Box, Alignment.Left, ColorEnum.Yellow, ColorEnum.White)]
	public Transform FingerTrans;
	public MaskableGraphic FingerIcon;
	public Transform EffectPos;
	public HelpOnClick HelpOnClickFinger;

	[ComplexHeader("对话框", Style.Box, Alignment.Left, ColorEnum.Yellow, ColorEnum.White)]
	public Transform WordTrans;
	public Image WordIcon;
	public Image WordImage;
	public Text WordText;
	public Text WordTextFill;
	public Text TipText;
	public Text TipTextFill;

	private float _radius = 3500f;
	private float _radiusStart = 3500f;
	private float _radiusEnd = 250f;
	private float _radiusTarget = 3500f;
	private Vector3 _targetPos = Vector3.zero;
	private float _darkHoleZoomSpeed = 5000f;
	private float _moveSpeed = 3.5f;
	private TypewriterEffect _typeEffect;
	private TweenScale _wordTween;

	private bool _isTextFinish = false;

	private float _timeInterval = 0.45f;
	private float _timer;
	private string _tipText;

	protected void Awake() {
		DarkHole.cam = GameObject.Find("UI/UICamera").GetComponent<Camera>();
		UICam = DarkHole.cam;
		_typeEffect = WordText.GetComponent<TypewriterEffect>();
		_wordTween = WordTrans.GetComponent<TweenScale>();
		var finish = new EventDelegate(this, "OnTextFinish");
		_typeEffect.onFinished.Add(finish);

		daihaoL = AtlasManager.GetSprite("Icon", "Icon_Hero_3004");
	}

	void Update() {
		_timer += Time.unscaledDeltaTime;
		// 移动
		FingerTrans.transform.position = Vector3.MoveTowards(FingerTrans.transform.position, _targetPos, Time.unscaledDeltaTime * _moveSpeed);
		// 遮罩缩放
		_radius = Mathf.MoveTowards(_radius, _radiusTarget, Time.unscaledDeltaTime * _darkHoleZoomSpeed);
		// 遮罩位置
		DarkHole.SetByWorld(FingerTrans.transform.position, new Vector2(_radius, _radius));
	}

	/// <summary>
	/// 开关黑底
	/// </summary>
	/// <param name="active"></param>
	public void ActiveBlackBack(bool active) {
		if (active) {
			TweenAlpha.Begin(BlackBack.gameObject, 0.5f, 0.5f);
		} else {
			TweenAlpha.Begin(BlackBack.gameObject, 0.5f, 0f);
		}
	}

	/// <summary>
	/// 开关手指
	/// </summary>
	/// <param name="active"></param>
	public void ActiveFinger(bool active) {
		if (active) {
			TweenAlpha.Begin(FingerIcon.gameObject, 0.2f, 1f);
		} else {
			TweenAlpha.Begin(FingerIcon.gameObject, 0.2f, 1f / 255f);
		}
		EffectPos.gameObject.SetActive(active);
	}

	/// <summary>
	/// 开关遮罩
	/// </summary>
	/// <param name="active"></param>
	public void ActiveDarkHole(bool active) {
		if (active) {
			_radiusTarget = _radiusEnd;
		} else {
			_radiusTarget = _radiusStart;
		}
	}

	/// <summary>
	/// 开关对话框
	/// </summary>
	/// <param name="active"></param>
	public void ActiveWord(bool active) {
		if (active) {
			WordTrans.gameObject.SetActive(true);
			_wordTween.ResetToBeginning();
			_wordTween.PlayForward();
			TweenCanvasGroupAlpha.Begin(WordTrans.gameObject, 0.3f, 1f);
		} else {
			TweenCanvasGroupAlpha.Begin(WordTrans.gameObject, 0.5f, 0f);
		}
	}

	/// <summary>
	/// 设置高亮区域半径<para/>
	/// 在设置手指位置后调用
	/// </summary>
	/// <param name="radius"></param>
	public void SetDarkHoleRadius(float radius) {
		_radiusTarget = radius;
	}

	public void SetFingerPos(Transform trans) {
		SetFingerPos(trans, new Vector3(0.5f, 0.5f, 0f));
	}

	public void SetFingerPos(Transform trans, Vector3 offset) {
		var pos = Vector3.zero;
		if (trans != null) {
			pos = trans.position;
		}
		_radiusTarget = _radiusEnd;
		_targetPos = pos + AdaptScreen(offset);
	}

	public void SetWordPos(Transform trans, string text, string tipText = null, Sprite headIcon = null, Sprite image = null) {
		SetWordPos(trans, new Vector3(0.5f, 0.5f, 0f), text, tipText, headIcon, image);
	}

	public void SetWordPos(Transform trans, Vector3 offset, string text, string tipText = null, Sprite headIcon = null, Sprite image = null) {
		ActiveWord(true);
		// pos
		var pos = Vector3.zero;
		if (trans != null) {
			pos = trans.position;
		}
		WordTrans.position = pos + AdaptScreen(offset);
		HelpOnClickScreen.transform.position = Vector3.zero;
		// change icon
		if (headIcon != null) {
			WordIcon.sprite = headIcon;
		}
		// set image
		WordImage.gameObject.SetActive(image != null);
		WordImage.sprite = image;
		// text
		WordTextFill.text = text;
		WordText.text = "";
		_typeEffect.Finish();

		WordText.text = text;
		_typeEffect.ResetToBeginning();
		_isTextFinish = false;

		this.TipText.text = "";
		this.TipTextFill.text = "";
		this._tipText = tipText;
	}

	public void OnTextFinish() {
		_isTextFinish = true;
		if (this._tipText != null) {
			this.TipText.text = this._tipText;
			this.TipTextFill.text = this._tipText;
		}
	}

	public bool CheckEnd() {
		if (!_isTextFinish) {
			SetAnimaEnd();
			_timer = 0f;
		}
		// Debug.Log(_timer + " " + (_timer >= _timeInterval));
		var result = _timer >= _timeInterval;
		if (result) _timer = 0f;
		return result;
	}

	public void SetAnimaEnd() {
		_typeEffect.Finish();
		// _wordTween.Sample(1, true);
	}

	/// <summary>
	/// 视口坐标转换到世界
	/// </summary>
	/// <param name="pos"></param>
	/// <returns></returns>
	public static Vector3 AdaptScreen(Vector3 pos) {
		//var rate = Screen.width * 1f / Screen.height;
		//pos = pos / rate * (16f / 9f);
		return Ins.UICam.ViewportToWorldPoint(pos);
	}

	public void Close() {
		ActiveFinger(false);
		ActiveDarkHole(false);
		ActiveBlackBack(false);
		ActiveWord(false);
		this.DelayInvoke(() => {
			ModHelp.Ins.Back(true);
		}, 0.9f);
	}

	public bool IsScreen() {
		var rate = Screen.width * 1f / Screen.height;
		if (rate > (4f / 3f)) {
			return true;
		}
		return false;
	}
}
