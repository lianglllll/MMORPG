namespace HSFramework.PoolModule
{
    public interface IObjectPoolItem
    {
        void OnGetHandle();
        void OnRecycleHandle();
    }
}
