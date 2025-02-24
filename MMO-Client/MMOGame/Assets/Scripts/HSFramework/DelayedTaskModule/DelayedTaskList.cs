using System;
using System.Collections;
using System.Collections.Generic;

namespace HSFramework.MyDelayedTaskScheduler
{
    public class DelayedTaskList : IComparable, IEnumerable<DelayedTaskData>, IDisposable
    {
        private bool _disposed = false;
        public long Time;
        public List<DelayedTaskData> DelayedTaskDataList;

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            return CompareTo((DelayedTaskList)obj);
        }

        public int CompareTo(DelayedTaskList obj)
        {
            return Time.CompareTo(obj.Time);
        }

        IEnumerator<DelayedTaskData> IEnumerable<DelayedTaskData>.GetEnumerator()
        {
            return DelayedTaskDataList.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return DelayedTaskDataList.GetEnumerator();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                DelayedTaskDataList.Clear();
                DelayedTaskDataList = null;
            }
            _disposed = true;
        }
        
        ~DelayedTaskList()
        {
            Dispose(false);
        }
    }
}
