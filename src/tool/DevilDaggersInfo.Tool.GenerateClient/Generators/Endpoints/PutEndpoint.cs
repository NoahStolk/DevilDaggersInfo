namespace DevilDaggersInfo.Tool.GenerateClient.Generators.Endpoints;

internal class PutEndpoint : Endpoint
{
	private const string _methodName = $"%{nameof(_methodName)}%";
	private const string _routeParameter = $"%{nameof(_routeParameter)}%";
	private const string _bodyParameterType = $"%{nameof(_bodyParameterType)}%";
	private const string _bodyParameter = $"%{nameof(_bodyParameter)}%";
	private const string _apiRoute = $"%{nameof(_apiRoute)}%";
	private const string _httpMethod = $"%{nameof(_httpMethod)}%";
	private const string _endpointTemplate = $@"public async Task<HttpResponseMessage> {_methodName}({_routeParameter}, {_bodyParameterType} {_bodyParameter})
{{
	return await SendRequest(new HttpMethod(""{_httpMethod}""), $""{_apiRoute}"", JsonContent.Create({_bodyParameter}));
}}
";

	public PutEndpoint(string methodName, string apiRoute, Parameter routeParameter, Parameter bodyParameter)
		: base(HttpMethod.Put, methodName, apiRoute)
	{
		RouteParameter = routeParameter;
		BodyParameter = bodyParameter;
	}

	public Parameter RouteParameter { get; }

	public Parameter BodyParameter { get; }

	public override string Build()
	{
		string bodyParameterStr = BodyParameter.ToString();
		string bodyParameterType = bodyParameterStr.Substring(0, bodyParameterStr.IndexOf(' '));
		string bodyParameter = bodyParameterStr.Substring(bodyParameterStr.IndexOf(' ') + 1);

		return _endpointTemplate
			.Replace(_methodName, MethodName)
			.Replace(_routeParameter, RouteParameter.ToString())
			.Replace(_bodyParameterType, bodyParameterType)
			.Replace(_bodyParameter, bodyParameter)
			.Replace(_httpMethod, HttpMethod.ToString())
			.Replace(_apiRoute, ApiRoute);
	}
}
