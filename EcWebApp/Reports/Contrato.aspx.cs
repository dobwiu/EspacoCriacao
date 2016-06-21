using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EcWebApp.Reports
{
    public partial class Contrato : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ShowReport();  
        }

        private void ShowReport()
        {
            try
            {
                //report url  
                string urlReportServer = "http://Servername/Reportserver";

                // ProcessingMode will be Either Remote or Local  
                rptViewer.ProcessingMode = ProcessingMode.Remote;

                //Set the ReportServer Url  
                rptViewer.ServerReport.ReportServerUrl = new Uri(urlReportServer);

                // setting report path  
                //Passing the Report Path with report name no need to add report extension   
                rptViewer.ServerReport.ReportPath = "/Jignesh/TestReport";

                //Set report Parameter  
                //Creating an ArrayList for combine the Parameters which will be passed into SSRS Report  
                //ArrayList reportParam = new ArrayList();  
                //reportParam = ReportDefaultPatam();  

                //ReportParameter[] param = new ReportParameter[reportParam.Count];  
                //for (int k = 0; k < reportParam.Count; k++)  
                //{  
                //    param[k] = (ReportParameter)reportParam[k];  
                //}  

                // pass credential as if any... no need to pass anything if we use windows authentication  
                //rptViewer.ServerReport.ReportServerCredentials =   
                //  new ReportServerCredentials("UserName", "Password", "domainname");  

                //pass parameters to report  
                //rptViewer.ServerReport.SetParameters(param);   
                rptViewer.ServerReport.Refresh();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}