<script setup>
 import { ref } from 'vue';
 import axios from 'axios';
 import { onMounted } from 'vue';
 import { useRouter } from 'vue-router';
 import { message } from 'ant-design-vue';
 
 const companyWarehouses = ref([]);
 const search = ref('');
 const loading = ref(true);
 const router = useRouter();
 const columns = ref([
    {
        title: 'ID',
        dataIndex: 'id',
        key: 'id',
        width: "1%",
    },
    {
        title: 'Название',
        dataIndex: 'name',
        key: 'name',
    },
    {
        title: 'Действия',
        dataIndex: 'actions',
        key: 'actions',
        width: "20%",
    }
 ]);
 const apiUrl = import.meta.env.VITE_API_URL;

 const getCompanyWarehouses = async () => {
    try {
        const response = await axios.get(apiUrl + '/api/company-warehouses');
        companyWarehouses.value = response.data.data;
    } catch (error) {
        message.error('Не удалось получить склады');
    } finally {
        loading.value = false;
    }
 }

 const onEditClick = (id) => {
    router.push(`/company-warehouses/${id}/edit`);
 }

 const onShowClick = (id) => {
    console.log(id);
    router.push(`/company-warehouses/${id}`);
 }

 const onAddClick = () => {
    router.push('/company-warehouses/create');
 }

 const onDeleteClick = async (id) => {
    try {
        const response = await axios.delete(apiUrl + `/api/company-warehouses/${id}`);
        if (response.status === 204) {
            message.success('Склад успешно удален');
            await getCompanyWarehouses();
        }
    } catch (error) {
        message.error('Не удалось удалить склад');
    }
 }

 onMounted(async () => {
    await getCompanyWarehouses();
 })
</script>

<template>
    <div class="flex justify-end mb-4">
        <a-button type="primary"@click="onAddClick">
            Добавить склад
        </a-button>
    </div>
    <a-table bordered :columns="columns" :data-source="companyWarehouses" :loading="loading">
        <template #bodyCell="{ column, record }">
            <template v-if="column.dataIndex === 'actions'">
                <div class="flex gap-2">
                    <a-button 
                        type="primary"
                        style="background-color:rgb(4, 160, 74); "
                        @click="onEditClick(record.id)"
                    >
                        Редактировать
                    </a-button>
                    <a-button 
                        type="primary" 
                        @click="onShowClick(record.id)"
                    >
                        Посмотреть
                    </a-button>
                    <a-popconfirm 
                        title="Вы уверены, что хотите удалить этот склад?" 
                        @confirm="onDeleteClick(record.id)"
                    >
                        <a-button type="primary" danger>
                            Удалить
                        </a-button>
                    </a-popconfirm>
                </div>
            </template>
        </template>
    </a-table>
</template>