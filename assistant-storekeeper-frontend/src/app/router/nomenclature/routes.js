import MainLayout from '../../layouts/MainLayout.vue';
import NomenclaturesMain from '../../../pages/nomenclatures/NomenclaturesMain.vue';
import NomenclatureShow from '../../../pages/nomenclatures/show/NomenclatureShow.vue';
import NomenclatureEdit from '../../../pages/nomenclatures/edit/NomenclatureEdit.vue';
import NomenclatureCreate from '../../../pages/nomenclatures/crete/NomenclatureCreate.vue';
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
                path: ":id",
                component: NomenclatureShow,
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