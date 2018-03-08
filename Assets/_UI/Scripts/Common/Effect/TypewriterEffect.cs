using UnityEngine;
using System.Text;
using System.Collections.Generic;
using UnityEngine.UI;
using GameUI;

/// <summary>
/// This script is able to fill in the label's text gradually, giving the effect of someone typing or fading in the content over time.
/// </summary>

[RequireComponent(typeof(Text))]
[AddComponentMenu("UI/Typewriter Effect")]
public class TypewriterEffect : MonoBehaviour
{
	static public TypewriterEffect current;

	struct FadeEntry
	{
		public int index;
		public string text;
		public float alpha;
	}

	/// <summary>
	/// How many characters will be printed per second.
	/// </summary>

	public int charsPerSecond = 20;

	/// <summary>
	/// How long it takes for each character to fade in.
	/// </summary>

	public float fadeInTime = 0f;

	/// <summary>
	/// How long to pause when a period is encountered (in seconds).
	/// </summary>

	public float delayOnPeriod = 0f;

	/// <summary>
	/// How long to pause when a new line character is encountered (in seconds).
	/// </summary>

	public float delayOnNewLine = 0f;

	/// <summary>
	/// If a scroll view is specified, its UpdatePosition() function will be called every time the text is updated.
	/// </summary>

	public ScrollRect scrollView;

	/// <summary>
	/// If set to 'true', the label's dimensions will be that of a fully faded-in content.
	/// </summary>

	public bool keepFullDimensions = false;

	/// <summary>
	/// Event delegate triggered when the typewriter effect finishes.
	/// </summary>

	public List<EventDelegate> onFinished = new List<EventDelegate>();

	Text mLabel;
	string mFullText = "";
	int mCurrentOffset = 0;
	float mNextChar = 0f;
	bool mReset = true;
	bool mActive = false;

	List<FadeEntry> mFade = new List<FadeEntry>();

	/// <summary>
	/// Whether the typewriter effect is currently active or not.
	/// </summary>

	public bool isActive { get { return mActive; } }

	/// <summary>
	/// Reset the typewriter effect to the beginning of the label.
	/// </summary>

	public void ResetToBeginning ()
	{
		Finish();
		mReset = true;
		mActive = true;
		mNextChar = 0f;
		mCurrentOffset = 0;
		Update();
	}

	/// <summary>
	/// Finish the typewriter operation and show all the text right away.
	/// </summary>

	public void Finish ()
	{
		if (mActive)
		{
			mActive = false;

			if (!mReset)
			{
				mCurrentOffset = mFullText.Length;
				mFade.Clear();
				mLabel.text = mFullText;
			}

			current = this;
			EventDelegate.Execute(onFinished);
			current = null;
		}
	}

	void OnEnable () { mReset = true; mActive = true; }

	void Update ()
	{
		if (!mActive) return;

		if (mReset)
		{
			mCurrentOffset = 0;
			mReset = false;
			mLabel = GetComponent<Text>();
			mFullText = mLabel.text;
			mFade.Clear();

		}

		while (mCurrentOffset < mFullText.Length && mNextChar <= RealTime.time)
		{
			int lastOffset = mCurrentOffset;
			charsPerSecond = Mathf.Max(1, charsPerSecond);

			// TODO 添加标签过滤
			++mCurrentOffset;

			// Reached the end? We're done.
			if (mCurrentOffset > mFullText.Length) break;

			// Periods and end-of-line characters should pause for a longer time.
			float delay = 1f / charsPerSecond;
			char c = (lastOffset < mFullText.Length) ? mFullText[lastOffset] : '\n';

			if (c == '\n')
			{
				delay += delayOnNewLine;
			}
			else if (lastOffset + 1 == mFullText.Length || mFullText[lastOffset + 1] <= ' ')
			{
				if (c == '.')
				{
					if (lastOffset + 2 < mFullText.Length && mFullText[lastOffset + 1] == '.' && mFullText[lastOffset + 2] == '.')
					{
						delay += delayOnPeriod * 3f;
						lastOffset += 2;
					}
					else delay += delayOnPeriod;
				}
				else if (c == '!' || c == '?')
				{
					delay += delayOnPeriod;
				}
			}

			if (mNextChar == 0f)
			{
				mNextChar = RealTime.time + delay;
			}
			else mNextChar += delay;

			if (fadeInTime != 0f)
			{
				// There is smooth fading involved
				FadeEntry fe = new FadeEntry();
				fe.index = lastOffset;
				fe.alpha = 0f;
				fe.text = mFullText.Substring(lastOffset, mCurrentOffset - lastOffset);
				mFade.Add(fe);
			}
			else
			{
				// No smooth fading necessary
				mLabel.text = keepFullDimensions ?
					mFullText.Substring(0, mCurrentOffset) + "[00]" + mFullText.Substring(mCurrentOffset) :
					mFullText.Substring(0, mCurrentOffset);
			}
		}

		// Alpha-based fading
		if (mFade.Count != 0)
		{
			for (int i = 0; i < mFade.Count; )
			{
				FadeEntry fe = mFade[i];
				fe.alpha += RealTime.deltaTime / fadeInTime;

				if (fe.alpha < 1f)
				{
					mFade[i] = fe;
					++i;
				}
				else mFade.RemoveAt(i);
			}

			if (mFade.Count == 0)
			{
				if (keepFullDimensions) mLabel.text = mFullText.Substring(0, mCurrentOffset) + "[00]" + mFullText.Substring(mCurrentOffset);
				else mLabel.text = mFullText.Substring(0, mCurrentOffset);
			}
			else
			{
				StringBuilder sb = new StringBuilder();

				for (int i = 0; i < mFade.Count; ++i)
				{
					FadeEntry fe = mFade[i];

					if (i == 0)
					{
						sb.Append(mFullText.Substring(0, fe.index));
					}
					sb.Append(EncodeAlpha(fe.text,fe.alpha));
				}

				if (keepFullDimensions)
				{
					sb.Append("[00]");
					sb.Append(mFullText.Substring(mCurrentOffset));
				}

				mLabel.text = sb.ToString();
			}
		}
		else if (mCurrentOffset >= mFullText.Length)
		{
			current = this;
			EventDelegate.Execute(onFinished);
			current = null;
			mActive = false;
		}
	}

	string EncodeAlpha(string text, float a){
		string colorCode = "";
		int alpha = Mathf.Clamp (Mathf.RoundToInt (a * 255f), 0, 255);
		colorCode += UIDataDef.DecimalToHex8 (Mathf.RoundToInt (mLabel.color.r * 255f));
		colorCode += UIDataDef.DecimalToHex8 (Mathf.RoundToInt (mLabel.color.g * 255f));
		colorCode += UIDataDef.DecimalToHex8 (Mathf.RoundToInt (mLabel.color.b * 255f));
		colorCode += UIDataDef.DecimalToHex8 (alpha);
		return "<color=#" + colorCode +">" + text + "</color>";
	}


}
