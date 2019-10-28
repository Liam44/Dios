'use strict';

angular.module('cookies')
    .factory('CookiesServices', [
        function () {
            var thisCookiesService = {};

            thisCookiesService.setCookie = function (cname, cvalue, exdays) {
                var expires = '';

                if (exdays) {
                    var d = new Date();

                    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
                    expires = "expires=" + d.toUTCString();
                }

                document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
            }

            thisCookiesService.getCookie = function (cname) {
                var name = cname + "=",
                    decodedCookie = decodeURIComponent(document.cookie),
                    ca = decodedCookie.split(';');

                for (var i = 0; i < ca.length; i++) {
                    var c = ca[i];

                    while (c.charAt(0) == ' ') {
                        c = c.substring(1);
                    }

                    if (c.indexOf(name) == 0) {
                        return c.substring(name.length, c.length);
                    }
                }

                return "";
            }

            return thisCookiesService;
        }
    ]);