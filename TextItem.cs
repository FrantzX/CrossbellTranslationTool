using System;
using System.Diagnostics;

namespace CrossbellTranslationTool
{
	class TextItem
	{
		public TextItem()
		{
			Text = "";
			Translation = "";
		}

		public TextItem(String text)
		{
			Assert.IsNotNull(text, nameof(text));

			Text = text;
			Translation = "";
		}

		public TextItem(String text, String translation)
		{
			Assert.IsNotNull(text, nameof(text));
			Assert.IsNotNull(translation, nameof(translation));

			Text = text;
			Translation = translation;
		}

		public String GetBestText()
		{
			return (Translation != "") ? Translation : Text;
		}

		public String Text
		{
			get { return m_text; }

			set
			{
				Assert.IsNotNull(value, nameof(value));

				m_text = value;
			}
		}

		public String Translation
		{
			get { return m_translation; }

			set
			{
				Assert.IsNotNull(value, nameof(value));

				m_translation = value;
			}
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		String m_text;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		String m_translation;

		#endregion
	}
}
