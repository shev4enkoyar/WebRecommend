function noDigits(event) {
    if ("abcdefghijklmnopqrstuvwxyz".indexOf(event.key) != -1) {
    }
    else {
        event.preventDefault();
    }
}