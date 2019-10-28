(function () {
    'use strict';

    angular
        .module('modal')
        .directive('modal', Directive);

    function Directive(ModalService) {
        return {
            link: function (scope, element, attrs) {
                // ensure id attribute exists
                if (!attrs.id) {
                    console.error('modal must have an id');
                    return;
                }

                // move element to bottom of page (just before </body>) so it can be displayed above everything else
                element.appendTo('body');

                // close modal on background click
                element.on('click', function (e) {
                    var target = $(e.target);
                    if (!target.closest('.modal-body').length) {
                        scope.$evalAsync(Close);
                    }
                });

                // close modal if 'ESC' is pressed
                function CloseOnESC(evt) {
                    evt = evt || window.event;
                    if (evt.keyCode === 27) {
                        scope.$evalAsync(Close);
                    }
                }

                // add self (this modal instance) to the modal service so it's accessible from controllers
                var modal = {
                    'id': attrs.id,
                    open: Open,
                    close: Close
                },
                    modalDiv = element[0].firstElementChild,
                    modalBody = modalDiv.firstElementChild,
                    navbar = document.getElementsByClassName('navbar')[0],
                    offset = { x: 0, y: 0 };

                ModalService.add(modal);

                // remove self from modal service when directive is destroyed
                scope.$on('$destroy', function () {
                    ModalService.remove(attrs.id);

                    window.removeEventListener('mousedown', mouseDown);
                    window.removeEventListener('mouseup', mouseUp);

                    element.remove();
                });

                // open modal
                // Expected parameter:
                // {
                //  top: top starting position of the window (number - optional),
                //  left: left starting position of the window (number - optional),
                //  movable: indicates if the window may or not be moved on the page ( - optional)
                // }
                function Open(params) {
                    var minTop = 0;

                    if (navbar)
                        minTop = (navbar.style.top | 0) + navbar.clientHeight + 1;

                    element.show();
                    $('body').addClass('modal-open');
                    document.addEventListener('keydown', CloseOnESC);

                    // Sets the max height to the client height
                    // -> avoids to have to scroll the whole page down
                    // -> scrolls the modal down instead
                    modalBody.style.maxHeight = modalDiv.clientHeight - minTop - 1 + 'px';

                    if (typeof params.top === 'number') {
                        modalBody.style.top = params.top + 'px';
                    }
                    else {
                        // Middle top
                        modalBody.style.top = Math.max(((modalDiv.offsetHeight - modalBody.offsetHeight) + minTop) / 2, minTop) + 'px';
                    }

                    if (typeof params.left === 'number') {
                        modalBody.style.left = params.left + 'px';
                    }
                    else {
                        // Middle left
                        modalBody.style.left = (modalDiv.clientWidth - modalBody.clientWidth) / 2 - 40 + 'px';
                    }

                    if (typeof params.movable !== 'undefined') {
                        // Add the needed events for the drag and move options
                        modalDiv.addEventListener('mousedown', mouseDown, false);
                        window.addEventListener('mouseup', mouseUp, false);
                    }
                }

                // close modal
                function Close() {
                    element.hide();

                    $('body').removeClass('modal-open');
                    document.removeEventListener('keydown', CloseOnESC);
                    window.removeEventListener('mousedown', mouseDown);
                }

                function mouseUp() {
                    modalDiv.style.cursor = 'default';
                    window.removeEventListener('mousemove', popupMove, true);
                }

                function mouseDown(e) {
                    modalBody.style.cursor = 'move';
                    offset.x = e.clientX - modalBody.offsetLeft;
                    offset.y = e.clientY - modalBody.offsetTop;
                    window.addEventListener('mousemove', popupMove, true);
                }

                function popupMove(e) {
                    var top = e.clientY - offset.y,
                        left = e.clientX - offset.x;

                    modalBody.style.top = top + 'px';
                    modalBody.style.left = left + 'px';
                }
            }
        };
    }
})();