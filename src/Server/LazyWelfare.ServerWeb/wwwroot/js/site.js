// Write your JavaScript code.
var BlueTouch = {}
BlueTouch.Partial = function (url, parent) {
    $.ajax({
        url: url, context:null, success: function (r) {
             $(parent).html(r);
        }
    });
};

