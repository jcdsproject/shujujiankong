// JScript File
$(function () {
    changeFrameSize($(this).width()-2, $(this).height() -2 );
    //根据页面大小自适应标题
    $(window).resize(function () {
        var width = $(this).width();
        var height = $(this).height();

        changeFrameSize(width - 2, height - 2);
    });
});
function changeFrameSize(myDisplayWidth,myDisplayHeight)
{
    document.getElementById("mainTableTop").style.width = max(myDisplayWidth,1000) + "px";
    document.getElementById("LeftloginTable").style.width = max(myDisplayWidth - 5,1000) + "px";
    document.getElementById("mainTableTop").style.height = ( max(500,myDisplayHeight) - 408 ) / 2 + "px";
    document.getElementById("LeftloginTableLeft").style.width = (max(myDisplayWidth,1000) -753 ) / 2 + "px";
    //alert((max(myDisplayWidth,1000) -753 ) / 2 + "px");
    //document.getElementById("mainTalbeTop").style.width = (max(myDisplayWidth,1000) - 753) / 2 + "px";
    //document.getElementById("mainTalbeTop").style.height = (max(500,myDisplayHeight) - 410) / 2 + "px";  
    //document.getElementById("mainTable").style.zIndex = "0";
}

function max(a, b) {
    if (a > b) {
        return a;
    }
    else {
        return b;
    }
}
