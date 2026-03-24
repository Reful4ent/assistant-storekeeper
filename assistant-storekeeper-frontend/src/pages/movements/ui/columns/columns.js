import { Tag } from 'ant-design-vue';
import { statuses } from './statuses';
export const columns = [
    {
        title: 'ID',
        dataIndex: 'id',
        key: 'id',
        sorter: true,
    },
    {
        title: 'Откуда',
        dataIndex: 'companyWarehouseFromName',
        key: 'companyWarehouseFromName',
        sorter: true,
    },
    {
        title: 'Куда',
        dataIndex: 'companyWarehouseToName',
        key: 'companyWarehouseToName',
        sorter: true,
    },
    {
        title: 'Дата',
        dataIndex: 'date',
        key: 'date',
        sorter: true,
    },
    {
        title: 'Статус',
        dataIndex: 'status',
        key: 'status',
        width: "5%",
        sorter: true,
    },
    {
        title: 'Действия',
        dataIndex: 'actions',
        key: 'actions',
        width: "15%",
    }
];