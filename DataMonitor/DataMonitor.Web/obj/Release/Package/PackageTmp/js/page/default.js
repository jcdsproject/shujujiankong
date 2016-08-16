//var PageExistNodeIdArray = new Array();
//$(function () {
//    try {
//        mainTabs = $('#mainTabs').tabs({
//            fit: true,
//            border: false,
//            tools: [],
//            onClose: function (title, index) {
//                //alert(index);
//                var m_DeleteIndex = index - 1;
//                if (m_DeleteIndex >= 0) {
//                    PageExistNodeIdArray.splice(m_DeleteIndex, 1);
//                }
//                else {
//                    PageExistNodeIdArray.splice(0, 1);
//                }
//            }
//        });
//    }
//    catch (e) {

//    }

//};
var MainFrameItemHeight;
var DefaultFrameItemHeight;
$(function () {
    $('#mainLayout').layout('panel', 'center').panel({
        onResize: function (width, height) {
            //$("div[name='MainFrameItems']").css("height", height - 20);
            MainFrameItemHeight = height - 15 - 30 - 16;
            $('.MainFrameItems').css("height", MainFrameItemHeight);

            DefaultFrameItemHeight = height - 4 - 31;
            $('.DefalutMainFrameItems').css("height", DefaultFrameItemHeight);

        }
    });


})

function RealtimeFun()
{
    $('#mainTabs').tabs('select',0);
}




function HistoryQueryFun()
{
    $('#mainTabs').tabs('add',{
        title: '历史查询',
        closable: true,      
        content:'<iframe id="DefalutMainFrame" class="DefalutMainFrameItems" src="/UI_DataQuery/HistoryQuery.aspx" style="width: 100%; height: 100%;border:0"></iframe>',
        border: false,
        fit: true
    });
}
function LogoutFun()
{
    self.location = "Login.aspx";
}