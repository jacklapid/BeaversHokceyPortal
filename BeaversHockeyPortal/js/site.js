function logoff() {
    try {
        document.getElementById('logoutForm').submit();
    }
    catch (err) {
        alert(err);
    }

    return false;
};

