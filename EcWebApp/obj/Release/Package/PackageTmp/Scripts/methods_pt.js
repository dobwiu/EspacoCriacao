/*
 * Localized default methods for the jQuery validation plugin.
 * Locale: PT_BR
 * http://cleytonferrari.com/validacao-de-data-e-moeda-asp-net-mvc-jquery-validation-em-portugues/
 */
jQuery.extend(jQuery.validator.methods, {
    date: function (value, element) {
        return this.optional(element) || /^\d\d?\/\d\d?\/\d\d\d?\d?$/.test(value);
    },
    number: function (value, element) {
        return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:\.\d{3})+)(?:,\d+)?$/.test(value);
    }
});