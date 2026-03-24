<script setup>
    import { breadcrumbs } from './ui/breadcrumbs/breadcrumbs';
    import Breadcrumbs from '../../../shared/breadcrumbs/Breadcrumbs.vue';
    import axios from 'axios';
    import { useRoute } from 'vue-router';
    import { statuses } from '../ui/columns/statuses';
    import { ref, reactive, onMounted } from 'vue';
    import { message } from 'ant-design-vue';
    import NomenclatureTable from '../../company-warehouses/ui/CompanyWarehouseForm/ui/CompanyWarehouseNomenclatureTable/CompanyWarehouseNomenclatureTable.vue';
    
    const apiUrl = import.meta.env.VITE_API_URL;

    const route = useRoute();
    const id = route.params.id;

    const movement = reactive({
        id: null,
        date: null,
        companyWarehouseFromName: null,
        companyWarehouseToName: null,
        status: null,
    });
    const nomenclatures = ref([]);
    const loading = ref(true);

    const getMovement = async () => {
        try {
            const response = await axios.get(apiUrl + '/api/movements/' + id);
            movement.id = response.data.id;
            movement.date = response.data.date;
            movement.companyWarehouseFromName = response.data.companyWarehouseFromName;
            movement.companyWarehouseToName = response.data.companyWarehouseToName;
            movement.status = response.data.status;
            nomenclatures.value = response.data.nomenclatures;
        } catch (error) {
            message.error(error);
        }
        finally {
            loading.value = false;
        }
    }

    onMounted(async () => {
        await getMovement();
    });
</script>

<template>
    <Breadcrumbs :breadcrumbs="breadcrumbs" />
    <a-card title="Просмотр перемещения">
        <a-form :model="movement" name="movement-show-form" :loading="loading">
            <a-form-item
                label="Статус"
                name="status"
                class="[&_.ant-form-item-control-input-content]:text-left"
            >
                <a-tag :color="statuses.find(status => status.value == movement.status)?.color">
                    {{ statuses.find(status => status.value == movement.status)?.label }}
                </a-tag>
            </a-form-item>
            <a-form-item
                label="Дата"
                name="date"
                class="[&_.ant-form-item-control-input-content]:text-left"
            >
                <span>{{ new Date(movement.date).toLocaleDateString('ru-RU') }}</span>
            </a-form-item>
            <a-form-item
                label="Склад отправления"
                name="companyWarehouseFromName"
                v-if="movement.companyWarehouseFromName != null"
            >
                <a-input
                    v-model:value="movement.companyWarehouseFromName" 
                    placeholder="Склад отправления"
                    :readonly="true"
                />
            </a-form-item>
            <a-form-item
                label="Склад назначения"
                name="companyWarehouseToName"
                v-if="movement.companyWarehouseToName != null"
            >
                <a-input
                    v-model:value="movement.companyWarehouseToName" 
                    placeholder="Склад назначения"
                    :readonly="true"
                />
            </a-form-item>
            <a-form-item>
                <a-typography-title :level="5" class="flex justify-start">Товары</a-typography-title>
                <NomenclatureTable :nomenclatures="nomenclatures" :formType="'view'" />
            </a-form-item>
        </a-form>
    </a-card>
</template>