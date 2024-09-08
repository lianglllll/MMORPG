using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"Assembly-CSharp-firstpass.dll",
		"DOTween.dll",
		"Google.Protobuf.dll",
		"Main.dll",
		"Newtonsoft.Json.dll",
		"Serilog.dll",
		"System.Core.dll",
		"System.dll",
		"Unity.InputSystem.dll",
		"UnityEngine.CoreModule.dll",
		"UnityEngine.JSONSerializeModule.dll",
		"YooAsset.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// Google.Protobuf.Collections.RepeatedField.<GetEnumerator>d__28<object>
	// Google.Protobuf.Collections.RepeatedField<object>
	// Google.Protobuf.FieldCodec.<>c<object>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass38_0<object>
	// Google.Protobuf.FieldCodec.<>c__DisplayClass39_0<object>
	// Google.Protobuf.FieldCodec.InputMerger<object>
	// Google.Protobuf.FieldCodec.ValuesMerger<object>
	// Google.Protobuf.FieldCodec<object>
	// Google.Protobuf.IDeepCloneable<object>
	// Google.Protobuf.IMessage<object>
	// Google.Protobuf.MessageParser.<>c__DisplayClass2_0<object>
	// Google.Protobuf.MessageParser<object>
	// Google.Protobuf.ValueReader<object>
	// Google.Protobuf.ValueWriter<object>
	// Newtonsoft.Json.JsonConverter<object>
	// Res.<>c__DisplayClass4_0<object>
	// Res.<LoadAssetAsyncWithTimeout>d__4<object>
	// Res.ResHandle<UnityEngine.SceneManagement.Scene>
	// Summer.Network.MessageRouter.MessageHandler<object>
	// Summer.Singleton<object>
	// System.Action<CameraHolder.SVA>
	// System.Action<Entry.MyVec3>
	// System.Action<UnityEngine.InputSystem.InputAction.CallbackContext>
	// System.Action<UnityEngine.SceneManagement.Scene>
	// System.Action<byte>
	// System.Action<float>
	// System.Action<int>
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
	// System.Collections.Generic.ArraySortHelper<byte>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<CameraHolder.SVA>
	// System.Collections.Generic.Comparer<Entry.MyVec3>
	// System.Collections.Generic.Comparer<byte>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,int>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,int>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary<int,int>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.EqualityComparer<TimeMachineMgr.TimerInfo>
	// System.Collections.Generic.EqualityComparer<TimeMachineMgr2.TimerInfo2>
	// System.Collections.Generic.EqualityComparer<double>
	// System.Collections.Generic.EqualityComparer<float>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.ICollection<CameraHolder.SVA>
	// System.Collections.Generic.ICollection<Entry.MyVec3>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<byte>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<CameraHolder.SVA>
	// System.Collections.Generic.IComparer<Entry.MyVec3>
	// System.Collections.Generic.IComparer<byte>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IDictionary<int,object>
	// System.Collections.Generic.IDictionary<object,object>
	// System.Collections.Generic.IEnumerable<CameraHolder.SVA>
	// System.Collections.Generic.IEnumerable<Entry.MyVec3>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<TimeMachineMgr.TimerInfo>
	// System.Collections.Generic.IEnumerable<TimeMachineMgr2.TimerInfo2>
	// System.Collections.Generic.IEnumerable<byte>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<CameraHolder.SVA>
	// System.Collections.Generic.IEnumerator<Entry.MyVec3>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<TimeMachineMgr.TimerInfo>
	// System.Collections.Generic.IEnumerator<TimeMachineMgr2.TimerInfo2>
	// System.Collections.Generic.IEnumerator<byte>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IList<CameraHolder.SVA>
	// System.Collections.Generic.IList<Entry.MyVec3>
	// System.Collections.Generic.IList<byte>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.IReadOnlyDictionary<int,object>
	// System.Collections.Generic.IReadOnlyDictionary<object,object>
	// System.Collections.Generic.KeyValuePair<int,int>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.LinkedList.Enumerator<TimeMachineMgr.TimerInfo>
	// System.Collections.Generic.LinkedList.Enumerator<TimeMachineMgr2.TimerInfo2>
	// System.Collections.Generic.LinkedList.Enumerator<object>
	// System.Collections.Generic.LinkedList<TimeMachineMgr.TimerInfo>
	// System.Collections.Generic.LinkedList<TimeMachineMgr2.TimerInfo2>
	// System.Collections.Generic.LinkedList<object>
	// System.Collections.Generic.LinkedListNode<TimeMachineMgr.TimerInfo>
	// System.Collections.Generic.LinkedListNode<TimeMachineMgr2.TimerInfo2>
	// System.Collections.Generic.LinkedListNode<object>
	// System.Collections.Generic.List.Enumerator<CameraHolder.SVA>
	// System.Collections.Generic.List.Enumerator<Entry.MyVec3>
	// System.Collections.Generic.List.Enumerator<byte>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<CameraHolder.SVA>
	// System.Collections.Generic.List<Entry.MyVec3>
	// System.Collections.Generic.List<byte>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<CameraHolder.SVA>
	// System.Collections.Generic.ObjectComparer<Entry.MyVec3>
	// System.Collections.Generic.ObjectComparer<byte>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<TimeMachineMgr.TimerInfo>
	// System.Collections.Generic.ObjectEqualityComparer<TimeMachineMgr2.TimerInfo2>
	// System.Collections.Generic.ObjectEqualityComparer<double>
	// System.Collections.Generic.ObjectEqualityComparer<float>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.Queue.Enumerator<ItemIOInfoSt>
	// System.Collections.Generic.Queue.Enumerator<object>
	// System.Collections.Generic.Queue<ItemIOInfoSt>
	// System.Collections.Generic.Queue<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<CameraHolder.SVA>
	// System.Collections.ObjectModel.ReadOnlyCollection<Entry.MyVec3>
	// System.Collections.ObjectModel.ReadOnlyCollection<byte>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<CameraHolder.SVA>
	// System.Comparison<Entry.MyVec3>
	// System.Comparison<byte>
	// System.Comparison<int>
	// System.Comparison<object>
	// System.EventHandler<object>
	// System.Func<int,object,object>
	// System.Func<int,object>
	// System.Func<object,byte>
	// System.Func<object,int>
	// System.Func<object,object,object>
	// System.Func<object,object>
	// System.Func<object>
	// System.IEquatable<object>
	// System.Linq.Enumerable.<OfTypeIterator>d__97<object>
	// System.Linq.Enumerable.Iterator<object>
	// System.Linq.Enumerable.WhereArrayIterator<object>
	// System.Linq.Enumerable.WhereEnumerableIterator<object>
	// System.Linq.Enumerable.WhereListIterator<object>
	// System.Nullable<UnityEngine.Color>
	// System.Predicate<CameraHolder.SVA>
	// System.Predicate<Entry.MyVec3>
	// System.Predicate<byte>
	// System.Predicate<int>
	// System.Predicate<object>
	// UnityEngine.InputSystem.InputBindingComposite<UnityEngine.Vector2>
	// UnityEngine.InputSystem.InputControl<UnityEngine.Vector2>
	// UnityEngine.InputSystem.InputProcessor<UnityEngine.Vector2>
	// UnityEngine.InputSystem.Utilities.InlinedArray<object>
	// }}

	public void RefMethods()
	{
		// object DG.Tweening.TweenSettingsExtensions.From<object>(object)
		// object DG.Tweening.TweenSettingsExtensions.From<object>(object,bool,bool)
		// object DG.Tweening.TweenSettingsExtensions.OnComplete<object>(object,DG.Tweening.TweenCallback)
		// object DG.Tweening.TweenSettingsExtensions.SetEase<object>(object,DG.Tweening.Ease)
		// object Newtonsoft.Json.JsonConvert.DeserializeObject<object>(string)
		// object Newtonsoft.Json.JsonConvert.DeserializeObject<object>(string,Newtonsoft.Json.JsonSerializerSettings)
		// System.Collections.IEnumerator Res.LoadAssetAsyncWithTimeout<object>(string,System.Action<object>,float)
		// object Res.LoadAssetSync<object>(string,FileType,uint)
		// System.Void Serilog.ILogger.Write<UnityEngine.Vector3>(Serilog.Events.LogEventLevel,string,UnityEngine.Vector3)
		// System.Void Serilog.ILogger.Write<object>(Serilog.Events.LogEventLevel,string,object)
		// System.Void Serilog.Log.Error<object>(string,object)
		// System.Void Serilog.Log.Information<UnityEngine.Vector3>(string,UnityEngine.Vector3)
		// System.Void Serilog.Log.Write<UnityEngine.Vector3>(Serilog.Events.LogEventLevel,string,UnityEngine.Vector3)
		// System.Void Serilog.Log.Write<object>(Serilog.Events.LogEventLevel,string,object)
		// System.Void Summer.Network.MessageRouter.Off<object>(Summer.Network.MessageRouter.MessageHandler<object>)
		// System.Void Summer.Network.MessageRouter.Subscribe<object>(Summer.Network.MessageRouter.MessageHandler<object>)
		// object System.Activator.CreateInstance<object>()
		// object[] System.Array.Empty<object>()
		// object System.Collections.Generic.CollectionExtensions.GetValueOrDefault<int,object>(System.Collections.Generic.IReadOnlyDictionary<int,object>,int)
		// object System.Collections.Generic.CollectionExtensions.GetValueOrDefault<int,object>(System.Collections.Generic.IReadOnlyDictionary<int,object>,int,object)
		// object System.Collections.Generic.CollectionExtensions.GetValueOrDefault<object,object>(System.Collections.Generic.IReadOnlyDictionary<object,object>,object,object)
		// bool System.Collections.Generic.CollectionExtensions.Remove<int,object>(System.Collections.Generic.IDictionary<int,object>,int,object&)
		// object System.Linq.Enumerable.FirstOrDefault<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.OfType<object>(System.Collections.IEnumerable)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.OfTypeIterator<object>(System.Collections.IEnumerable)
		// System.Collections.Generic.List<object> System.Linq.Enumerable.ToList<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Where<object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,bool>)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,CombatService.<>c__DisplayClass2_0.<<_SpaceEnterResponse>b__1>d>(System.Runtime.CompilerServices.TaskAwaiter&,CombatService.<>c__DisplayClass2_0.<<_SpaceEnterResponse>b__1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,CombatService.<>c__DisplayClass2_0.<<_SpaceEnterResponse>b__2>d>(System.Runtime.CompilerServices.TaskAwaiter&,CombatService.<>c__DisplayClass2_0.<<_SpaceEnterResponse>b__2>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<CombatService.<>c__DisplayClass2_0.<<_SpaceEnterResponse>b__1>d>(CombatService.<>c__DisplayClass2_0.<<_SpaceEnterResponse>b__1>d&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<CombatService.<>c__DisplayClass2_0.<<_SpaceEnterResponse>b__2>d>(CombatService.<>c__DisplayClass2_0.<<_SpaceEnterResponse>b__2>d&)
		// System.Void* Unity.Collections.LowLevel.Unsafe.UnsafeUtility.AddressOf<UnityEngine.Vector2>(UnityEngine.Vector2&)
		// int Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<UnityEngine.Vector2>()
		// object UnityEngine.Component.GetComponent<object>()
		// object UnityEngine.Component.GetComponentInChildren<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>()
		// object[] UnityEngine.Component.GetComponentsInChildren<object>(bool)
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.GameObject.GetComponentInParent<object>()
		// object UnityEngine.GameObject.GetComponentInParent<object>(bool)
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>()
		// object[] UnityEngine.GameObject.GetComponentsInChildren<object>(bool)
		// UnityEngine.Vector2 UnityEngine.InputSystem.InputAction.ReadValue<UnityEngine.Vector2>()
		// UnityEngine.Vector2 UnityEngine.InputSystem.InputActionState.ReadValue<UnityEngine.Vector2>(int,int,bool)
		// object UnityEngine.JsonUtility.FromJson<object>(string)
		// object UnityEngine.Object.FindObjectOfType<object>()
		// object UnityEngine.Object.Instantiate<object>(object)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Transform,bool)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Vector3,UnityEngine.Quaternion)
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
		// YooAsset.AssetHandle YooAsset.ResourcePackage.LoadAssetSync<object>(string)
	}
}