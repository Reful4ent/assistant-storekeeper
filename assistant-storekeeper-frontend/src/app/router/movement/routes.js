import MainLayout from '../../layouts/MainLayout.vue';
import MovementsMain from '../../../pages/movements/MovementsMain.vue';
import MovementShow from '../../../pages/movements/show/MovementShow.vue';
import MovementCreate from '../../../pages/movements/create/MovementCreate.vue';
export const movementRoutes = [
    {
        path: "/movements",
        component: MainLayout,
        children: [
            {
                path: "",
                component: MovementsMain,
            },
            {
                path: ":id",
                component: MovementShow,
            },
            {
                path: "create",
                component: MovementCreate,
            },
        ],
    }
]