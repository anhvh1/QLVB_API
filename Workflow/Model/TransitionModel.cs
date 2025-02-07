using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Workflow.Model
{
    public class TransitionModel
    {
        public int TransitionID { get; set; }
        public int CurrentStateID { get; set; }
        public int NextStateID { get; set; }
        public int CommandID { get; set; }
    }
}
