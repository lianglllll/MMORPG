using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

    public class Hash
    {

        /// <summary>
        /// SDBM 这个算法在开源的SDBM中使用，似乎对很多不同类型的数据都能得到不错的分布
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int GetSDBMHash(string text)
        {
            int hash = 0;
            for (int i = 0; i < text.Length; i++)
            {
                hash = text[i] + (hash << 6) + (hash << 16) - hash;
            }
            //这里需要注意的是，如果hash值为负数，那么hash & 0x7FFFFFFF将会得到一个正数
            return (hash & 0x7FFFFFFF);
        }

        /// <summary>
        /// 这个算法来自Brian Kernighan 和 Dennis Ritchie的 The C Programming Language。
        /// 这是一个很简单的哈希算法,使用了一系列奇怪的数字,形式如31,3131,31...31,
        /// 看上去和DJB算法很相似
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetBKDRHash(string str)
        {
            int seed = 131; // 31 131 1313 13131 131313 etc..
            int hash = 0;
            for (int i = 0; i < str.Length; i++)
            {
                hash = (hash * seed) + str[i];
            }
            return (hash & 0x7FFFFFFF);
        }

        /// <summary>
        /// DJB 这个算法是Daniel J.Bernstein 教授发明的，是目前公布的最有效的哈希函数。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetDJBHash(string str)
        {
            int hash = 5381;
            for (int i = 0; i < str.Length; i++)
            {
                hash += (hash << 5) + str[i];
            }
            return (hash & 0x7FFFFFFF);
        }

    }



}
