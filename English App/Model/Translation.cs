using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace English_App.Model
{
    public class Translation
    {
        public string MainTranslate { get; set; }
        public string Defination { get; set; }
        public string SubTranslate { get; set; }
        public string Example { get; set; }

        public void Clear()
        { 
            MainTranslate = string.Empty;
            Defination = string.Empty;
            SubTranslate = string.Empty;
            Example = string.Empty;
        }
    }
}
