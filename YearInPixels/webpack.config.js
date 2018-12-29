/// <binding ProjectOpened='Watch - Development' /> 
"use strict";

var path = require('path');
const VueLoaderPlugin = require('vue-loader/lib/plugin');

module.exports = {
    mode: 'development', //production
    entry: {
        main: './Scripts/app'
    },
    output: {
        publicPath: "/js/",
        path: path.join(__dirname, '/wwwroot/js/'),
        filename: 'dist.js'
    },
    module: {
        rules: [
            {
                test: /\.vue$/,
                loader: 'vue-loader',
                options: {
                    loaders: {
                        js: 'babel-loader',
                        css: 'less-loader'
                    }
                }
            },
            {
                test: /\.less$/,
                use: [
                    'vue-style-loader',
                    'css-loader',
                    'less-loader'
                ]
            },
        ]
    },
    plugins: [
        // make sure to include the plugin!
        new VueLoaderPlugin()
    ]
};