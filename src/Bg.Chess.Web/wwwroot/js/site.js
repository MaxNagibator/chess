// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function SendRequest(options) {
    let _this = {};
    let defaultOptions = {
        method: 'POST',
    }

    _this.options = Object.assign({}, defaultOptions, options);

    _this.Send = function () {
        let bodyString = '';
        let first = true;

        for (var prop in _this.options.body) {
            if (first == false) {
                bodyString += "&";
            }
            bodyString += prop + "=" + _this.options.body[prop];
            first = false;
        }

        let xhr = new XMLHttpRequest();
        xhr.open(_this.options.method, _this.options.url, true);
        xhr.setRequestHeader('Content-Type', "application/x-www-form-urlencoded");
        xhr.onreadystatechange = function () {
            if (this.readyState != 4) {
                return;
            }
            if (this.status == 200) {
                if (_this.options.success) {
                    _this.options.success(this);
                }
            } else {
                if (_this.options.error) {
                    _this.options.error(this);
                }
            }
            if (_this.options.always) {
                _this.options.always(this);
            }
        };
        xhr.send(bodyString);
    }
    _this.Send();
}