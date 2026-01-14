import './assets/main.css'
import {createApp} from 'vue'
import {createRouter, createWebHistory} from 'vue-router'
import App from './App.vue'

import 'vuetify/styles'
import {createVuetify} from 'vuetify'
import * as vuetifyComponents from 'vuetify/components'
import * as directives from 'vuetify/directives'
import '@mdi/font/css/materialdesignicons.css'

import Dashboard from "@/views/Dashboard.vue";
import Backtester from "@/views/Backtester.vue";
import HistoryManager from "@/views/HistoryManager.vue";
import HistoryBrowse from "@/views/history/HistoryBrowse.vue";
import HistoryCreate from "@/views/history/HistoryCreate.vue";
import HistoryDetails from "@/views/history/HistoryDetails.vue";

import {useErrorSnackbar} from "@/composables/useErrorSnackbar.js";

const {showGlobalError} = useErrorSnackbar()


const vuetify = createVuetify({
  components: vuetifyComponents,
  directives,
  theme: {
    defaultTheme: 'dark',
    themes: {
      dark: {
        colors: {
          primary: '#546e7a',
          secondary: '#00897b',
          accent: '#ffa000',
          error: '#e64a19',
          info: '#0097a7',
          success: '#43a047',
          warning: '#f57c00',
          background: '#263238',
          surface: '#37474f',
        }
      }
    }
  }
})

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      component: Dashboard,
    },
    {
      path: '/history',
      component: HistoryManager,
      children: [
        {
          path: 'browse', component: HistoryBrowse
        },
        {
          path: 'create', component: HistoryCreate
        },
        {
          path: 'details/:id',
          component: HistoryDetails,
          props: true
        },
      ]
    },
    {
      path: '/backtester',
      component: Backtester
    }
  ]
})

const app = createApp(App)

app.config.errorHandler = (err, instance, info) => {
  console.error('Global error:', err)
  console.error('Component:', instance)
  console.error('Error info:', info)

  showGlobalError(err);
}

app.use(router).use(vuetify).mount('#app')
