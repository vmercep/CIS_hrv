using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class SysLog
    {
        public string Comment { get; set; }
        public int Id { get; set; }
        public string ObjAfter { get; set; }
        public string ObjBefore { get; set; }
        public int ObjId { get; set; }
    }
}
