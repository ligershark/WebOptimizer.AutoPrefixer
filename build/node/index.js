var autoprefixer = require('autoprefixer');
var postcss = require('postcss');


module.exports = function (callback, input, browsers) {
    var plugin = autoprefixer({ browsers: browsers });

    postcss([plugin]).process(input).then(function (result) {
        callback(/* error */ null, result.css);
    });
};