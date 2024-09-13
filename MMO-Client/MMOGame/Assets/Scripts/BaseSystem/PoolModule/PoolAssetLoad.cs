using UnityEngine;

namespace BaseSystem.PoolModule
{
    public static class PoolAssetLoad
    {
        public static T LoadAssetByResource<T>(string path) where T : UnityEngine.Object
        {
            return Resources.Load<T>(path);
        }

        public static T LoadAssetByYoo<T>(string path) where T : UnityEngine.Object
        {
            return Res.LoadAssetSync<T>(path);
        }
    }
}
