using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MO.Model.Entitys
{
    public enum LoginType
    {
        /// <summary>
        /// 游客登录
        /// </summary>
        None,
        /// <summary>
        /// 微信登录
        /// </summary>
        WeiXin = 1,
        /// <summary>
        /// 闲聊登录
        /// </summary>
        XianLiao,
        /// <summary>
        /// 手机密码
        /// </summary>
        PhonePwd,
        /// <summary>
        /// 手机验证码
        /// </summary>
        PhoneCode
    }

    public class LoginRecord : BaseEntity
    {
        public long UserId { get; set; }

        [MaxLength(2)]
        public LoginType LoginType { get; set; }

        [MaxLength(20)]
        public string LoginIP { get; set; }

        [MaxLength(20)]
        public string LoginDevice { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}
