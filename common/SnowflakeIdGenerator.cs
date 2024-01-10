using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace irm_wpf.common
{
    /*
    *雪花算法工具类
    */
    public static class SnowflakeIdGenerator
    {
        private static readonly object LockObject = new object();

        private const long Twepoch = 1288834974657L;
        private const int WorkerIdBits = 5;
        private const int DatacenterIdBits = 5;
        private const int SequenceBits = 12;

        private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
        private const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);

        private const int WorkerIdShift = SequenceBits;
        private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
        private const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);

        private static long _workerId;
        private static long _datacenterId;
        private static long _sequence = 0L;
        private static long _lastTimestamp = -1L;

        static SnowflakeIdGenerator()
        {
            // 初始化 workerId 和 datacenterId，可以根据需要修改
            _workerId = 1;
            _datacenterId = 1;
        }

        public static long NextId()
        {
            lock (LockObject)
            {
                long timestamp = GetTimestamp();

                if (timestamp < _lastTimestamp)
                {
                    throw new Exception($"Clock moved backwards. Refusing to generate ID for {_lastTimestamp - timestamp} milliseconds.");
                }

                if (_lastTimestamp == timestamp)
                {
                    _sequence = (_sequence + 1) & SequenceMask;
                    if (_sequence == 0)
                    {
                        timestamp = UntilNextMillis(_lastTimestamp);
                    }
                }
                else
                {
                    _sequence = 0L;
                }

                _lastTimestamp = timestamp;

                return ((timestamp - Twepoch) << TimestampLeftShift) |
                       (_datacenterId << DatacenterIdShift) |
                       (_workerId << WorkerIdShift) |
                       _sequence;
            }
        }

        private static long UntilNextMillis(long lastTimestamp)
        {
            long timestamp = GetTimestamp();
            while (timestamp <= lastTimestamp)
            {
                timestamp = GetTimestamp();
            }
            return timestamp;
        }

        private static long GetTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}