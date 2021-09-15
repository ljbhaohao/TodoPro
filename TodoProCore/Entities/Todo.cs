using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoProCore.Entities
{
        /// <summary>
        /// 待办事项
        /// </summary>
        public class Todo
        {
            /// <summary>
            /// ID
            /// </summary>
            public Guid Id { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            [Required(ErrorMessage = "名称不能为空")]
            public string Name { get; set; }
        }
}
