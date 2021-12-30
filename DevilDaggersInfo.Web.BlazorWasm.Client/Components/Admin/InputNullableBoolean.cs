namespace DevilDaggersInfo.Web.BlazorWasm.Client.Components.Admin;

public partial class InputNullableBoolean
{
	public sbyte ValueAsSignedByte
	{
		get => CurrentValue switch
		{
			true => 1,
			false => 0,
			_ => -1,
		};
		set => CurrentValue = value switch
		{
			1 => true,
			0 => false,
			_ => null,
		};
	}

	protected override bool TryParseValueFromString(string? value, out bool? result, out string validationErrorMessage)
	{
		(validationErrorMessage, result) = value switch
		{
			"Unknown" => (string.Empty, (bool?)null),
			"False" => (string.Empty, (bool?)false),
			"True" => (string.Empty, (bool?)true),
			_ => ($"{value} is not a supported value for {nameof(InputNullableBoolean)}.", (bool?)null),
		};
		return validationErrorMessage == string.Empty;
	}
}
