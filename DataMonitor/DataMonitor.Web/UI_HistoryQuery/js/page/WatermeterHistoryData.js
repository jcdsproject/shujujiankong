/// <reference path="WatermeterHistoryData.js" />

$(function () {
    InitDate();
    LoadWaterMeterData("first");
});
function InitDate() {
    var nowDate = new Date();
    var beforeDate = new Date();
    beforeDate.setDate(nowDate.getMonth() - 1);
    var nowString = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate() + " " + nowDate.getHours() + ":" + nowDate.getMinutes() + ":" + nowDate.getSeconds();
    var beforeString = beforeDate.getFullYear() + '-' + (beforeDate.getMonth() + 1) + '-01 00:00:00';
    $('#startDate').datetimebox('setValue', beforeString);
    $('#endDate').datetimebox('setValue', nowString);
    //$('#startDate').datetimebox('setValue', '2016-01-25 16:07:48.000');
    //$('#endDate').datetimebox('setValue', '2016-01-25 17:17:54.000');
}
function LoadWaterMeterData(type, myData) {
    if (type == "first") {
        $('#grid_WaterMeterDatagrid').datagrid({
            columns:
                     [[{ "title": "名称", "colspan": 4, "algign": "center" },
                     { field: 'StartValue', title: '期初值', width: 100, rowspan: 2, align: 'right' },
                     { field: 'EndValue', title: '期末值', width: 100, rowspan: 2, align: 'right' },
                     { field: 'Consume', title: '耗水量', width: 100, rowspan: 2, align: 'right' }
                     //,{ field: 'Wsum', title: '小计', width: 100, rowspan: 2, align: 'right' }
                     ],
                     [{
                         field: 'FloorName', title: '楼层', width: 100, align: 'center'
                         , styler: function (value, row, index) {
                             //if (index < g_num - 1 && index != 0) {
                             //return 'background-color:#CCCCCC;border-top:1px solid #1F1F1F;';
                             return 'background-color:#B0E0E6;border-top:1px solid #1F1F1F;';
                             //}
                             //else {
                             //return 'background-color:#CCCCCC;';
                             //}
                         }
                     }, { field: 'GaugeNumber', title: '水表号', width: 100, align: 'left' },
                      { field: 'MeterName', title: '水表名', width: 100, align: 'left' },
                       { field: 'mAddress', title: '表地址', width: 100, align: 'center' }]],
            striped: true,
            singleSelect: true,
            fit: true
        });
    }
    else if (type == "last") {
        $('#grid_WaterMeterDatagrid').datagrid('loadData', myData);
    }
}
function queryHistoryData() {
    var mger = "";
    var m_starTime = $('#startDate').datetimebox('getValue');
    var m_endTime = $('#endDate').datetimebox('getValue');
    $.ajax({
        type: "POST",
        url: "WatermeterHistoryData.aspx/GetWaterMeterHistoryData",
        data: "{startTime: '" + m_starTime + "',endTime:'" + m_endTime + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            mger.window('close');
            var myData = jQuery.parseJSON(msg.d);
            if (myData == [] || myData.total == 0) {
                $.messager.alert('提示', "没有查询的数据");
                return;
            }
            else {
                LoadWaterMeterData("last", myData);
                myMergeCell('grid_WaterMeterDatagrid', "FloorName");
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
    $('#grid_WaterMeterDatagrid').datagrid('loadData', []);
    $.messager.alert('失败', '获取数据失败');
}