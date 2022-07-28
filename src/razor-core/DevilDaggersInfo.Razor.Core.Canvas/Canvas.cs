using Microsoft.JSInterop.WebAssembly;

namespace DevilDaggersInfo.Razor.Core.Canvas;

public abstract class Canvas
{
	private static readonly CustomWebAssemblyJSRuntime _runtime = new();

	protected Canvas(string id)
	{
		Id = id;
	}

	public string Id { get; }

	public void Invoke(string identifier) => InvokeReturn<object>(identifier);
	public void Invoke<T0>(string identifier, T0 arg0) => InvokeReturn<T0, object>(identifier, arg0);
	public void Invoke<T0, T1>(string identifier, T0 arg0, T1 arg1) => InvokeReturn<T0, T1, object>(identifier, arg0, arg1);
	public void Invoke<T0, T1, T2>(string identifier, T0 arg0, T1 arg1, T2 arg2) => InvokeReturn<T0, T1, T2, object>(identifier, arg0, arg1, arg2);
	public void Invoke<T0, T1, T2, T3>(string identifier, T0 arg0, T1 arg1, T2 arg2, T3 arg3) => InvokeReturn<T0, T1, T2, T3, object>(identifier, arg0, arg1, arg2, arg3);
	public void Invoke<T0, T1, T2, T3, T4>(string identifier, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => InvokeReturn<T0, T1, T2, T3, T4, object>(identifier, arg0, arg1, arg2, arg3, arg4);
	public void Invoke<T0, T1, T2, T3, T4, T5>(string identifier, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => InvokeReturn<T0, T1, T2, T3, T4, T5, object>(identifier, arg0, arg1, arg2, arg3, arg4, arg5);
	public void Invoke<T0, T1, T2, T3, T4, T5, T6>(string identifier, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => InvokeReturn<T0, T1, T2, T3, T4, T5, T6, object>(identifier, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
	public void Invoke<T0, T1, T2, T3, T4, T5, T6, T7>(string identifier, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => InvokeReturn<T0, T1, T2, T3, T4, T5, T6, T7, object>(identifier, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

	public TResult InvokeReturn<TResult>(string identifier) => _runtime.InvokeUnmarshalled<ValueTuple<string>, TResult>(identifier, ValueTuple.Create(Id));
	public TResult InvokeReturn<T0, TResult>(string identifier, T0 arg0) => _runtime.InvokeUnmarshalled<ValueTuple<string, T0>, TResult>(identifier, (Id, arg0));
	public TResult InvokeReturn<T0, T1, TResult>(string identifier, T0 arg0, T1 arg1) => _runtime.InvokeUnmarshalled<ValueTuple<string, T0, T1>, TResult>(identifier, (Id, arg0, arg1));
	public TResult InvokeReturn<T0, T1, T2, TResult>(string identifier, T0 arg0, T1 arg1, T2 arg2) => _runtime.InvokeUnmarshalled<ValueTuple<string, T0, T1, T2>, TResult>(identifier, (Id, arg0, arg1, arg2));
	public TResult InvokeReturn<T0, T1, T2, T3, TResult>(string identifier, T0 arg0, T1 arg1, T2 arg2, T3 arg3) => _runtime.InvokeUnmarshalled<ValueTuple<string, T0, T1, T2, T3>, TResult>(identifier, (Id, arg0, arg1, arg2, arg3));
	public TResult InvokeReturn<T0, T1, T2, T3, T4, TResult>(string identifier, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => _runtime.InvokeUnmarshalled<ValueTuple<string, T0, T1, T2, T3, T4>, TResult>(identifier, (Id, arg0, arg1, arg2, arg3, arg4));
	public TResult InvokeReturn<T0, T1, T2, T3, T4, T5, TResult>(string identifier, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => _runtime.InvokeUnmarshalled<ValueTuple<string, T0, T1, T2, T3, T4, T5>, TResult>(identifier, (Id, arg0, arg1, arg2, arg3, arg4, arg5));
	public TResult InvokeReturn<T0, T1, T2, T3, T4, T5, T6, TResult>(string identifier, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => _runtime.InvokeUnmarshalled<ArgStruct8<string, T0, T1, T2, T3, T4, T5, T6>, TResult>(identifier, new(Id, arg0, arg1, arg2, arg3, arg4, arg5, arg6));
	public TResult InvokeReturn<T0, T1, T2, T3, T4, T5, T6, T7, TResult>(string identifier, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => _runtime.InvokeUnmarshalled<ArgStruct9<string, T0, T1, T2, T3, T4, T5, T6, T7>, TResult>(identifier, new(Id, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7));

	private readonly record struct ArgStruct8<T0, T1, T2, T3, T4, T5, T6, T7>(T0 Arg0, T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4, T5 Arg5, T6 Arg6, T7 Arg7);
	private readonly record struct ArgStruct9<T0, T1, T2, T3, T4, T5, T6, T7, T8>(T0 Arg0, T1 Arg1, T2 Arg2, T3 Arg3, T4 Arg4, T5 Arg5, T6 Arg6, T7 Arg7, T8 Arg8);

	private sealed class CustomWebAssemblyJSRuntime : WebAssemblyJSRuntime
	{
	}
}
