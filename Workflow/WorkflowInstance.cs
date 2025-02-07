using System;
using System.Collections.Generic;
using Workflow.DatabaseProxy;
using Workflow.Model;

namespace Workflow
{
    public class WorkflowInstance
    {
        private DataQuery _dataQuery;
        protected DataQuery DataQuery
        {
            get { return _dataQuery ?? (_dataQuery = new DataQuery()); }
        }

        private static readonly WorkflowInstance _instance = new WorkflowInstance();
        public static WorkflowInstance Instance
        {
            get { return _instance; }
        }

        public WorkflowInstance()
        {
            _dataQuery = new DataQuery();
        }

        public string Get_WFCode_ByCoQuanID(int coQuanID)
        {
            string result = "";
            result = this.DataQuery.GetWFTienHanh_By_CoQuanID(coQuanID);
            return result;
        }

        public string Get_WFLapKeHoachTTCode_ByCoQuanID(int coQuanID)
        {
            string result = "";
            result = this.DataQuery.GetWFLapKeHoach_By_CoQuanID(coQuanID);
            return result;
        }
        public string Get_WFLapKeHoachTTCode_ByDocumentID(int doccumentID)
        {
            string result = "";
            result = this.DataQuery.GetWFLapKeHoach_By_DocumentID(doccumentID);
            return result;
        }

        public int AttachDocument(int documentID, string workflowCode, int userID, Nullable<DateTime> dueDate, int coQuanID)
        {
            int insertID = 0;

            DocumentModel docModel = new DocumentModel();
            docModel.DocumentID = documentID;
            docModel.WorkflowID = this.DataQuery.GetWorkflowIDByCode(workflowCode);
            docModel.StateID = this.DataQuery.GetFirstState(workflowCode);
            docModel.CoQuanID = coQuanID;
            docModel.DueDate = dueDate;

            insertID = this.DataQuery.InsertDocument(docModel);

            return insertID;
        }

        public int AttachDocument(int documentID, string workflowCode, string stateCode, int userID, Nullable<DateTime> dueDate, int coQuanID)
        {
            int insertID = 0;

            DocumentModel docModel = new DocumentModel();
            docModel.DocumentID = documentID;
            docModel.WorkflowID = this.DataQuery.GetWorkflowIDByCode(workflowCode);
            docModel.StateID = this.DataQuery.GetStateByWFAndStateCode(workflowCode, stateCode);
            docModel.CoQuanID = coQuanID;
            docModel.DueDate = dueDate;

            insertID = this.DataQuery.InsertDocument(docModel);

            return insertID;
        }

        public string GetPrevStateOfDocument(int documentID)
        {
            string result = "";
            result = this.DataQuery.GetPrevStateOfDocument(documentID);
            return result;
        }

        public string GetCurrentStateOfDocument(int documentID)
        {
            if (documentID == 0) return string.Empty;

            string result = "";
            result = this.DataQuery.GetCurrentStateOfDocument(documentID);
            return result;
        }

        public List<int> GetDocumentByPrevState(string currentStateName, string nextStateName, DateTime startDate, DateTime endDate)
        {
            return this.DataQuery.GetDocumentByPrevState(currentStateName, nextStateName, startDate, endDate);
        }

        public List<DocumentModel> GetDocumentByState(string stateName, DateTime startDate, DateTime endDate)
        {
            return this.DataQuery.GetDocumentByState(stateName, startDate, endDate);
        }

        public List<string> GetAvailabelCommands(int documentID)
        {
            List<string> commandList = new List<string>();

            DocumentModel docModel = this.DataQuery.GetDocumentByID(documentID);

            var commands = this.DataQuery.GetCommandByState(docModel.StateID);

            foreach (var command in commands)
            {
                commandList.Add(command.CommandCode);
            }

            return commandList;
        }

        public StateModel GetCurentStatesByDocument(int documentID)
        {
            StateModel stateModel = this.DataQuery.GetCurentStatesByDocument(documentID);

            return stateModel;
        }

        public List<CommandModel> GetAvailabelFullCommands(int documentID)
        {
            List<CommandModel> lsCommandInfo = new List<CommandModel>();

            DocumentModel docModel = this.DataQuery.GetDocumentByID(documentID);

            var commands = this.DataQuery.GetCommandByState(docModel.StateID);


            lsCommandInfo = commands;

            return lsCommandInfo;
        }

        public ResponeseModel ExecuteCommand(int documentID, int userID, string commandCode, DateTime dueDate, string comment, int coQuanID)
        {
            ResponeseModel responInfo = new ResponeseModel();
            responInfo.TransactionHistoryID = 0;

            DocumentModel docModel = this.DataQuery.GetDocumentByID(documentID);

            if (docModel == null)
            {
                responInfo.response = false;
                return responInfo;
            }


            int currentStateID = docModel.StateID;

            docModel.DocumentID = documentID;
            docModel.StateID = this.DataQuery.GetNextState(currentStateID, commandCode);
            docModel.DueDate = dueDate;
            docModel.CoQuanID = coQuanID;


            this.DataQuery.UpdateDocumentState(docModel);

            TransitionHistoryModel transHistoryModel = new TransitionHistoryModel();

            TransitionModel transModel = this.DataQuery.GetTransitionByStateAndCommand(currentStateID, commandCode);

            if (transModel == null)
            {

                responInfo.response = false;
                return responInfo;
            }

            transHistoryModel.DueDate = dueDate;
            transHistoryModel.Comment = comment;
            transHistoryModel.DocumentID = documentID;
            transHistoryModel.TransitionID = transModel.TransitionID;
            transHistoryModel.UserID = userID;
            transHistoryModel.ModifiedDate = DateTime.Now;
            transHistoryModel.CoQuanID = coQuanID;


            int transactionHistoryID = this.DataQuery.InsertTransitionHistory(transHistoryModel);


            responInfo.TransactionHistoryID = transactionHistoryID;
            responInfo.response = true;
            return responInfo;
        }

        public List<WorkFlowModel> GetAllWorkflow()
        {
            List<WorkFlowModel> lsWorkFlowInfo = new List<WorkFlowModel>();
            lsWorkFlowInfo = this.DataQuery.GetAllWorkflow();
            return lsWorkFlowInfo;
        }
    }
}
