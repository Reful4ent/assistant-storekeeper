<script setup>
    import Breadcrumbs from '../../../shared/breadcrumbs/Breadcrumbs.vue';
    import { breadcrumbs } from './ui/breadcrumbs/breadcrumbs.js';
    import CompanyWarehouseNomenclatureTable from '../ui/CompanyWarehouseForm/ui/CompanyWarehouseNomenclatureTable/CompanyWarehouseNomenclatureTable.vue';
    import { ref, onMounted, reactive } from 'vue';
    import axios from 'axios';
    import { message } from 'ant-design-vue';

    const apiUrl = import.meta.env.VITE_API_URL;

    const date = ref(null);
    const companyWarehousesOptions = ref([]);
    const nomenclatures = ref([]);
    const loading = ref(true);

    const searchData = reactive({
        companyWarehouseId: null,
        requestDate: null,
    });

    const getCompanyWarehouses = async () => {
        try {
            const response = await axios.get(apiUrl + '/api/company-warehouses?pageSize=1000');
            console.log(response.data);
            if (response.status === 200) {
                companyWarehousesOptions.value = response.data.data.map(companyWarehouse => ({
                    label: companyWarehouse.name,
                    value: companyWarehouse.id,
                }));
            }
        } catch (error) {
            message.error('Не удалось получить склады');
        } finally {
            loading.value = false;
        }
    }

    const onSearchClick = async () => {
        try {
            const response = await axios.post(apiUrl + '/api/movements/warehouse-state', searchData);
            nomenclatures.value = response.data.companyWarehouseNomenclatures.map(nomenclature => ({
                ...nomenclature,
                id: nomenclature.nomenclatureId,
            }));
        } catch (error) {
            message.error('Не удалось получить остатки по дате');
        } finally {
            loading.value = false;
        }
    }

    onMounted(async () => {
        await getCompanyWarehouses();
    });
</script>

<template>
    <Breadcrumbs :breadcrumbs="breadcrumbs" />
    <a-card title="Остатки по дате">
        <a-form :model="searchData" name="search-data-form" @finish="onSearchClick" >
            <a-form-item
                label="Склад"
                name="companyWarehouseId"
                :rules="[{ required: true, message: 'Пожалуйста, выберите склад' }]"
            >
                <a-select v-model:value="searchData.companyWarehouseId" :options="companyWarehousesOptions" />
            </a-form-item>
            <a-form-item
                label="Дата"
                name="requestDate"
                class="[&_.ant-form-item-control-input-content]:flex [&_.ant-form-item-control-input-content]:justify-start [&_.ant-form-item-explain]:text-left"
                :rules="[{ required: true, message: 'Пожалуйста, выберите дату' }]"
            >
                <a-date-picker v-model:value="searchData.requestDate" />
            </a-form-item>
            <a-form-item
                class="[&_.ant-form-item-control-input-content]:flex [&_.ant-form-item-control-input-content]:justify-end"
            >
                <a-button type="primary" html-type="submit">Поиск</a-button>
            </a-form-item>
        </a-form>
        <CompanyWarehouseNomenclatureTable :nomenclatures="nomenclatures" :formType="'view'" />
    </a-card>
</template>