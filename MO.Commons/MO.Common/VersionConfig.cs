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
                int original, newValue;
                do
                {
                    original = _currversion;
                    newValue = original + 1 < _maxVersion ? original + 1 : _minVersion;
                }
                while (Interlocked.CompareExchange(ref _currversion, newValue, original) != original);
                
                return newValue;
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