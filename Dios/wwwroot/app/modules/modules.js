// Allows to display modal windows such as menus...
angular.module('modal', []);

angular.module('cookies', []);

angular.module('users', ['modal', 'cookies']);

angular.module('addresses', ['cookies']);

angular.module('flats', []);

angular.module('hosts', []);