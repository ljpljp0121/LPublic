using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"CoreEngine.dll",
		"System.Core.dll",
		"UnityEngine.CoreModule.dll",
		"UnityEngine.JSONSerializeModule.dll",
		"YooAsset.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// Singleton<object>
	// SingletonMono<object>
	// System.Action<object,object>
	// System.Action<object>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<object>
	// System.Func<System.Threading.Tasks.VoidTaskResult>
	// System.Func<byte>
	// System.Func<object,System.Threading.Tasks.VoidTaskResult>
	// System.Func<object,byte>
	// System.Func<object,object>
	// System.Func<object>
	// System.IEquatable<SerializedVector2>
	// System.IEquatable<object>
	// System.Linq.Buffer<object>
	// System.Linq.EnumerableSorter<object,object>
	// System.Linq.EnumerableSorter<object>
	// System.Linq.OrderedEnumerable.<GetEnumerator>d__1<object>
	// System.Linq.OrderedEnumerable<object,object>
	// System.Linq.OrderedEnumerable<object>
	// System.Predicate<object>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<byte>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<byte>
	// System.Runtime.CompilerServices.TaskAwaiter<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.TaskAwaiter<byte>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<byte>
	// System.Threading.Tasks.Task<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.Task<byte>
	// System.Threading.Tasks.TaskCompletionSource<byte>
	// System.Threading.Tasks.TaskFactory<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.TaskFactory<byte>
	// }}

	public void RefMethods()
	{
		// object AssetSystem.LoadAsset<object>(string)
		// object System.Activator.CreateInstance<object>()
		// object[] System.Array.Empty<object>()
		// System.Linq.IOrderedEnumerable<object> System.Linq.Enumerable.OrderBy<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,object>)
		// System.Linq.IOrderedEnumerable<object> System.Linq.Enumerable.OrderByDescending<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,object>)
		// System.Collections.Generic.List<object> System.Linq.Enumerable.ToList<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,UIBase.<Show>d__10>(System.Runtime.CompilerServices.TaskAwaiter&,UIBase.<Show>d__10&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,UIManager.<>c__DisplayClass20_1.<<ShowUIByNameImp>b__2>d>(System.Runtime.CompilerServices.TaskAwaiter&,UIManager.<>c__DisplayClass20_1.<<ShowUIByNameImp>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,UIBase.<Show>d__10>(System.Runtime.CompilerServices.TaskAwaiter&,UIBase.<Show>d__10&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,UIManager.<>c__DisplayClass20_1.<<ShowUIByNameImp>b__2>d>(System.Runtime.CompilerServices.TaskAwaiter&,UIManager.<>c__DisplayClass20_1.<<ShowUIByNameImp>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<UIBase.<Show>d__10>(UIBase.<Show>d__10&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<UIManager.<>c__DisplayClass20_1.<<ShowUIByNameImp>b__2>d>(UIManager.<>c__DisplayClass20_1.<<ShowUIByNameImp>b__2>d&)
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.JsonUtility.FromJson<object>(string)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform,bool)
		// YooAsset.AssetHandle YooAsset.ResourcePackage.LoadAssetSync<object>(string)
		// YooAsset.AssetHandle YooAsset.YooAssets.LoadAssetSync<object>(string)
	}
}