﻿namespace OnBaseRocketUnityScriptTrigger
{
    using System.Data;
    using System.Collections.Generic;
    using Hyland.Unity.UnityForm;
    using System;
    using Hyland.Unity;
    using Hyland.Rocket.RPA.ApiHelper;

    /// <summary>
    /// OnBaseUnityScriptTrigger
    /// </summary>
    public class OnBaseRocketUnityScriptTrigger : Hyland.Unity.IClientWorkflowScript
    {
        private string defaultProcessIdPropertyBagName = "pProcessorID";
        private string defaultProcessId = "1";

        private string defaultHeartApiUrl = "";
        private string defaultIdentityApiUrl = "";

        private string defaultAccessToken = "";
        
        private bool isTaskRedoable = true;
        private RpaTaskType type = RpaTaskType.PRO;
        private string diversity = "";

        INPUTDATA_MODE INPUTMODE = INPUTDATA_MODE.DOCUMENT_ID; //INPUTDATA_MODE.WORKVIEW_ID, INPUTDATA_MODE.UNITYFORM_FIELDS

        /******************************************************************/
        /******************************************************************/
        /****** STOP EDITING HERE UNLESS YOU KNOW WHAT YOU ARE DOING ******/
        /******************************************************************/
        /******************************************************************/
        #region IClientWorkflowScript
        private Dictionary<FieldDataType, Type> mapping =>
            new Dictionary<FieldDataType, Type>()
            {
                {FieldDataType.Numeric9, typeof(int)},
                {FieldDataType.Numeric20, typeof(int)},
                {FieldDataType.FloatingPoint, typeof(Double)},
                {FieldDataType.Date, typeof(DateTime)},
                {FieldDataType.AlphaNumeric, typeof(string)},
                {FieldDataType.Currency, typeof(string)},
                {FieldDataType.Decimal, typeof(Double)},
                {FieldDataType.Boolean, typeof(bool)},
                {FieldDataType.DateTime, typeof(DateTime)}
            };

        public Application _app { get; set; }

        /// <summary>
        /// Implementation of <see cref="IClientWorkflowScript.OnClientWorkflowScriptExecute" />.
        /// <seealso cref="IClientWorkflowScript" />
        /// </summary>
        /// <param name="app"></param>
        /// <param name="args"></param>
        public void OnClientWorkflowScriptExecute(Hyland.Unity.Application app, Hyland.Unity.ClientWorkflowEventArgs args)
        {
            try
            {
                var doc = args.Document;
				var wv = args.WorkViewObject;
                _app = app;

                // Get Configuration
                string heartApiUrl;
                GetConfigValue("Hyland.RPA.HeartApiUrl", defaultHeartApiUrl, out heartApiUrl);

                string identityApiUrl;
                GetConfigValue("Hyland.RPA.IdentityApiUrl", defaultIdentityApiUrl, out identityApiUrl);

                string apiAccessToken;
                GetConfigValue("Hyland.RPA.AccessToken", defaultAccessToken, out apiAccessToken);

                // Get ProcessorID
                // Depending on your Property it can be a string or int
                string processId;
                try
                {
                    var found = args.PropertyBag.TryGetValue(defaultProcessIdPropertyBagName, out processId);
                    processId = found ? processId : defaultProcessId;
                }
                catch (Exception e)
                {
                    app.Diagnostics.Write("Could not get ProcessID");
                    app.Diagnostics.Write(e);
                    throw new Exception("Could not get ProcessID", e);
                }

                // Connect to the API
                RpaApi rpaApi = null;
                try
                {
                    rpaApi = new RpaApi(heartApiUrl, identityApiUrl, apiAccessToken);
                }
                catch (Exception e)
                {
                    app.Diagnostics.Write("Could not connect to the API");
                    app.Diagnostics.Write(e);
                    throw new Exception("Could not connect to the API", e);
                }

                // Create InputData DataTable
                DataTable dt;

                switch (INPUTMODE)
                {
                    case INPUTDATA_MODE.UNITYFORM_FIELDS:
                        dt = GetUnityFormFieldsAsDataTable(app, doc);
                        break;
                    default:
                    case INPUTDATA_MODE.DOCUMENT_ID:
                        dt = GetDocumentIdAsDataTable(app, doc);
                        break;
					case INPUTDATA_MODE.WORKVIEW_ID:
						dt = GetWorkViewIdAsDataTable(app, wv);
                        break;
                }


                var inputData = RpaHelper.SerializeDataTable(dt);

                var task = rpaApi.Tasks.Create(Convert.ToInt32(processId), inputData, type, diversity, isTaskRedoable);
                app.Diagnostics.Write("Created a new RPA Task with ID: " + task);


            }
            catch (Exception e)
            {
                app.Diagnostics.Write(e);
                throw e;
            }
        }

        private DataTable GetDocumentIdAsDataTable(Application app, Document doc)
        {
            var dt = new DataTable("InputData");
            dt.Columns.Add("DocumentId", typeof(int));
            dt.Rows.Add(Convert.ToInt32(doc.ID));
            return dt;
        }
		
		
        private DataTable GetWorkViewIdAsDataTable(Application app, Hyland.Unity.WorkView.Object doc)
        {
            var dt = new DataTable("InputData");
            dt.Columns.Add("WorkViewId", typeof(int));
            dt.Rows.Add(Convert.ToInt32(doc.ID));
            return dt;
        }

        private DataTable GetUnityFormFieldsAsDataTable(Application app, Document doc)
        {
            var dt = new DataTable("InputData");
            // Get all UnityFields from Document
            var unityFields = doc.UnityForm?.AllFields?.ValueFields;
            foreach (var x in unityFields)
            {

                Type currentType = mapping[x.FieldDefinition.DataType];
                dt.Columns.Add(new DataColumn(x.FieldDefinition.Name, currentType));

            }

            // Add all values to the dt DataTable
            var d = dt.NewRow();
            foreach (var field in unityFields)
            {
                try
                {
                    app.Diagnostics.Write("Start Field with name: " + field.FieldDefinition.Name);
                    Type currentType = mapping[field.FieldDefinition.DataType];
                    app.Diagnostics.Write("Field DataType: " + field.FieldDefinition.DataType + " / " + currentType.ToString());
                    switch (currentType.ToString())
                    {
                        case "System.Int32":
                            if (!field.IsEmpty)
                                d.SetField<int>(field.FieldDefinition.Name, System.Convert.ToInt32(field.Value.ToString()));
                            break;
                        case "System.Double":
                            if (!field.IsEmpty)
                                d.SetField<double>(field.FieldDefinition.Name, field.FloatingPointValue);
                            break;
                        case "System.String":
                            if (!field.IsEmpty)
                                d.SetField<string>(field.FieldDefinition.Name, field.Value.ToString());
                            break;
                        case "System.DateTime":
                            if (!field.IsEmpty)
                                d.SetField<DateTime>(field.FieldDefinition.Name, field.DateTimeValue);
                            break;
                        case "System.Boolean":
                            if (!field.IsEmpty)
                                d.SetField<bool>(field.FieldDefinition.Name, field.BooleanValue);
                            break;
                    }
                }
                catch (Exception e)
                {
                    app.Diagnostics.Write("Could not convert value for " + field.FieldDefinition.Name);
                    app.Diagnostics.Write("Value: " + field.Value.ToString());
                    app.Diagnostics.Write(e);
                    throw new Exception("Could not convert value for " + field?.FieldDefinition?.Name, e);
                }
            }

            dt.Columns.Add(new DataColumn("DocumentId", typeof(int)));
            d.SetField<int>("DocumentId", System.Convert.ToInt32(doc?.ID));

            dt.Rows.Add(d);
            return dt;
        }

        protected void GetConfigValue(string key, string defaultValue, out string result)
        {
            string _r;
            result = _app.Configuration.TryGetValue(key, out _r) ? _r : defaultValue;
        }

        public enum INPUTDATA_MODE
        {
            DOCUMENT_ID,
            UNITYFORM_FIELDS,
			WORKVIEW_ID
        }
        #endregion
    }
}