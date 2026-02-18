import {createRouter, createWebHistory} from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: () => import("@/views/home/HomeView.vue"),
    },
    {
      path: '/data-manager',
      name: 'data-manager',
      component: () => import("@/views/data-manager/DataManagerView.vue"),
    },
    {
      path: "/backtest",
      name: "backtest",
      component: () => import("@/views/backtest/backtest-view/BackTestView.vue"),
    },
    {
      path: "/result/:id",
      name: "backtest-result",
      component: () => import("@/views/backtest/backtest-result-view/BacktestResultView.vue"),
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'not-found',
      component: () => import('@/views/not-found/NotFoundView.vue')
    }
  ],
})

export default router
