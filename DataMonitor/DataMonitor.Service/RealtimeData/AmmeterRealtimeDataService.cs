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
    public class AmmeterRealtimeDataService
    {
        public static DataTable GetAmmeterDataTable()
        {
            string connectionString = ConnectionStringFactory.JCJTConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = "";
            string Asql = @"select Field_name from [dbo].[GaugeContrast] where Gauge_number like 'A%'";
            DataTable table_G = dataFactory.Query(Asql);
            string colStr = "";
            StringBuilder sumStr = new StringBuilder();
            sumStr.Append("select ");
            string Anull = "";
            foreach (DataRow dr in table_G.Rows)
            {
                string _name = dr["Field_name"].ToString().Trim();
                colStr = colStr + _name + ",";
                Anull = Anull + "isnull(" + _name + ",-1)" + _name + ",";
                sumStr.Append("isnull(sum(");
                sumStr.Append(_name);
                sumStr.Append("),-1) AS ");
                sumStr.Append(_name);
                sumStr.Append(",");
            }
            sumStr.Remove(sumStr.Length - 1, 1);
            sumStr.Append(" FROM [dbo].[History_Increment_A_Energy]");
            colStr = colStr.Remove(colStr.Length - 1, 1);
            Anull = Anull.Remove(Anull.Length - 1, 1);

            mySql = @"select B.Floor_name as FloorName,B.Gauge_number as GaugeNumber,B.Gauge_description as Gname,B.Floor,A.s_Value as Greal,C.s_Value as Gpower,D.s_Value as GdaySum,E.s_Value as GmonthSum,F.s_Value as GyearSum,B.Com_ip as mIP,B.Gauge_address as mAddress
	                   from [dbo].[GaugeContrast] B,
			                (select [s_Name],[s_Value] from " + "(select " + Anull + " from Realtime_A_Energy)AEN unpivot ([s_Value] for [s_Name] in(" + colStr + @"))a) A,
			                (select [s_Name],[s_Value] from" + "(select " + Anull + " from Realtime_A_Power)APN unpivot ([s_Value] for [s_Name] in(" + colStr + @"))c) C,
                            (select [s_Name],[s_Value] from (";
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(mySql).Append(sumStr);
            sqlBuilder.Append(@" where datediff(Day,[vDate],getdate())=0)Dsum  unpivot ([s_Value] for [s_Name] in(" + colStr + @"))d) D,(select [s_Name],[s_Value] from (");
            sqlBuilder.Append(sumStr);
            sqlBuilder.Append(@" where datediff(Month,[vDate],getdate())=0)Msum  unpivot ([s_Value] for [s_Name] in(" + colStr + @"))e) E,(select [s_Name],[s_Value] from (");
            sqlBuilder.Append(sumStr);
            sqlBuilder.Append(@" where datediff(Year,[vDate],getdate())=0)Ysum  unpivot ([s_Value] for [s_Name] in(" + colStr + @"))f)F ");
            mySql = @"where B.Gauge_number like 'A%' 
                                            and B.Field_name=A.s_Name 
                                            and B.Field_name=C.s_Name 
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
           // double mreal = 0;
            double mdaySum = 0;
            double mmonthSum = 0;
            double myearSum = 0;

            //每层汇总
            string floorName = table.Rows[0]["FloorName"].ToString();
           // double real = Convert.ToDouble(table.Rows[0]["Greal"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[0]["Greal"]);
            double daySum = Convert.ToDouble(table.Rows[0]["GdaySum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[0]["GdaySum"]);
            double monthSum = Convert.ToDouble(table.Rows[0]["GmonthSum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[0]["GmonthSum"]);
            double yearSum = Convert.ToDouble(table.Rows[0]["GyearSum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[0]["GyearSum"]);
            for (int i = 1; i < rowsCount; i++)
            {
                if (table.Rows[i]["Floor"].ToString().Equals(table.Rows[i - 1]["Floor"].ToString()))
                {

                   // real = real + (Convert.ToDouble(table.Rows[i]["Greal"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["Greal"]));
                    daySum = daySum + (Convert.ToDouble(table.Rows[i]["GdaySum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["GdaySum"]));
                    monthSum = monthSum + (Convert.ToDouble(table.Rows[i]["GmonthSum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["GmonthSum"]));
                    yearSum = yearSum + (Convert.ToDouble(table.Rows[i]["GyearSum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["GyearSum"]));

                    if (rowsCount - 1 == i)
                    {
                        table.Rows.Add(table.Rows[i]["FloorName"], "总计",null, table.Rows[i]["Floor"], null, null, daySum, monthSum, yearSum);
                       // mreal = mreal + real;
                        mdaySum = mdaySum + daySum;
                        mmonthSum = mmonthSum + monthSum;
                        myearSum = myearSum + yearSum;
                        table.Rows.Add("所有楼层", "总计", null, 30, null, null, mdaySum, mmonthSum, myearSum);
                    }
                }
                else
                {
                    table.Rows.Add(table.Rows[i - 1]["FloorName"], "总计", null, table.Rows[i - 1]["Floor"], null, null, daySum, monthSum, yearSum);

                   // mreal = mreal + real;
                    mdaySum = mdaySum + daySum;
                    mmonthSum = mmonthSum + monthSum;
                    myearSum = myearSum + yearSum;


                 //   real = Convert.ToDouble(table.Rows[i]["Greal"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["Greal"]);
                    daySum = Convert.ToDouble(table.Rows[i]["GdaySum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["GdaySum"]);
                    monthSum = Convert.ToDouble(table.Rows[i]["GmonthSum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["GmonthSum"]);
                    yearSum = Convert.ToDouble(table.Rows[i]["GyearSum"]) <= 0 ? 0 : Convert.ToDouble(table.Rows[i]["GyearSum"]);
                }
            }
            table.DefaultView.Sort = "Floor asc";
            table = table.DefaultView.ToTable();

            return table;
        
        }

    }
}
