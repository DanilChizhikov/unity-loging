using System;
using UnityEngine;

namespace DTech.Logging.Editor
{
	[Serializable]
	internal sealed class TagColorRule
	{
		[SerializeField] private string _tag = string.Empty;
		[SerializeField] private Color _color = new Color(0.25f, 0.45f, 0.7f, 0.35f);
		[SerializeField] private bool _enabled = true;

		public string Tag => _tag;
		public Color Color => _color;
		public bool Enabled => _enabled;
	}
}