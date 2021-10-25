namespace DevilDaggersInfo.SourceGen.Web.BlazorWasm.Client.Extensions;

public static class StringExtensions
{
	public static string ToUsingDirective(this string namespaceString)
		=> $"using {namespaceString};";

	public static string TrimStart(this string str, params string[] values)
	{
		if (values.Length == 0)
			return str;

		foreach (string value in values)
		{
			if (str.StartsWith(value))
				return str.Substring(value.Length);
		}

		return str;
	}
}