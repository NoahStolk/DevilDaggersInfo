using Microsoft.JSInterop.WebAssembly;

namespace UmCanvas;

public abstract class Canvas
{
	private static readonly WebAssemblyJSRuntime _runtime = new CustomWebAssemblyJSRuntime();

	protected Canvas(string id)
	{
		Id = id;
	}

	public string Id { get; }

	public void Invoke(string identifier) => InvokeRet<object>(identifier);
	public void Invoke<T0>(string identifier, T0 arg0) => InvokeRet<T0, object>(identifier, arg0);
	public void Invoke<T0, T1>(string identifier, T0 arg0, T1 arg1) => InvokeRet<T0, T1, object>(identifier, arg0, arg1);
	public void Invoke<T0, T1, T2>(string identifier, T0 arg0, T1 arg1, T2 arg2) => InvokeRet<T0, T1, T2, object>(identifier, arg0, arg1, arg2);
	public void Invoke<T0, T1, T2, T3>(string identifier, T0 arg0, T1 arg1, T2 arg2, T3 arg3) => InvokeRet<T0, T1, T2, T3, object>(identifier, arg0, arg1, arg2, arg3);
	public void Invoke<T0, T1, T2, T3, T4>(string identifier, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => InvokeRet<T0, T1, T2, T3, T4, object>(identifier, arg0, arg1, arg2, arg3, arg4);
	public void Invoke<T0, T1, T2, T3, T4, T5>(string identifier, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => InvokeRet<T0, T1, T2, T3, T4, T5, object>(identifier, arg0, arg1, arg2, arg3, arg4, arg5);
	public void Invoke<T0, T1, T2, T3, T4, T5, T6>(string identifier, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => InvokeRet<T0, T1, T2, T3, T4, T5, T6, object>(identifier, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
	public void Invoke<T0, T1, T2, T3, T4, T5, T6, T7>(string identifier, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => InvokeRet<T0, T1, T2, T3, T4, T5, T6, T7, object>(identifier, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

	public TRes InvokeRet<TRes>(string identifier) => _runtime.InvokeUnmarshalled<ValueTuple<string>, TRes>(identifier, ValueTuple.Create(Id));
	public TRes InvokeRet<T0, TRes>(string identifier, T0 arg0) => _runtime.InvokeUnmarshalled<ValueTuple<string, T0>, TRes>(identifier, (Id, arg0));
	public TRes InvokeRet<T0, T1, TRes>(string identifier, T0 arg0, T1 arg1) => _runtime.InvokeUnmarshalled<ValueTuple<string, T0, T1>, TRes>(identifier, (Id, arg0, arg1));
	public TRes InvokeRet<T0, T1, T2, TRes>(string identifier, T0 arg0, T1 arg1, T2 arg2) => _runtime.InvokeUnmarshalled<ValueTuple<string, T0, T1, T2>, TRes>(identifier, (Id, arg0, arg1, arg2));
	public TRes InvokeRet<T0, T1, T2, T3, TRes>(string identifier, T0 arg0, T1 arg1, T2 arg2, T3 arg3) => _runtime.InvokeUnmarshalled<ValueTuple<string, T0, T1, T2, T3>, TRes>(identifier, (Id, arg0, arg1, arg2, arg3));
	public TRes InvokeRet<T0, T1, T2, T3, T4, TRes>(string identifier, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => _runtime.InvokeUnmarshalled<ValueTuple<string, T0, T1, T2, T3, T4>, TRes>(identifier, (Id, arg0, arg1, arg2, arg3, arg4));
	public TRes InvokeRet<T0, T1, T2, T3, T4, T5, TRes>(string identifier, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => _runtime.InvokeUnmarshalled<ValueTuple<string, T0, T1, T2, T3, T4, T5>, TRes>(identifier, (Id, arg0, arg1, arg2, arg3, arg4, arg5));
	public TRes InvokeRet<T0, T1, T2, T3, T4, T5, T6, TRes>(string identifier, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => _runtime.InvokeUnmarshalled<ArgStruct8<string, T0, T1, T2, T3, T4, T5, T6>, TRes>(identifier, new(Id, arg0, arg1, arg2, arg3, arg4, arg5, arg6));
	public TRes InvokeRet<T0, T1, T2, T3, T4, T5, T6, T7, TRes>(string identifier, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => _runtime.InvokeUnmarshalled<ArgStruct9<string, T0, T1, T2, T3, T4, T5, T6, T7>, TRes>(identifier, new(Id, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7));

	private readonly record struct ArgStruct8<T0, T1, T2, T3, T4, T5, T6, T7>(T0 Arg0, T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4, T5 Arg5, T6 Arg6, T7 Arg7);
	private readonly record struct ArgStruct9<T0, T1, T2, T3, T4, T5, T6, T7, T8>(T0 Arg0, T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4, T5 Arg5, T6 Arg6, T7 Arg7, T8 Arg8);

	private sealed class CustomWebAssemblyJSRuntime : WebAssemblyJSRuntime
	{
	}
}