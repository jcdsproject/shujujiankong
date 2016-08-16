<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AmmeterHistoryData.aspx.cs" Inherits="DataMonitor.Web.UI_HistoryQuery.AmmeterHistoryData" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
     <title>电表历史查询</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/pllib/jquery.jqplot.min.js"></script>

    <script type="text/javascript" src="/js/common/format/DateTimeFormat.js" charset="utf-8"></script>
        <script type="text/javascript" src="js/common/MergeCell.js" charset="utf-8"></script>
      <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->
    <script type="text/javascript" src="js/page/AmmeterHistoryData.js" charset="utf-8"></script>
</head>
<body>
        <div class="easyui-layout" data-options="fit:'true'">
                <div data-options="region:'north',border:false" style="height:48px">
                    <table>
                        <tr>
                            <td>
                                <table style="padding-top:10px">
                                    <tr>                                  
                                    <td style="width:80px;text-align:center">开始时间：</td>
                                    <td>
                                        <input id="startDate" class="easyui-datetimebox" required="required" style="width:150px"/>
                                    </td>
                                    <td style="width:80px;text-align:center">结束时间：</td>
                                    <td>
                                        <input id="endDate" class="easyui-datetimebox" required="required"  style="width:150px"/>
                                    </td>
                                    <td style="text-align:center">
                                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true" onclick="queryHistoryData()">查询</a>
                                    </td>
                                    </tr>
                                </table>                      
                           </td>
                          </tr>
                    </table>                            
                </div>
                <div id="reportTable" class="easyui-panel" data-options="region:'center', border:true, collapsible:true, split:false">
                    <table id="grid_AmmeterDatagrid" class="easyui-datagrid"></table>
                </div>   
        </div>
</body>
</html>
