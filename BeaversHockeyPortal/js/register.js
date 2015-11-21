
$(document).ready(function () {
    Sys.Application.add_load(function () {
        var optionList = $('role-selector');

        onRoleChanged(optionList);
    });
});

function onRoleChanged(optionList) {
    var $list = $(optionList);

    var isPlayer = $list.val() == 'Player';

    if (isPlayer) {
        $('.player-only').show();
    }
    else {
        $('.player-only').hide();
    }
};