/// <reference path="AmmeterRealtimeData.js" />

$(function () {
    LoadMeterData("first");
    LoadData();
});

function setTimer() {
    g_timer = setTimeout(" LoadData()", 60000);
}
function LoadData() {
    LoadRealTimeData();
    setTimer();
}
function LoadMeterData(type, mData) {
    if (type == "first") {
        $('#realTime_Ammeter').datagrid({
            frozenColumns: [[{
                field: 'FloorName', title: '楼层', width: 100, align: 'center'
                      , styler: function (value, row, index) {
                          return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;';
                      }
            }]],
            columns:
                        [[{ "title": "电表", "colspan": 9, }, ],
                        [{
                            field: 'GaugeNumber', title: '电表号', width: 100, align: 'left', styler: function (value, row, index) {
                                if (row["GaugeNumber"] == "总计") {
                                    return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;';
                                }
                            } },
                        {
                            field: 'Gname', title: '电表名', width: 120, align: 'left', styler: function (value, row, index) {
                                if (row["GaugeNumber"] == "总计") {
                                    return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;';
                                }                              
                            }
                        },
                        {
                            field: 'Greal', title: '实时电能', width: 100, align: 'right', styler: function (value, row, index) {
                                if (row["GaugeNumber"] == "总计") {
                                    return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;';
                                }
                            }
                        },
                        {
                            field: 'Gpower', title: '实时功率', width: 100, align: 'right', styler: function (value, row, index) {
                                if (row["GaugeNumber"] == "总计") {
                                    return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;';
                                }
                            }
                        },
                        {
                            field: 'GdaySum', title: '日累计', width: 100, align: 'right', styler: function (value, row, index) {
                                if (row["GaugeNumber"] == "总计") {
                                    return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;';
                                }
                            }
                        },
                        {
                            field: 'GmonthSum', title: '月累计', width: 100, align: 'right', styler: function (value, row, index) {
                                if (row["GaugeNumber"] == "总计") {
                                    return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;';
                                }
                            }
                        },
                        {
                            field: 'GyearSum', title: '年累计', width: 100, align: 'right', styler: function (value, row, index) {
                                if (row["GaugeNumber"] == "总计") {
                                    return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;';
                                }
                            }
                        },
                        {
                            field: 'mIP', title: 'IP地址', width: 120, align: 'center', styler: function (value, row, index) {
                                if (row["GaugeNumber"] == "总计") {
                                    return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;';
                                }
                            }
                        },
                        {
                            field: 'mAddress', title: '表地址', width: 100, align: 'center', styler: function (value, row, index) {
                                if (row["GaugeNumber"] == "总计") {
                                    return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;';
                                }
                            }
                        }]],
            striped: true,
            singleSelect: true,
            fit: true
        });
    } else if(type=="last") {
        $('#realTime_Ammeter').datagrid('loadData',mData);
    }      
}
function LoadRealTimeData() {
    var mger = "";
    $.ajax({
        type: "POST",
        url: "AmmeterRealtimeData.aspx/GetAmmeterRealtimeData",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            mger.window('close');
            if (msg.d == "[]") {
                $.messager.alert('提示', "没有查询的数据");
                return;
            } else {
                var myData = jQuery.parseJSON(msg.d);
                //g_num = myData.total;
                LoadMeterData("last", myData);
                myMergeCell('realTime_Ammeter', "FloorName");
            }          
        },
        beforeSend: function (XMLHttpRequest) {
            //alert('远程调用开始...');
            mger = $.messager.alert('提示', "加载中...");
        },
        error: handleError
    });
}
function handleError() {
    $('#realTime_Ammeter').datagrid('loadData', []);
    $.messager.alert('失败', '获取数据失败');
}
