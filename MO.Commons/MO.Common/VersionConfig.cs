using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MO.Common
{
    /// <summary>
    /// 版本信息
    /// </summary>
    public class VersionConfig
    {
        private readonly int _minVersion;
        private readonly int _maxVersion;
        private int _currversion;
        /// <summary>
        /// 
        /// </summary>
        public VersionConfig()
            : this(0, int.MaxValue)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="minVersion"></param>
        /// <param name="maxVersion"></param>
        public VersionConfig(int minVersion, int maxVersion)
        {
            _minVersion = minVersion;
            _maxVersion = maxVersion;
            _currversion = _minVersion;
        }

        /// <summary>
        /// 取下一个版本号
        /// </summary>
        public int NextId
        {
            get
            {
                Interlocked.Exchange(ref _currversion, _currversion + 1 < _maxVersion ? _currversion + 1 : _minVersion);
                return _currversion;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            get
            {
                return _currversion;
            }
        }
    }
}