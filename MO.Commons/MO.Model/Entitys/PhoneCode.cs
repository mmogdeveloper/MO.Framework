using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MO.Model.Entitys
{
    public class PhoneCode : BaseEntity
    {
        [MaxLength(20)]
        public string PhoneNum { get; set; }

        [MaxLength(2)]
        public int PhoneType { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
