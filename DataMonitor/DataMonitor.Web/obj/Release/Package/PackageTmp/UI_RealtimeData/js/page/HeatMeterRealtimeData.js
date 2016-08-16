
$(function () {
    initMeterDatagridFram("first");
    LoadData();
});

function setTimer() {
    g_timer = setTimeout(" LoadData()", 60000);
}
function LoadData() {
    LoadRealTimeData();
    setTimer();
}
function initMeterDatagridFram(type, mData) {
    if (type == "first") {
        $('#realTime_HeatMeter').datagrid({
            frozenColumns: [[{
                field: 'FloorName', title: '楼层', width: 80, align: 'center', styler: function (value, row, index) {
                    return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;';
                }
            },
                {
                    field: 'GaugeNumber', title: '热表号', width: 100, align: 'left', styler: function (value, row, index) {
                        if (row["GaugeNumber"] == "总计") {
                            return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;';
                        }
                    }
                },
                {
                    field: 'Hname', title: '热量表名称', width: 120, align: 'left', styler: function (value, row, index) {
                        if (row["GaugeNumber"] == "总计") {
                            return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;';
                        }
                    }
                }
                           ]],
            columns:
                [[
                { "title": "热量表", "colspan": 10 }],
                [
                {
                    field: 'Hreal', title: '实时热量', width: 100, align: 'right', styler: function (value, row, index) {
                        if (row["GaugeNumber"] == "总计") {
                            return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;';
                        }
                    }
                },
                {
                    field: 'HdaySum', title: '日累计', width: 100, align: 'right', styler: function (value, row, index) {
                        if (row["GaugeNumber"] == "总计") {
                            return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;';
                        }
                    }
                },
                {
                    field: 'HmonthSum', title: '月累计', width: 100, align: 'right', styler: function (value, row, index) {
                        if (row["GaugeNumber"] == "总计") {
                            return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;';
                        }
                    }
                },
                {
                    field: 'HyearSum', title: '年累计', width: 100, align: 'right', styler: function (value, row, index) {
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
                },
                {
                    field: 'Hpower', title: '当前热功率', width: 100, align: 'right', styler: function (value, row, index) {
                        if (row["GaugeNumber"] == "总计") {
                            return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;';
                        }
                    }
                },
                {
                    field: 'Hflow', title: '瞬时流量', width: 100, align: 'right', styler: function (value, row, index) {
                        if (row["GaugeNumber"] == "总计") {
                            return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;';
                        }
                    }
                },
                {
                    field: 'Hsupply', title: '供水温度', width: 100, align: 'right', styler: function (value, row, index) {
                        if (row["GaugeNumber"] == "总计") {
                            return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;';
                        }
                    }
                },
                {
                    field: 'Hback', title: '回水温度', width: 100, align: 'right', styler: function (value, row, index) {
                        if (row["GaugeNumber"] == "总计") {
                            return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;';
                        }
                    }
                }]
                ],
            striped: true,
            singleSelect: true,
            fit: true
        });
    } else if (type == "last") {
        $('#realTime_HeatMeter').datagrid('loadData', mData);
    }
}
function LoadRealTimeData() {
    var mger = "";
    $.ajax({
        type: "POST",
        url: "HeatMeterRealtimeData.aspx/GetHeatMeterRealtimeData",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            mger.window('close');
            if (msg.d == "[]") {
                initMeterDatagridFram("last", []);
                $.messager.alert('提示', "没有查询的数据");
                return;
            } else {
                var myData = jQuery.parseJSON(msg.d);
                //g_num = myData.total;
                initMeterDatagridFram("last", myData);
                myMergeCell('realTime_HeatMeter', "FloorName");
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
