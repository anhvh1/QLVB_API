using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Workflow.Model
{
    public class DocumentModel
    {
        public int DocumentID { get; set; }
        public int WorkflowID { get; set; }
        public int StateID { get; set; }
        public int CoQuanID { get; set; }
        public Nullable<DateTime> DueDate { get; set; }
        public int ProcessCount { get; set; }
    }
}
