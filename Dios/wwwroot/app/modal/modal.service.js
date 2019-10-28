(function () {
    'use strict';

    angular
        .module('modal')
        .factory('ModalService', Service);

    function Service() {
        var modals = [], // array of modals on the page
            service = {},
            currentModal = undefined;

        service.add = add;
        service.remove = remove;
        service.open = open;
        service.close = close;

        return service;

        function add(modal) {
            // add modal to array of active modals
            modals.push(modal);
        }

        function remove(id) {
            // remove modal from array of active modals
            var modalToRemove = FindWhere(id);
            modals = modals.filter(i => ![modalToRemove].includes(i));// _.without(modals, modalToRemove);
        }

        // Expected parameter:
        // {
        //  id: unique ID of the window to be shown (string - mandatory)
        //  top: top starting position of the window (number - optional),
        //  left: left starting position of the window (number - optional),
        //  movable: indicates if the window may or not be movable (bool - optional)
        // }
        function open(params) {
            // Close all other modals before opening another one
            close();

            // open modal specified by id
            currentModal = FindWhere(params.id);
            if (currentModal) {
                currentModal.open({ 'top': params.top, 'left': params.left, 'movable': params.movable });
            }
        }

        function close() {
            if (currentModal) {
                currentModal.close();

                currentModal = undefined;
            }
        }

        function FindWhere(id) {
            for (var i = 0; i < modals.length; i++) {
                if (modals[i].id === id) {
                    return modals[i];
                }
            }
        }
    }

})();
