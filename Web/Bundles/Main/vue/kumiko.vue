<template>
    <div>
        <loading :active="loading" type="rotating-square" :fullscreen="true"></loading> 
        <alert type="danger" :errors="errors"></alert>
        <div v-if="playlist != null && music != null && !loading">
            <aplayer :music="music" :list="playlist" :listMaxHeight="listMaxHeight" />
        </div>
    </div>
</template>
<script>
  import Vue from 'vue'
  import axios from 'axios'
  import Aplayer from '../../vue-aplayer-kumiko/src/vue-aplayer.vue'
  import { Loading, Alert } from 'vue-nani-kore'

  Vue.component('alert', Alert)
  Vue.component('loading', Loading)

  export default {
    components: {
      Aplayer
    },
    data () {
      return {
        playlist: null,
        music: null,
        errors: [],
        loading: true
      }
    },
    created () {
      this.loadPlaylist()
    },
    computed: {
      listMaxHeight () {
        const l = 44
        const minus = 7 * l
        let final = 0
        let max = Math.max(
          document.documentElement.clientHeight,
          window.innerHeight || 0
        )

        max -= minus

        if (max < 0) {
          return ''
        }

        while (true) {
          final += l
          if (final > max) {
            final -= l
            break
          }
        }

        return final + 'px'
      }
    },
    methods: {
      loadPlaylist () {
        let self = this
        axios
          .all([axios.get('/api/playlist')])
          .then(
            axios.spread(playlist => {
              self.playlist = playlist.data
              if (self.playlist != null && self.playlist.length > 0) {
                self.music = self.playlist[0]
              }
              self.loading = false
            })
          )
          .catch(function (error) {
            self.playlist = []
            self.loading = false
            try {
              self.errors = error.response.data
            } catch (e) {
              console.log('ERROR', e)
            }
          })
      }
    }
  }
</script>