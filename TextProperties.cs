using System;
using TMPro;
using UnityEngine;

[Serializable]
public class TextProperties
{
	[TextArea]
	public string text;
	public Color textColor;
	public float textSize;
	public FontStyles fontStyle;
	public TMP_FontAsset font;

	public TextProperties(string _text, FontStyles _fontStyle, float _textSize, Color _textColor)
	{
		text = _text;
		fontStyle = _fontStyle;
		textSize = _textSize;
		textColor = _textColor;
	}
}
