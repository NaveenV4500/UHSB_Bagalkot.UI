

$(document).ready(function () {
    startSessionTimers();


    $(document).on('mousemove keypress click scroll', function () {
        startSessionTimers();
    });
});

var sessionTimeout = 0.5 * 60 * 1000;   // 2 minutes total
var warningTime = 1.5 * 60 * 1000;    // Show modal after 1.5 min (90 sec)
var countdownSeconds = (sessionTimeout - warningTime) / 1000; // 30 sec
var warningTimer, logoutTimer, countdownInterval;

function startSessionTimers() {
    clearTimeout(warningTimer);
    clearTimeout(logoutTimer);

    warningTimer = setTimeout(showSessionModal, warningTime);
    logoutTimer = setTimeout(autoLogout, sessionTimeout);
}

function showSessionModal() {
    $('#sessionTimeoutModal').modal('show');
    var remaining = countdownSeconds;
    $('#countdown').text(remaining);

    countdownInterval = setInterval(function () {
        remaining--;
        $('#countdown').text(remaining);
        if (remaining <= 0) {
            clearInterval(countdownInterval);
            autoLogout();
        }
    }, 1000);
}

function resetSessionTimers() {
    $('#sessionTimeoutModal').modal('hide');
    clearInterval(countdownInterval);
    startSessionTimers();
}

function autoLogout() {
    clearInterval(countdownInterval);
    window.location.href = '/Account/Logout';  
}

$('#continueBtn').click(function () {
    resetSessionTimers();
});

$('#logoutBtn').click(function () {
    autoLogout();
});


