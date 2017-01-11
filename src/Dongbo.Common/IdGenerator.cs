using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Dongbo.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class IdGenerator
    {
        private static readonly Dictionary<string, IIdGen> GenDic = new Dictionary<string, IIdGen>();

        /// <summary>
        /// 生成一个48位时间有序的id 
        /// </summary>
        /// <returns></returns>
        public static long NextId(int workerId = 0)
        {
            IIdGen worker = null;
            string key = "long-idGen-" + workerId;
            lock (GenDic)
            {
                if (GenDic.ContainsKey(key))
                    worker = GenDic[key] as IIdGen;
                else
                {
                    worker = new IdWorker((long)workerId);
                    GenDic.Add(key, worker);
                }
            }
            long id = worker.NextId();
            return id;
        }

        public static long NextImgId()
        {
            IIdGen worker = null;
            string key = "long-idGen-7";
            lock (GenDic)
            {
                if (GenDic.ContainsKey(key))
                    worker = GenDic[key] as IIdGen;
                else
                {
                    worker = new IdWorker((long)7);
                    GenDic.Add(key, worker);
                }
            }
            long id = worker.NextId();
            return id;
        }
        public static string NextOrderCode(int workerId = 0)
        {
            if (workerId > 16)
                throw new ArgumentOutOfRangeException("workerId must <=16");
            IIdGen worker = null;
            string key = "string-idGen-" + workerId;
            lock (GenDic)
            {
                if (GenDic.ContainsKey(key))
                    worker = GenDic[key] as IIdGen;
                else
                {
                    worker = new OrderCodeWorker(workerId);
                    GenDic.Add(key, worker);
                }
            }
            var id = worker.NextStringId();
            return id;
        }


    }

    internal interface IIdGen
    {
        long NextId();
        string NextStringId();
    }
    internal class OrderCodeWorker : IIdGen
    {
        int workerId = 0;
        private long lastTimestamp = -1L;
        private long sequence = 0L;
        private const int sequenceBits = 12;
        private const long sequenceMask = -1L ^ -1L << sequenceBits;//4095

        private static readonly object _locker = new object();
        public OrderCodeWorker(int worker)
        {
            workerId = worker;
        }
        public string NextStringId()
        {
            lock (_locker)
            {
                DateTime current = DateTime.Now;
                long timestamp = this.timeGen(current);
                if (this.lastTimestamp == timestamp)//当前(毫)秒内自增
                {
                    Interlocked.Increment(ref this.sequence);
                    this.sequence = this.sequence & sequenceMask;
                    if (this.sequence == 0)
                    {
                        timestamp = this.tilNextMillis(this.lastTimestamp);
                    }
                }
                else
                {
                    this.sequence = 0;
                }
                if (timestamp < this.lastTimestamp)
                {
                    throw new IdGeneratorException(String
                        .Format("Clock moved backwards.  Refusing to generate id for {0} milliseconds",
                            (this.lastTimestamp - timestamp)));
                }

                this.lastTimestamp = timestamp;

                string TimeSection = current.ToString("yyyyMMddHHmmss");
                string workerIdSection = workerId.ToString("D2");
                return string.Format("{0}{1}{2}", TimeSection, workerIdSection, this.sequence.ToString("D4"));
            }
        }
        private long tilNextMillis(long lastTimestamp)
        {
            long timestamp = this.timeGen(DateTime.Now);
            while (timestamp <= lastTimestamp)
            {
                timestamp = this.timeGen(DateTime.Now);
            }
            return timestamp;
        }


        private long timeGen(DateTime now)
        {

            return now.CurrentTimeSeconds();
        }

        long IIdGen.NextId()
        {
            throw new NotImplementedException();
        }

    }

    /// <summary>
    /// 00000000 00000000 00000000 00000000-0000-00000000 0000 
    /// 32时间(当前时间到2013-10-1 的秒数 约100+年)+4机器+12计数
    /// twitter 全局id 生成算法 snowflake;整体上按照时间自增排序，并且整个分布式系统内不会产生ID碰撞
    /// <see cref="> https://github.com/twitter/snowflake"/>
    /// @author zhigang.hao 2013-10-21
    /// </summary>
    /// <remarks>
    /// snowflack算法如下：
    /// 0---0000000000 0000000000 0000000000 0000000000 0 --- 00000 ---00000 ---0000000000 00
    /// 在上面的字符串中，第一位为未使用（实际上也可作为long的符号位），接下来的41位为毫秒级时间，
    /// 然后5位datacenter标识位，5位机器ID（并不算标识符，实际是为线程标识），
    /// 然后12位该毫秒内的当前毫秒内的计数，加起来刚好64位，为一个Long型
    /// </remarks>
    internal class IdWorker : IIdGen
    {
        //32时间+4机器+12计数
        private long workerId;
        private const long _epoch = 432000L;//2013.10.1-2013.9.25
        private const int workerIdBits = 4;//max=8
        private const int sequenceBits = 12;
        private const long sequenceMask = -1L ^ -1L << sequenceBits;//4095
        private const int workerIdShift = 12;
        private const int timestampLeftShift = sequenceBits + workerIdBits;//前32位表示时间

        private long sequence = 0L;
        private long maxWorkerId = 15L;
        private long lastTimestamp = -1L;

        private static readonly object _locker = new object();

        public IdWorker(long workerId)
        {
            this.maxWorkerId = -1L ^ -1L << workerIdBits;
            if (workerId > this.maxWorkerId || workerId < 0)
            {
                throw new ArgumentOutOfRangeException(String.Format("worker Id can't be greater than {0} or less than 0",
                    this.maxWorkerId));
            }
            this.workerId = workerId;


        }


        public long NextId()
        {
            lock (_locker)
            {
                long timestamp = this.timeGen();
                if (this.lastTimestamp == timestamp)//当前(毫)秒内自增
                {
                    Interlocked.Increment(ref this.sequence);
                    this.sequence = this.sequence & sequenceMask;
                    if (this.sequence == 0)
                    {
                        timestamp = this.tilNextMillis(this.lastTimestamp);
                    }
                }
                else
                {
                    this.sequence = 0;
                }
                if (timestamp < this.lastTimestamp)
                {
                    throw new IdGeneratorException(String
                        .Format("Clock moved backwards.  Refusing to generate id for {0} milliseconds",
                            (this.lastTimestamp - timestamp)));
                }

                this.lastTimestamp = timestamp;
                return timestamp - _epoch << timestampLeftShift | this.workerId << workerIdShift
                        | this.sequence;
            }
        }


        private long tilNextMillis(long lastTimestamp)
        {
            long timestamp = this.timeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = this.timeGen();
            }
            return timestamp;
        }


        private long timeGen()
        {

            return DateTime.Now.CurrentTimeSeconds();
        }



        string IIdGen.NextStringId()
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class IdGeneratorException : ApplicationException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public IdGeneratorException(string message)
            : base(message)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public IdGeneratorException(string message, Exception innerException) : base(message, innerException) { }
    }

    internal static class DateTimeExtensions
    {
        private static DateTime Jan1st2013101 = new DateTime(2013, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        public static long CurrentTimeMillis(this DateTime d)
        {
            return (long)((DateTime.UtcNow - Jan1st2013101).TotalMilliseconds);
        }
        public static long CurrentTimeSeconds(this DateTime d)
        {
            //DateTime.UtcNow
            return (long)((d.ToUniversalTime() - Jan1st2013101).TotalSeconds);
        }
    }
}
