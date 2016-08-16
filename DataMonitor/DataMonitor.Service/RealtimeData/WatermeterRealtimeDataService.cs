using DataMonitor.Infrastructure.Configuration;
using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMonitor.Service.RealtimeData
{
    public class WatermeterRealtimeDataService
    {
        public static DataTable GetWaterMeterRealtimeDataTable()
        {
            string connectionString = ConnectionStringFactory.JCJTConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = "";
            string Wsql = @"select Field_name from [dbo].[GaugeContrast] where Gauge_number like 'W%'";
            DataTable table_W = dataFactory.Query(Wsql);
            StringBuilder sumStr = new StringBuilder();
            string colStr = "";
            sumStr.Append("select ");
            string Wnull = "";
            foreach (DataRow dr in table_W.Rows)
            {
                string _name = dr["Field_name"].ToString().Trim();
                colStr = colStr + _name + ",";
                Wnull = Wnull + "isnull(" + _name + ",-1)" + _name + ",";
                sumStr.Append("isnull(sum(");
                sumStr.Append(_name);
                sumStr.Append("),-1) AS ");
                sumStr.Append(_name);
                sumStr.Append(",");
            }
            sumStr.Remove(sumStr.Length - 1, 1);
            sumStr.Append(" FROM [dbo].[History_Increment_W_WaterFlow]");
            colStr = colStr.Remove(colStr.Length - 1, 1);
            Wnull = Wnull.Remove(Wnull.Length - 1, 1);

            mySql = @"select B.Floor_name as FloorName,B.Gauge_number as GaugeNumber,B.Gauge_description as Wname,B.Floor,A.s_Value as Wreal,D.s_Value as WdaySum,E.s_Value as WmonthSum,F.s_Value as WyearSum,B.Com_ip as mIP,B.Gauge_address as mAddress
	                       from [dbo].[GaugeContrast] B,
			                    (select [s_Name],[s_Value] from " + "(select " + Wnull + " from Realtime_W_WaterFlow)WFN unpivot ([s_Value] for [s_Name] in(" + colStr + @"))a) A,
                                (select [s_Name],[s_Value] from (";
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(mySql).Append(sumStr);
            sqlBuilder.Append(@" where datediff(Day,[vDate],getdate())=0)Dsum  unpivot ([s_Value] for [s_Name] in(" + colStr + @"))d) D,(select [s_Name],[s_Value] from (");
            sqlBuilder.Append(sumStr);
            sqlBuilder.Append(@" where datediff(Month,[vDate],getdate())=0)Msum  unpivot ([s_Value] for [s_Name] in(" + colStr + @"))e) E,(select [s_Name],[s_Value] from (");
            sqlBuilder.Append(sumStr);
            sqlBuilder.Append(@" where datediff(Year,[vDate],getdate())=0)Ysum  unpivot ([s_Value] for [s_Name] in(" + colStr + @"))f)F ");
            mySql = @"where B.Gauge_number like 'W%' 
                                            and B.Field_name=A.s_Name 
                                            and B.Field_name=D.s_Name 
                                            and B.Field_name=E.s_Name
                                            and B.Field_name=F.s_Name
	                                         order by B.Floor";
            sqlBuilder.Append(mySql);
            mySql = sqlBuilder.ToString();
            DataTable result = dataFactory.Query(mySql);
            result = GetSumbyLayout(result);
            return result;
        }
        private static DataTable GetSumbyLayout(DataTable table)
        {
            int rowsCount = table.Rows.Count;
            //整楼汇总
            double mreal = 0;
            double mdaySum = 0;
            double mmonthSum = 0;
            double myearSum = 0;

            //每层汇总
            string floorName = table.Rows[0]["FloorName"].ToString();
            double real = Convert.ToDouble(table.Rows[0]["Wreal"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[0]["Wreal"]);
            double daySum = Convert.ToDouble(table.Rows[0]["WdaySum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[0]["WdaySum"]);
            double monthSum = Convert.ToDouble(table.Rows[0]["WmonthSum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[0]["WmonthSum"]);
            double yearSum = Convert.ToDouble(table.Rows[0]["WyearSum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[0]["WyearSum"]);
            for (int i = 1; i < rowsCount; i++)
            {
                if (table.Rows[i]["Floor"].ToString().Equals(table.Rows[i - 1]["Floor"].ToString()))
                {
                    // floorName = table.Rows[i]["FloorName"].ToString();


                    real = real + (Convert.ToDouble(table.Rows[i]["Wreal"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["Wreal"]));


                    daySum = daySum + (Convert.ToDouble(table.Rows[i]["WdaySum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["WdaySum"]));


                    monthSum = monthSum + (Convert.ToDouble(table.Rows[i]["WmonthSum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["WmonthSum"]));

                    yearSum = yearSum + (Convert.ToDouble(table.Rows[i]["WyearSum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["WyearSum"]));

                    if (rowsCount - 1 == i)
                    {
                        table.Rows.Add(table.Rows[i]["FloorName"], "总计", null, table.Rows[i]["Floor"], null, daySum, monthSum, yearSum);

                        mreal = mreal + real;
                        mdaySum = mdaySum + daySum;
                        mmonthSum = mmonthSum + monthSum;
                        myearSum = myearSum + yearSum;
                        table.Rows.Add("所有楼层", "总计", null, 30, null, mdaySum, mmonthSum, myearSum);
                    }
                }
                else
                {
                    table.Rows.Add(table.Rows[i - 1]["FloorName"], "总计",null, table.Rows[i - 1]["Floor"], null, daySum, monthSum, yearSum);

                    mreal = mreal + real;
                    mdaySum = mdaySum + daySum;
                    mmonthSum = mmonthSum + monthSum;
                    myearSum = myearSum + yearSum;

                    real = Convert.ToDouble(table.Rows[i]["Wreal"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["Wreal"]);
                    daySum = Convert.ToDouble(table.Rows[i]["WdaySum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["WdaySum"]);
                    monthSum = Convert.ToDouble(table.Rows[i]["WmonthSum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["WmonthSum"]);
                    yearSum = Convert.ToDouble(table.Rows[i]["WyearSum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["WyearSum"]);
                }
            }
            table.DefaultView.Sort = "Floor asc";
            table = table.DefaultView.ToTable();

            return table;

        }
    }
}
