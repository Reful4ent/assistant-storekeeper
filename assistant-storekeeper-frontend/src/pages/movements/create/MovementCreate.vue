<script setup>
    import { breadcrumbs } from './ui/breadcrumbs/breadcrumbs';
    import Breadcrumbs from '../../../shared/breadcrumbs/Breadcrumbs.vue';
    import { statuses } from '../ui/columns/statuses';
    import { reactive, ref, onMounted, watch } from 'vue';
    import axios from 'axios';
    import { message } from 'ant-design-vue';

    const apiUrl = import.meta.env.VITE_API_URL;


    const statusesOptions = statuses.map(status => ({
        label: status.label,
        value: status.value,
    }));

    const allNomenclaturesOptions = ref([]);

    const companyWarehousesOptions = ref([]);

    const nomenclaturesByFromWarehouseId = ref([]);

    const nomenclaturesByFromWarehouseIdOptions = ref([]);

    const loading = ref(true);

    const submitLoading = ref(false);

    const movement = reactive({
        status: null,
        companyWarehouseFromId: null,
        companyWarehouseToId: null,
        nomenclatures: [{
            nomenclatureId: null,
            quantity: null,
        }],
    });

    const getCompanyWarehouses = async () => {
        loading.value = true;
        try {
            const response = await axios.get(apiUrl + '/api/company-warehouses');
            companyWarehousesOptions.value = response.data.data.map(companyWarehouse => ({
                label: companyWarehouse.name,
                value: companyWarehouse.id,
            }));
        } catch (error) {
            message.error('Не удалось получить склады!');
        }
        finally {
            loading.value = false;
        }
    }
    
    const getAllNomenclatures = async () => {
        try {
            const pageSize = 1000;
            const response = await axios.get(apiUrl + '/api/nomenclatures?pageSize=' + pageSize);
            allNomenclaturesOptions.value = response.data.data.map(nomenclature => ({
                label: nomenclature.name,
                value: nomenclature.id,
            }));
        } catch (error) {
            message.error('Не удалось получить товары!');
        }
        finally {
            loading.value = false;
        }
    }

    const getNomenclaturesByFromWarehouseId = async (fromWarehouseId) => {
        try {
            const response = await axios.get(apiUrl + '/api/company-warehouse-nomenclatures?companyWarehouseId=' + fromWarehouseId);
            nomenclaturesByFromWarehouseId.value = response.data;
            console.log(nomenclaturesByFromWarehouseId.value);
            nomenclaturesByFromWarehouseIdOptions.value = response.data.map(nomenclature => ({
                label: nomenclature.nomenclatureName,
                value: nomenclature.id,
            }));
        } catch (error) {
            message.error('Не удалось получить товары!');
        }
    }

    watch(() => [movement.status, movement.companyWarehouseFromId], async ([newStatus, newCompanyWarehouseFromId]) => {
        if (newStatus && newCompanyWarehouseFromId) {
            if (newStatus === 1 || newStatus === 2) {
                await getNomenclaturesByFromWarehouseId(newCompanyWarehouseFromId);
            } else {
                nomenclaturesByFromWarehouseIdOptions.value = [];
            }
        }
    });

    watch(() => movement.status, async (newStatus) => {
        if (newStatus) {
            movement.nomenclatures = [{
            nomenclatureId: null,
            quantity: null,
        }];
            movement.companyWarehouseFromId = null;
            movement.companyWarehouseToId = null;
        }
    });

    const onFinish = async (values) => {
        console.log(values);
    }

    const addNomenclature = () => {
        movement.nomenclatures.push({
            nomenclatureId: null,
            quantity: null,
        });
    }

    const removeNomenclature = (index) => {
        movement.nomenclatures.splice(index, 1);
    }

    const validateQuantity = (nomenclatureId) => {
        return (rule, value, callback) => {
            if (value < 1) {
                callback(new Error('Количество должно быть больше 0!'));
                return;
            }
            if (movement.status === 0) {
                callback();
                return;
            }
            if (value > nomenclaturesByFromWarehouseId.value.find(nomenclature => nomenclature.id === nomenclatureId).quantity) {
                callback(new Error('Количество не может быть больше количества на складе!'));
                return;
            }
            callback();
            return;
        }
    }

    const createMovement = async () => {
        console.log(movement);
    }

    onMounted(async () => {
        await getCompanyWarehouses();
        await getAllNomenclatures();
    });
</script>

<template>
    <Breadcrumbs :breadcrumbs="breadcrumbs" />
    <a-card title="Создание перемещения">
        <a-form 
            :model="movement"
            name="movement-create-form"
            @finish="onFinish"
        >
            <a-form-item
                label="Тип перемещения"
                name="status"
                :rules="[{ required: true, message: 'Выберите тип перемещения!' }]"
            >
                <a-select v-model:value="movement.status" :options="statusesOptions" />
            </a-form-item>
            <a-form-item 
                v-if="movement.status === 1 || movement.status === 2"
                label="Склад отправления"
                name="companyWarehouseFromId"
                :rules="[{ required: true, message: 'Выберите склад отправления!' }]"
            >
                <a-select 
                    v-model:value="movement.companyWarehouseFromId" 
                    :options="companyWarehousesOptions"
                />
            </a-form-item>
            <a-form-item 
                v-if="movement.status === 0 || movement.status === 2"
                label="Склад назначения"
                name="companyWarehouseToId"
                :rules="[{ required: true, message: 'Выберите склад назначения!' }]"
            >
                <a-select 
                    v-model:value="movement.companyWarehouseToId" 
                    :options="companyWarehousesOptions"
                />
            </a-form-item>
            <a-form-item v-if="movement.status !== null && (movement.companyWarehouseFromId || movement.companyWarehouseToId)">
                <a-typography-title :level="5" class="flex justify-start">Товары</a-typography-title>
                <div
                    class="grid grid-cols-5 gap-10"
                >
                    <a-card
                        v-for="(nomenclature, index) in movement.nomenclatures" 
                        :key="index"
                        class="w-80"
                        size="small"
                        :title="`Товар ${index + 1}`"
                    >
                        <template #extra>
                            <a-button
                                v-if="movement.nomenclatures.length > 1"
                                type="primary" 
                                danger
                                @click="removeNomenclature(index)"
                            >
                                X
                            </a-button>
                        </template>
                        <a-form-item 
                            label="Товар" 
                            :name="['nomenclatures', index, 'nomenclatureId']" 
                            :rules="[{ required: true, message: 'Выберите товар!' }]"
                        >
                            <a-select 
                                v-model:value="nomenclature.nomenclatureId"
                                :options="movement.status === 0 ? allNomenclaturesOptions : nomenclaturesByFromWarehouseIdOptions"
                            />
                        </a-form-item>
                        <a-form-item 
                            label="Количество" 
                            :name="['nomenclatures', index, 'quantity']"
                            :rules="[{ required: true, message: 'Введите количество!' }, { validator: validateQuantity(nomenclature.nomenclatureId) }]"
                            class="w-full [&_.ant-form-item-control-input-content]:block [&_.ant-form-item-control-input-content]:w-full [&_.ant-input-number]:!w-full"
                        >
                            <a-input-number v-model:value="nomenclature.quantity" class="w-full" type="number" />
                        </a-form-item>
                        <div class="flex justify-start" v-if="movement.status !== 0">
                            <span class="text-blue-500 font-bold">
                                    На складе: {{ nomenclaturesByFromWarehouseId.find(value => value.id == nomenclature.nomenclatureId)?.quantity }} шт.
                            </span>
                        </div>
                    </a-card>
                </div>
                <div class="mt-5">
                    <a-button 
                        type="primary" 
                        @click="addNomenclature"
                    >
                        Добавить товар
                    </a-button>
                </div>
            </a-form-item>
            <a-form-item class="[&_.ant-form-item-control-input-content]:flex [&_.ant-form-item-control-input-content]:justify-end [&_.ant-form-item-control-input-content]:w-full">
                <a-button 
                    type="primary" 
                    html-type="submit" 
                    :loading="submitLoading"
                >
                    Создать перемещение
                </a-button>
            </a-form-item>
        </a-form>   
    </a-card>
</template>