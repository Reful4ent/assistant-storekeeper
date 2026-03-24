import { createApp } from 'vue'
import Antd from 'ant-design-vue'
import 'ant-design-vue/dist/reset.css'
import './style.css'
import App from './App.vue'
import router from './app/router'

const app = createApp(App)
app.use(router).use(Antd).mount('#app')
