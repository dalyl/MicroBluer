// Write your JavaScript code.
var BlueTouch = {}
BlueTouch.Partial = function (url, parent, after) {
    $.ajax({
        url: url, context:null, success: function (r) {
             $(parent).html(r);
             if (after && typeof (after) == "function") after(parent);
        }
    });
};

