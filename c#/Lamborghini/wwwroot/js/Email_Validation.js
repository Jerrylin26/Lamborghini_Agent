
console.log("Before addMethod");

jQuery.validator.addMethod("myemail", function (value, element) {
    console.log("Validating email:", value);
    if (this.optional(element)) return true;
    var pattern = /^[\w\.-]+@[\w\.-]+\.\w{2,}$/;
    return pattern.test(value);
});
console.log("After addMethod");

jQuery.validator.unobtrusive.adapters.addBool("myemail");

console.log("After addBool");

// 3. 解析表單（如果表單已經在 DOM 上）
jQuery(function () {
    console.log("Parsing form...");
    jQuery.validator.unobtrusive.parse("#myForm");
});
