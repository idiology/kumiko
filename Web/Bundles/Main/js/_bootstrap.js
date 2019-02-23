// eslint-disable-next-line no-undef
$ = window.jQuery = window.$ = require('jquery')
import Ready from 'document-ready'
import Vue from 'vue'
import Vuex from 'vuex'
// eslint-disable-next-line no-undef
require('bootstrap.native')

Vue.use(Vuex)
import createPersistedState from 'vuex-persistedstate'

import Kumiko from '../vue/kumiko.vue'

const store = new Vuex.Store({
    state: {
        zoom: 10
    },
    mutations: {
        setZoom (state, zoom) {
            state.zoom = zoom
        }
    },
    plugins: [createPersistedState()]
})

Ready(function () {
    if (document.getElementById('kumiko')) {
        new Vue({
            store,
            el: '#kumiko',
            render: h => h(Kumiko)
        })
    }
})