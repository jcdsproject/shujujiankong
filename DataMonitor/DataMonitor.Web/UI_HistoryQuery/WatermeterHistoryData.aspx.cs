using DataMonitor.Service.HistoryQuery;
using DataMonitor.Service.RealtimeData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DataMonitor.Web.UI_HistoryQuery
{
    public partial class WatermeterHistoryData : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        [WebMethod]
        public static string GetWaterMeterHistoryData(string startTime, string endTime) 
        {
            DataTable table = WatermeterHistoryDataService.GetWaterMeterHistoryDataTable(startTime, endTime);
            string json = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(table).Replace("-1.00", "Null");
            return json;             
        }
    }
}