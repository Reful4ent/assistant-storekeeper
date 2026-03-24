import MainLayout from '../../layouts/MainLayout.vue';
import NomenclaturesMain from '../../../pages/nomenclatures/NomenclaturesMain.vue';
import NomenclatureEdit from '../../../pages/nomenclatures/edit/NomenclatureEdit.vue';
import NomenclatureCreate from '../../../pages/nomenclatures/create/NomenclatureCreate.vue';
export const nomenclatureRoutes = [
    {
        path: "/nomenclatures",
        component: MainLayout,
        children: [
            {
                path: "",
                component: NomenclaturesMain,
            },
            {
                path: ":id/edit",
                component: NomenclatureEdit,
            },
            {
                path: "create",
                component: NomenclatureCreate,
            },
        ],
    }
]