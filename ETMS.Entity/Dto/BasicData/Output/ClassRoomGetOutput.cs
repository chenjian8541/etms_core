using System;
using System.Collections.Generic;
using System.Text;

namespace ETMS.Entity.Dto.BasicData.Output
{
    public class ClassRoomGetOutput
    {
        public List<ClassRoomViewOutput> ClassRooms { get; set; }
    }

    public class ClassRoomViewOutput
    {
        public long CId { get; set; }

        public string Name { get; set; }
    }
}
