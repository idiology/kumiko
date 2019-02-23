<template>
  <transition name="slide-v">
    <div class="aplayer-list" :style="listHeightStyle" ref="list" v-show="show">
      <ol ref="ol" :style="listHeightStyle">
        <li v-for="(aMusic, index) of musicList" :key="index" :class="{'aplayer-list-light': aMusic === currentMusic}" @click="$emit('selectsong', aMusic)">
          <span class="aplayer-list-cur" :style="{background: theme}"></span>
          <span class="aplayer-list-index">{{ showIndex(index) }}</span>
          <div class="aplayer-list-left">{{ aMusic.title || 'Untitled' }}<br /><span class="second">{{ aMusic.artist || 'Unknown' }}</span></div>          
          <div class="aplayer-list-right">{{ aMusic.anime }} <span class="second">{{ aMusic.type }}</span></div>
        </li>
      </ol>
    </div>
  </transition>
</template>
<script>
  export default {
    props: {
      show: {
        type: Boolean,
        default: true
      },
      currentMusic: Object,
      musicList: {
        type: Array,
        default () {
          return []
        }
      },
      playIndex: {
        type: Number,
        default: 0
      },
      theme: String,
      listmaxheight: String
    },
    computed: {
      listHeightStyle () {
        return {
          height: `${(2 * 22 * (this.musicList.length)) + 2}px`,
          maxHeight: this.listmaxheight || ''
        }
      },
      listPadding () {
        var max = this.musicList.length
        return max.toString().length
      }
    },
    methods: {
      showIndex (index) {        
        return (index + 1).toString().padStart(this.listPadding, '0')
      }
    }
  }
</script>

<style lang="scss">
  .aplayer-list {
    overflow: hidden;

    &.slide-v-enter-active,
    &.slide-v-leave-active {
      transition: height 500ms ease;
      will-change: height;
    }

    &.slide-v-enter,
    &.slide-v-leave-to {
      height: 0 !important;
    }

    ol {
      list-style-type: none;
      margin: 0;
      padding: 0;
      overflow-y: auto;

      &::-webkit-scrollbar {
        width: 5px;
      }

      &::-webkit-scrollbar-track {
        background-color: #f9f9f9;
      }

      &::-webkit-scrollbar-thumb {
        border-radius: 3px;
        background-color: #eee;
      }

      &::-webkit-scrollbar-thumb:hover {
        background-color: #ccc;
      }

      &:hover {
        li.aplayer-list-light:not(:hover) {
          background-color: inherit;
          transition: inherit;
        }
      }

      &:not(:hover) {
        li.aplayer-list-light {
          transition: background-color 0.6s ease;
        }
      }
      li {
        position: relative;
        height: 2 * 22px;
        line-height: 22px;
        padding: 0 15px;
        font-size: 12px;
        border-top: 1px solid #e9e9e9;
        cursor: pointer;
        transition: all 0.2s ease;
        overflow: hidden;
        margin: 0;
        text-align: start;
        display: flex;

        &:first-child {
          border-top: none;
        }

        &:hover {
          background: #efefef;
        }

        &.aplayer-list-light {
          background: #efefef;

          .aplayer-list-cur {
            display: inline-block;
          }
        }

        .aplayer-list-cur {
          display: none;
          width: 4px;
          height: 34px;
          position: absolute;
          left: 0;
          top: 5px;
          transition: background-color 0.3s;
        }
        .aplayer-list-index {
          color: #666;
          margin-right: 12px;
        }
        .aplayer-list-left {
          flex-grow: 1;
          font-weight: bold;
          color: #ff9933;
          > .second {
            font-weight: normal;
            color: #555;
            text-overflow: ellipsis;
            //white-space: nowrap;
            overflow: hidden;
            //width:100px;
          }
        }
        .aplayer-list-right {
          flex-shrink: 0;
          color: #ff9933;
          float: right;
          > .second {
            color: #555;
            text-align: right;
            display: block;
          }
        }
      }
    }
  }
</style>
