using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"DOTween.dll",
		"Google.Protobuf.dll",
		"HSFramework.dll",
		"Newtonsoft.Json.dll",
		"Serilog.dll",
		"Summer.dll",
		"System.Core.dll",
		"System.dll",
		"UnityEngine.CoreModule.dll",
		"UnityEngine.JSONSerializeModule.dll",
		"YooAsset.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// Common.Summer.Net.MessageRouter.MessageHandler<object>
	// Common.Summer.Tools.Singleton<object>
	// Google.Protobuf.Collections.RepeatedField.<GetEnumerator>d__28<int>
	// Google.Protobuf.Collections.RepeatedField.<GetEnumerator>d__28<object>
	// Google.Protobuf.Collections.RepeatedField<int>
	// Google.Protobuf.Collections.RepeatedField<object>
	// Google.Protobuf.FieldCodec.<>c<int>
	// Google.Protobuf.FieldCodec.<>c<object>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass38_0<int>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass38_0<object>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass39_0<int>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass39_0<object>
	// Google.Protobuf.FieldCodec.InputMerger<int>
	// Google.Protobuf.FieldCodec.InputMerger<object>
	// Google.Protobuf.FieldCodec.ValuesMerger<int>
	// Google.Protobuf.FieldCodec.ValuesMerger<object>
	// Google.Protobuf.FieldCodec<int>
	// Google.Protobuf.FieldCodec<object>
	// Google.Protobuf.IDeepCloneable<int>
	// Google.Protobuf.IDeepCloneable<object>
	// Google.Protobuf.ValueReader<int>
	// Google.Protobuf.ValueReader<object>
	// Google.Protobuf.ValueWriter<int>
	// Google.Protobuf.ValueWriter<object>
	// HSFramework.MySingleton.Singleton<object>
	// HSFramework.PoolModule.UnityObjectPoolFactory.LoadFuncDel<object>
	// Newtonsoft.Json.JsonConverter<object>
	// Res.<>c__DisplayClass4_0<object>
	// Res.<LoadAssetAsyncWithTimeout>d__4<object>
	// Res.ResHandle<object>
	// System.Action<CameraHolder.SVA>
	// System.Action<Entry.MyVec3>
	// System.Action<System.ValueTuple<object,int>>
	// System.Action<UnityEngine.SceneManagement.Scene>
	// System.Action<UnityEngine.Vector3,UnityEngine.Quaternion>
	// System.Action<UnityEngine.Vector3>
	// System.Action<float>
	// System.Action<int>
	// System.Action<object,UnityEngine.Vector3>
	// System.Action<object>
	// System.Collections.Concurrent.ConcurrentDictionary.<GetEnumerator>d__35<int,object>
	// System.Collections.Concurrent.ConcurrentDictionary.<GetEnumerator>d__35<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary.DictionaryEnumerator<int,object>
	// System.Collections.Concurrent.ConcurrentDictionary.DictionaryEnumerator<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Node<int,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Node<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Tables<int,object>
	// System.Collections.Concurrent.ConcurrentDictionary.Tables<object,object>
	// System.Collections.Concurrent.ConcurrentDictionary<int,object>
	// System.Collections.Concurrent.ConcurrentDictionary<object,object>
	// System.Collections.Concurrent.ConcurrentQueue.<Enumerate>d__28<object>
	// System.Collections.Concurrent.ConcurrentQueue.Segment<object>
	// System.Collections.Concurrent.ConcurrentQueue<object>
	// System.Collections.Generic.ArraySortHelper<CameraHolder.SVA>
	// System.Collections.Generic.ArraySortHelper<Entry.MyVec3>
	// System.Collections.Generic.ArraySortHelper<System.ValueTuple<object,int>>
	// System.Collections.Generic.ArraySortHelper<UnityEngine.Vector3>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<CameraHolder.SVA>
	// System.Collections.Generic.Comparer<Entry.MyVec3>
	// System.Collections.Generic.Comparer<System.ValueTuple<object,int>>
	// System.Collections.Generic.Comparer<UnityEngine.Vector3>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary<int,int>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<object,int>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.ICollection<CameraHolder.SVA>
	// System.Collections.Generic.ICollection<Entry.MyVec3>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<System.ValueTuple<object,int>>
	// System.Collections.Generic.ICollection<UnityEngine.Vector3>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<CameraHolder.SVA>
	// System.Collections.Generic.IComparer<Entry.MyVec3>
	// System.Collections.Generic.IComparer<System.ValueTuple<object,int>>
	// System.Collections.Generic.IComparer<UnityEngine.Vector3>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IDictionary<int,object>
	// System.Collections.Generic.IDictionary<object,object>
	// System.Collections.Generic.IEnumerable<CameraHolder.SVA>
	// System.Collections.Generic.IEnumerable<Entry.MyVec3>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<System.ValueTuple<object,int>>
	// System.Collections.Generic.IEnumerable<UnityEngine.Vector3>
	// System.Collections.Generic.IEnumerable<float>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<CameraHolder.SVA>
	// System.Collections.Generic.IEnumerator<Entry.MyVec3>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<System.ValueTuple<object,int>>
	// System.Collections.Generic.IEnumerator<UnityEngine.Vector3>
	// System.Collections.Generic.IEnumerator<float>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IList<CameraHolder.SVA>
	// System.Collections.Generic.IList<Entry.MyVec3>
	// System.Collections.Generic.IList<System.ValueTuple<object,int>>
	// System.Collections.Generic.IList<UnityEngine.Vector3>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.IReadOnlyDictionary<int,object>
	// System.Collections.Generic.IReadOnlyDictionary<object,object>
	// System.Collections.Generic.KeyValuePair<int,int>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<object,int>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.LinkedList.Enumerator<object>
	// System.Collections.Generic.LinkedList<object>
	// System.Collections.Generic.LinkedListNode<object>
	// System.Collections.Generic.List.Enumerator<CameraHolder.SVA>
	// System.Collections.Generic.List.Enumerator<Entry.MyVec3>
	// System.Collections.Generic.List.Enumerator<System.ValueTuple<object,int>>
	// System.Collections.Generic.List.Enumerator<UnityEngine.Vector3>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<CameraHolder.SVA>
	// System.Collections.Generic.List<Entry.MyVec3>
	// System.Collections.Generic.List<System.ValueTuple<object,int>>
	// System.Collections.Generic.List<UnityEngine.Vector3>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<CameraHolder.SVA>
	// System.Collections.Generic.ObjectComparer<Entry.MyVec3>
	// System.Collections.Generic.ObjectComparer<System.ValueTuple<object,int>>
	// System.Collections.Generic.ObjectComparer<UnityEngine.Vector3>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.Queue.Enumerator<ItemIOInfoSt>
	// System.Collections.Generic.Queue<ItemIOInfoSt>
	// System.Collections.ObjectModel.ReadOnlyCollection<CameraHolder.SVA>
	// System.Collections.ObjectModel.ReadOnlyCollection<Entry.MyVec3>
	// System.Collections.ObjectModel.ReadOnlyCollection<System.ValueTuple<object,int>>
	// System.Collections.ObjectModel.ReadOnlyCollection<UnityEngine.Vector3>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<CameraHolder.SVA>
	// System.Comparison<Entry.MyVec3>
	// System.Comparison<System.ValueTuple<object,int>>
	// System.Comparison<UnityEngine.Vector3>
	// System.Comparison<int>
	// System.Comparison<object>
	// System.EventHandler<object>
	// System.Func<System.ValueTuple<object,int>,byte>
	// System.Func<int,int>
	// System.Func<int,object,object>
	// System.Func<int,object>
	// System.Func<object,System.ValueTuple<object,int>>
	// System.Func<object,byte>
	// System.Func<object,int,object>
	// System.Func<object,int>
	// System.Func<object,object,object>
	// System.Func<object,object>
	// System.Linq.Buffer<object>
	// System.Linq.Enumerable.<CastIterator>d__99<object>
	// System.Linq.Enumerable.<OfTypeIterator>d__97<object>
	// System.Linq.Enumerable.<SelectIterator>d__5<object,object>
	// System.Linq.Enumerable.Iterator<System.ValueTuple<object,int>>
	// System.Linq.Enumerable.Iterator<object>
	// System.Linq.Enumerable.WhereArrayIterator<object>
	// System.Linq.Enumerable.WhereEnumerableIterator<System.ValueTuple<object,int>>
	// System.Linq.Enumerable.WhereEnumerableIterator<object>
	// System.Linq.Enumerable.WhereListIterator<object>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,System.ValueTuple<object,int>>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,System.ValueTuple<object,int>>
	// System.Linq.Enumerable.WhereSelectListIterator<object,System.ValueTuple<object,int>>
	// System.Linq.EnumerableSorter<object,int>
	// System.Linq.EnumerableSorter<object>
	// System.Linq.OrderedEnumerable.<GetEnumerator>d__1<object>
	// System.Linq.OrderedEnumerable<object,int>
	// System.Linq.OrderedEnumerable<object>
	// System.Nullable<UnityEngine.Color>
	// System.Predicate<CameraHolder.SVA>
	// System.Predicate<Entry.MyVec3>
	// System.Predicate<System.ValueTuple<object,int>>
	// System.Predicate<UnityEngine.Vector3>
	// System.Predicate<int>
	// System.Predicate<object>
	// System.ValueTuple<object,int>
	// System.ValueTuple<object,object>
	// UnityEngine.Events.InvokableCall<float>
	// UnityEngine.Events.InvokableCall<int>
	// UnityEngine.Events.UnityAction<float>
	// UnityEngine.Events.UnityAction<int>
	// UnityEngine.Events.UnityEvent<float>
	// UnityEngine.Events.UnityEvent<int>
	// }}

	public void RefMethods()
	{
		// System.Void Common.Summer.Net.MessageRouter.Subscribe<object>(Common.Summer.Net.MessageRouter.MessageHandler<object>)
		// System.Void Common.Summer.Net.MessageRouter.UnSubscribe<object>(Common.Summer.Net.MessageRouter.MessageHandler<object>)
		// bool Common.Summer.Net.ProtoHelper.Register<object>(int)
		// object DG.Tweening.TweenSettingsExtensions.From<object>(object)
		// object DG.Tweening.TweenSettingsExtensions.From<object>(object,bool,bool)
		// object DG.Tweening.TweenSettingsExtensions.OnComplete<object>(object,DG.Tweening.TweenCallback)
		// object DG.Tweening.TweenSettingsExtensions.SetAutoKill<object>(object,bool)
		// object DG.Tweening.TweenSettingsExtensions.SetEase<object>(object,DG.Tweening.Ease)
		// bool HSFramework.AI.StateMachine.StateMachine.ChangeState<object>(bool)
		// HSFramework.AI.StateMachine.StateBase HSFramework.AI.StateMachine.StateMachine.GetState<object>()
		// object HSFramework.PoolModule.PoolAssetLoad.LoadAssetByYoo<object>(string)
		// object Newtonsoft.Json.JsonConvert.DeserializeObject<object>(string)
		// object Newtonsoft.Json.JsonConvert.DeserializeObject<object>(string,Newtonsoft.Json.JsonSerializerSettings)
		// System.Collections.IEnumerator Res.LoadAssetAsyncWithTimeout<object>(string,System.Action<object>,float)
		// object Res.LoadAssetSync<object>(string,uint)
		// System.Void Serilog.ILogger.Write<UnityEngine.Vector3>(Serilog.Events.LogEventLevel,string,UnityEngine.Vector3)
		// System.Void Serilog.ILogger.Write<int,int>(Serilog.Events.LogEventLevel,string,int,int)
		// System.Void Serilog.ILogger.Write<int>(Serilog.Events.LogEventLevel,string,int)
		// System.Void Serilog.ILogger.Write<object>(Serilog.Events.LogEventLevel,string,object)
		// System.Void Serilog.Log.Error<object>(string,object)
		// System.Void Serilog.Log.Information<UnityEngine.Vector3>(string,UnityEngine.Vector3)
		// System.Void Serilog.Log.Warning<int,int>(string,int,int)
		// System.Void Serilog.Log.Warning<int>(string,int)
		// System.Void Serilog.Log.Write<UnityEngine.Vector3>(Serilog.Events.LogEventLevel,string,UnityEngine.Vector3)
		// System.Void Serilog.Log.Write<int,int>(Serilog.Events.LogEventLevel,string,int,int)
		// System.Void Serilog.Log.Write<int>(Serilog.Events.LogEventLevel,string,int)
		// System.Void Serilog.Log.Write<object>(Serilog.Events.LogEventLevel,string,object)
		// object System.Activator.CreateInstance<object>()
		// object[] System.Array.Empty<object>()
		// object System.Collections.Generic.CollectionExtensions.GetValueOrDefault<int,object>(System.Collections.Generic.IReadOnlyDictionary<int,object>,int)
		// object System.Collections.Generic.CollectionExtensions.GetValueOrDefault<int,object>(System.Collections.Generic.IReadOnlyDictionary<int,object>,int,object)
		// object System.Collections.Generic.CollectionExtensions.GetValueOrDefault<object,object>(System.Collections.Generic.IReadOnlyDictionary<object,object>,object,object)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Cast<object>(System.Collections.IEnumerable)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.CastIterator<object>(System.Collections.IEnumerable)
		// object System.Linq.Enumerable.FirstOrDefault<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.OfType<object>(System.Collections.IEnumerable)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.OfTypeIterator<object>(System.Collections.IEnumerable)
		// System.Linq.IOrderedEnumerable<object> System.Linq.Enumerable.OrderBy<object,int>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int>)
		// System.Collections.Generic.IEnumerable<System.ValueTuple<object,int>> System.Linq.Enumerable.Select<object,System.ValueTuple<object,int>>(System.Collections.Generic.IEnumerable<object>,System.Func<object,System.ValueTuple<object,int>>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Select<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int,object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.SelectIterator<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,int,object>)
		// object[] System.Linq.Enumerable.ToArray<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.List<System.ValueTuple<object,int>> System.Linq.Enumerable.ToList<System.ValueTuple<object,int>>(System.Collections.Generic.IEnumerable<System.ValueTuple<object,int>>)
		// System.Collections.Generic.List<object> System.Linq.Enumerable.ToList<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Where<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Collections.Generic.IEnumerable<System.ValueTuple<object,int>> System.Linq.Enumerable.Iterator<object>.Select<System.ValueTuple<object,int>>(System.Func<object,System.ValueTuple<object,int>>)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,EntryGameWorldService.<>c__DisplayClass11_0.<<_HandleEnterGameResponse>b__2>d>(System.Runtime.CompilerServices.TaskAwaiter&,EntryGameWorldService.<>c__DisplayClass11_0.<<_HandleEnterGameResponse>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<EntryGameWorldService.<>c__DisplayClass11_0.<<_HandleEnterGameResponse>b__2>d>(EntryGameWorldService.<>c__DisplayClass11_0.<<_HandleEnterGameResponse>b__2>d&)
		// object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<object>(object&)
		// object UnityEngine.Component.GetComponent<object>()
		// object UnityEngine.Component.GetComponentInChildren<object>()
		// object UnityEngine.Component.GetComponentInParent<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>(bool)
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>()
		// object UnityEngine.GameObject.GetComponentInChildren<object>(bool)
		// object UnityEngine.GameObject.GetComponentInParent<object>()
		// object UnityEngine.GameObject.GetComponentInParent<object>(bool)
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>()
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>(bool)
		// bool UnityEngine.GameObject.TryGetComponent<object>(object&)
		// object UnityEngine.JsonUtility.FromJson<object>(string)
		// object UnityEngine.Object.Instantiate<object>(object)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform,bool)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Vector3,UnityEngine.Quaternion)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
		// YooAsset.AssetHandle YooAsset.ResourcePackage.LoadAssetSync<object>(string)
	}
}