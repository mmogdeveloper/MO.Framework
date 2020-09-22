using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MO.Model.Entitys
{
    public class GameUser : BaseEntity
    {
        public long UserId { get; set; }

        [MaxLength(50)]
        public string NickName { get; set; }

        [MaxLength(100)]
        public string HeadIcon { get; set; }

        [MaxLength(50)]
        [DefaultValue("")]
        public string DeviceId { get; set; }

        [MaxLength(20)]
        public string PhoneNum { get; set; }

        [MaxLength(100)]
        public string Password { get; set; }

        [MaxLength(50)]
        public string WeiXinCode { get; set; }
    }
}
