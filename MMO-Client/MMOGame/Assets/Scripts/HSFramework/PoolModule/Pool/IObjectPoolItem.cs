namespace BaseSystem.PoolModule
{
    public interface IObjectPoolItem
    {
        void OnGetHandle();
        void OnRecycleHandle();
    }
}
