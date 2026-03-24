import CompanyWarehousesMain from '../../../pages/company-warehouses/CompanyWarehousesMain.vue';
import CompanyWarehouseShow from '../../../pages/company-warehouses/show/CompanyWarehouseShow.vue';
import CompanyWarehouseEdit from '../../../pages/company-warehouses/edit/CompanyWarehouseEdit.vue';
import CompanyWarehouseCreate from '../../../pages/company-warehouses/create/CompanyWarehouseCreate.vue';
import CompanyWarehouseState from '../../../pages/company-warehouses/warehouse-state/CompanyWarehouseState.vue';
import MainLayout from '../../layouts/MainLayout.vue';

export const companyWarehouseRoutes = [
    {
        path: '/company-warehouses',
        component: MainLayout,
        children: [
            {
                path: '',
                component: CompanyWarehousesMain,
            },
            {
                path: ':id',
                component: CompanyWarehouseShow,
            },
            {
                path: ':id/edit',
                component: CompanyWarehouseEdit,
            },
            {
                path: 'create',
                component: CompanyWarehouseCreate,
            },
            {
                path: 'warehouse-state',
                component: CompanyWarehouseState,
            },
        ],
    },
]