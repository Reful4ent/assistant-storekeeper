<script setup>
    import { nomenclaturesBaseColumns } from '../columns/columns';
    import { computed, ref } from 'vue';

    const props = defineProps({
        nomenclatures: {
            type: Array,
            required: true,
        },
        formType: {
            type: String,
            required: true,
            validator: (value) => ["create", "edit", "view"].includes(value),
            default: "create",
        },
        onDeleteNomenclature: {
            type: Function,
            required: false,
        },
        onUpdateNomenclatureQuantity: {
            type: Function,
            required: false,
        },
    });

    const editingRecordId = ref(null);
    const editingQuantity = ref(0);

    const nomenclaturesColumns = computed(() => {
        if (props.formType === 'view') {
            return nomenclaturesBaseColumns.filter((col) => col.dataIndex !== 'actions');
        }
        return nomenclaturesBaseColumns;
    });

    const onDeleteClick = (id) => {
        props.onDeleteNomenclature?.(id);
    };


    const onEditClick = (record) => {
        editingRecordId.value = record.id;
        editingQuantity.value = record.quantity;
    };


    const onQuantitySave = (id) => {
        const validQuantity = Math.max(0, Number(editingQuantity.value) || 0);
        props.onUpdateNomenclatureQuantity?.({ id, quantity: validQuantity });
        editingRecordId.value = null;
    };
</script>

<template>
    <a-table 
        :columns="nomenclaturesColumns" 
        :data-source="props.nomenclatures" 
        :pagination="false"
        bordered
    >
        <template #bodyCell="{ column, record }">
            <template v-if="column.dataIndex === 'quantity' && editingRecordId === record.id">
                <div class="flex gap-2 w-full">
                    <a-input-number
                        v-model:value="editingQuantity"
                        :min="0"
                        class="flex-1 min-w-0"
                    />
                        <a-button type="primary" @click="onQuantitySave(record.id)">
                            Сохранить
                        </a-button>
                    </div>
            </template>
            <template v-else-if="column.dataIndex === 'actions' && props.formType === 'edit'">
                <div class="flex gap-2">
                    <a-button
                        type="primary"
                        style="background-color:rgb(4, 160, 74);"
                        @click="onEditClick(record)"
                    >
                                Редактировать
                    </a-button>
                    <a-popconfirm
                        title="Вы уверены, что хотите удалить этот товар со склада?"
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