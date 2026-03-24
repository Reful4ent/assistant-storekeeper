import { createRouter, createWebHistory } from 'vue-router';
import { companyWarehouseRoutes } from './company-wearehouse/routes';
import { nomenclatureRoutes } from './nomenclature/routes';
import { movementRoutes } from './movement/routes';
import MainLayout from '../layouts/MainLayout.vue';
import NotFoundView from '../../pages/not-found/NotFoundView.vue';
const router = createRouter({
  history: createWebHistory(),
  routes: [
    ...companyWarehouseRoutes,
    ...nomenclatureRoutes,
    ...movementRoutes,
    {
        path: '/:pathMatch(.*)*',
        component: MainLayout,
        children: [
            {
                path: '/:pathMatch(.*)*',
                component: NotFoundView,
            },
        ],
    },
    {
        path: '/',
        redirect: '/company-warehouses',
    }
  ],
});

export default router;