$(document).ready(function () {
    $(".enterPress").keypress(function (e) {
        if (e.which == 32) {
            var input = $(".enterPress").val();
            if (input == " " || input == "") {
                return false;
            }
            else {
                var tags = $(".allTags").val().split(' ');
                if (!tags.includes(input)) {
                    var allTag = $(".allTags").val();
                    $(".allTags").val(allTag + " " + input);
                    $(".enterPress").val('');
                }
                else {
                    $(".enterPress").val('');
                }

            }
            return false;
        }
    });
});