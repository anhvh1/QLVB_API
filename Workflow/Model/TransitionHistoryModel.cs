using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Workflow.Model
{
    public class TransitionHistoryModel
    {
        public int TransactionHistoryID { get; set; }
        public int DocumentID { get; set; }
        public int TransitionID { get; set; }
        public DateTime DueDate { get; set; }
        public string Comment { get; set; }
        public int UserID { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int CoQuanID { get; set; }
    }

    public class ResponeseModel
    {
        public bool response { get; set; }
        public int TransactionHistoryID { get; set; }
    }
}
