using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

static public class StringExtensions {

	/// <summary>
	/// Returns the string, converted to PascalCase
	/// </summary>
	static public string ToPascalCase(this string value)
	{
		string[] words = value.ToLower().Split(' ', '-', '.');
		var sb = new StringBuilder();

		for (int i = 0; i < words.Length; i++) { 
			sb.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i]));
		}
		return sb.ToString();
	}

	/// <summary>
	/// Returns the string, converted to camelCase
	/// </summary>
	static public string ToCamelCase(this string value)
	{
		string[] words = value.ToLower().Split(' ', '-', '.');
		var sb = new StringBuilder();

		for (int i = 0; i < words.Length; i++) {
			if (i == 0)
				sb.Append(words[i]);
			else
				sb.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i]));
		}

		return sb.ToString();
	}

	/// <summary>
	/// Split a camelCase (or PascalCase) string into words seperated by spaces.  Acronyms are not split.
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	static public string SplitCamelCase(this string value)
	{
		return Regex.Replace(Regex.Replace(value, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
	}
	// There are loads of alternate RegEx expressions scattered across the web to do this!  Maybe one of these is worth checking out...
	//	return Regex.Replace(value, @"(?<=[a-z])([A-Z])", @" $1").Trim();
	//	return Regex.Replace(value, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", @" $1").Trim();

	/// <summary>
	/// Returns a new string with any HTML tags removed
	/// </summary>
	static public string StripTags(this string value)
	{
		return Regex.Replace(value, "<.*?>", string.Empty);
	}

	/// <summary>
	/// Returns the string in title case (except for words that are entirely in uppercase, which are considered to be acronyms).
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	static public string ToTitleCase(this string value)
	{
		return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
	}

}

