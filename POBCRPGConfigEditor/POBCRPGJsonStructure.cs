using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POBCRPGConfigEditor
{
    public class POBCRPGJsonStructure
    {

        public class Rootobject
        {
            public bool Display { get; set; }
            public List<Rpglist> RPGList { get; set; }
        }

        public class Rpglist
        {
            public string Group { get; set; }
            public string GoGroup { get; set; }
            public List<string> Co { get; set; }
            public long C { get; set; }
        }

    }
}
