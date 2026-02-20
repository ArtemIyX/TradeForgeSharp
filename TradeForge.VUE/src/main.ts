import {createApp} from 'vue'
import {createPinia} from 'pinia'
import {createVuetify} from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import 'vuetify/styles'
import '@mdi/font/css/materialdesignicons.css'

import ECharts from 'vue-echarts'
import {use} from 'echarts/core'

// Import required ECharts components
import {CanvasRenderer} from 'echarts/renderers'
import {CandlestickChart, LineChart, BarChart, ScatterChart, PieChart} from 'echarts/charts'
import {
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent,
  DataZoomComponent,
  MarkLineComponent,
  MarkPointComponent,
  GraphicComponent
} from 'echarts/components'

// Register components
use([
  CanvasRenderer,
  CandlestickChart,
  LineChart,
  BarChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent,
  DataZoomComponent,
  MarkLineComponent,
  MarkPointComponent,
  ScatterChart,
  GraphicComponent,
  PieChart
])

import App from './App.vue'
import router from './router'

const app = createApp(App)

const vuetify = createVuetify({
  components,
  directives,
})

app.component('v-chart', ECharts)
app.use(vuetify)
app.use(createPinia())
app.use(router)

app.mount('#app')
