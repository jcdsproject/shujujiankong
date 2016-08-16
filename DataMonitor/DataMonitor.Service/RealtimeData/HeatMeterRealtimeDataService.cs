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
    public class HeatMeterRealtimeDataService
    {
        public static DataTable GetHeatMeterRealtimeDataTable() 
        {
            string connectionString = ConnectionStringFactory.JCJTConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = "";
            string Hsql = @"select Field_name from [dbo].[GaugeContrast] where Gauge_number like 'H%'";
            DataTable table_H = dataFactory.Query(Hsql);
            StringBuilder sumStr = new StringBuilder();
            string colStr = "";
            sumStr.Append("select ");
            string Hnull = "";
            foreach (DataRow dr in table_H.Rows)
            {
                string _name = dr["Field_name"].ToString().Trim();
                colStr = colStr + _name + ",";
                Hnull = Hnull + "isnull(" + _name + ",-1)" + _name + ",";
                sumStr.Append("isnull(sum(");
                sumStr.Append(_name);
                sumStr.Append("),-1) AS ");
                sumStr.Append(_name);
                sumStr.Append(",");
            }
            sumStr.Remove(sumStr.Length - 1, 1);
            sumStr.Append(" FROM [dbo].[History_Increment_H_Heat]");
            colStr = colStr.Remove(colStr.Length - 1, 1);
            Hnull = Hnull.Remove(Hnull.Length - 1, 1);

            mySql = @"select B.Floor_name as FloorName,B.Gauge_number as GaugeNumber,B.Gauge_description as Hname,B.Floor,A.s_Value as Hreal,D.s_Value as HdaySum,E.s_Value as HmonthSum,F.s_Value as HyearSum,B.Com_ip as mIP,B.Gauge_address as mAddress
                              ,P.s_Value as Hpower,I.s_Value as Hflow,S.s_Value as Hsupply,T.s_Value as Hback
	                       from [dbo].[GaugeContrast] B,
			                    (select [s_Name],[s_Value] from " + "(select " + Hnull + " from Realtime_H_CurrentHeat)HFN unpivot ([s_Value] for [s_Name] in(" + colStr + @"))a) A,
                                (select [s_Name],[s_Value] from (";
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(mySql).Append(sumStr);
            sqlBuilder.Append(@" where datediff(Day,[vDate],getdate())=0)Dsum  unpivot ([s_Value] for [s_Name] in(" + colStr + @"))d) D,(select [s_Name],[s_Value] from (");
            sqlBuilder.Append(sumStr);
            sqlBuilder.Append(@" where datediff(Month,[vDate],getdate())=0)Msum  unpivot ([s_Value] for [s_Name] in(" + colStr + @"))e) E,(select [s_Name],[s_Value] from (");
            sqlBuilder.Append(sumStr);
            sqlBuilder.Append(@" where datediff(Year,[vDate],getdate())=0)Ysum  unpivot ([s_Value] for [s_Name] in(" + colStr + @"))f)F ");
            sqlBuilder.Append(@",(select [s_Name],[s_Value] from " + "(select " + Hnull + " from [Realtime_H_PowerHeat])HFN unpivot ([s_Value] for [s_Name] in(" + colStr + @"))a) P ");
            sqlBuilder.Append(@",(select [s_Name],[s_Value] from " + "(select " + Hnull + " from [Realtime_H_InstantFlow])HFN unpivot ([s_Value] for [s_Name] in(" + colStr + @"))a) I ");
            sqlBuilder.Append(@",(select [s_Name],[s_Value] from " + "(select " + Hnull + " from [Realtime_H_SupplyTemp])HFN unpivot ([s_Value] for [s_Name] in(" + colStr + @"))a) S ");
            sqlBuilder.Append(@",(select [s_Name],[s_Value] from " + "(select " + Hnull + " from [Realtime_H_BackTemp])HFN unpivot ([s_Value] for [s_Name] in(" + colStr + @"))a) T ");
            mySql = @"where B.Gauge_number like 'H%' 
                        and B.Field_name=A.s_Name 
                        and B.Field_name=D.s_Name 
                        and B.Field_name=E.s_Name
                        and B.Field_name=F.s_Name
                        and B.Field_name=P.s_Name
                        and B.Field_name=I.s_Name
                        and B.Field_name=S.s_Name
                        and B.Field_name=T.s_Name
                        order by B.Floor ";
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
            double real = Convert.ToDouble(table.Rows[0]["Hreal"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[0]["Hreal"]);
            double daySum = Convert.ToDouble(table.Rows[0]["HdaySum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[0]["HdaySum"]);
            double monthSum = Convert.ToDouble(table.Rows[0]["HmonthSum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[0]["HmonthSum"]);
            double yearSum = Convert.ToDouble(table.Rows[0]["HyearSum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[0]["HyearSum"]);
            for (int i = 1; i < rowsCount; i++)
            {
                if (table.Rows[i]["Floor"].ToString().Equals(table.Rows[i - 1]["Floor"].ToString()))
                {
                    // floorName = table.Rows[i]["FloorName"].ToString();


                    real = real + (Convert.ToDouble(table.Rows[i]["Hreal"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["Hreal"]));


                    daySum = daySum + (Convert.ToDouble(table.Rows[i]["HdaySum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["HdaySum"]));


                    monthSum = monthSum + (Convert.ToDouble(table.Rows[i]["HmonthSum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["HmonthSum"]));

                    yearSum = yearSum + (Convert.ToDouble(table.Rows[i]["HyearSum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["HyearSum"]));

                    if (rowsCount - 1 == i)
                    {
                        table.Rows.Add(table.Rows[i]["FloorName"], "总计", null, table.Rows[i]["Floor"], null, daySum, monthSum, yearSum);

                        mreal = mreal + real;
                        mdaySum = mdaySum + daySum;
                        mmonthSum = mmonthSum + monthSum;
                        myearSum = myearSum + yearSum;
                        table.Rows.Add("所有楼层", "总计",null, 30, null, mdaySum, mmonthSum, myearSum);
                    }
                }
                else
                {
                    table.Rows.Add(table.Rows[i - 1]["FloorName"], "总计", null, table.Rows[i - 1]["Floor"], null, daySum, monthSum, yearSum);

                    mreal = mreal + real;
                    mdaySum = mdaySum + daySum;
                    mmonthSum = mmonthSum + monthSum;
                    myearSum = myearSum + yearSum;


                    real = Convert.ToDouble(table.Rows[i]["Hreal"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["Hreal"]);
                    daySum = Convert.ToDouble(table.Rows[i]["HdaySum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["HdaySum"]);
                    monthSum = Convert.ToDouble(table.Rows[i]["HmonthSum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["HmonthSum"]);
                    yearSum = Convert.ToDouble(table.Rows[i]["HyearSum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["HyearSum"]);
                }
            }
            table.DefaultView.Sort = "Floor asc"; 
            table = table.DefaultView.ToTable();

            return table;

        }
    }
}
