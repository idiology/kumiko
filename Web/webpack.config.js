const webpack = require('webpack')
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
var CopyWebpackPlugin = require('copy-webpack-plugin')
let dev = !(process.env.NODE_ENV === 'production')
const { VueLoaderPlugin } = require('vue-loader')
var ImageminPlugin = require('imagemin-webpack-plugin').default
const OptimizeCssAssetsPlugin = require('optimize-css-assets-webpack-plugin');

module.exports = {
    entry: {
        kumiko: ['babel-polyfill', './Bundles/Main/js/_bootstrap.js', 'bootstrap-loader/lib/bootstrap.loader?configFilePath=../../../.bootstraprc!bootstrap-loader/no-op.js'],
    },
    output: {
        path: __dirname + '/wwwroot/assets/',
        publicPath: './assets/',
        filename: '[name].js'
    },
    resolve: {
        extensions: ['.js', '.vue'],
        alias: {
            'vue$': 'vue/dist/vue.esm.js'
        }
    },
    module: {
        rules: [
            {
                test: /\.vue$/,
                use: 'vue-loader'
            },
            {
                test: /\.(css|sass|scss)$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    'css-loader',
                    'resolve-url-loader?sourceMap',
                    'sass-loader?sourceMap',
                ]
            },
            {
                test: /\.js$/,
                use: 'babel-loader',
                exclude: /node_modules/
            },
            {
                test: /iconz\.(eot|woff|woff2|ttf|svg|otf)$/,
                use: 'file-loader?name=[name]-[md5:hash:base64:10].[ext]&publicPath=/assets/'
            },
            {
                test: /\.svg$/,
                loader: 'svg-inline-loader'
            },
            {
                test: /\.(eot|woff|woff2|ttf|otf|png|jpe?g|gif|htc)$/,
                exclude: /iconz\.(eot|woff|woff2|ttf|svg|otf)$/,
                use: 'file-loader?name=[name].[ext]&publicPath=/assets/'
            },
            {
                test: /\.(jpe?g|png|gif|svg)$/i,
                use: [
                    'img-loader'
                ]
            }
        ]
    },
    plugins: [
        new webpack.ProvidePlugin({
            $: "jquery",
            jQuery: "jquery"
        }),
        new MiniCssExtractPlugin({
            filename: '[name].css'
        }),
        new CopyWebpackPlugin([
            { from: './Bundles/Main/images/*', to: 'main', flatten: true },
        ]),
        new ImageminPlugin({ disable: dev, test: /\.(jpe?g|png|gif|svg)$/i }),
        new VueLoaderPlugin(),
        new webpack.DefinePlugin({
            'process.env': {
                NODE_ENV: `"${process.env.NODE_ENV}"`
            },
            VERSION: JSON.stringify(require('./package.json').version)
        })
    ],
    performance: {
        hints: false
    },
    devtool: dev ? '#eval-source-map' : false
};

if (!dev) {
    module.exports.plugins = (module.exports.plugins || []).concat([
        new webpack.LoaderOptionsPlugin({
            minimize: true
        }),
        new OptimizeCssAssetsPlugin()
    ]);
}