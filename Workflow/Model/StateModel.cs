using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Workflow.Model
{
    public class StateModel
    {
        public int StateID { get; set; }
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public int WorkFlowID { get; set; }
        public string WorkFlowCode { get; set; }
    }
}
