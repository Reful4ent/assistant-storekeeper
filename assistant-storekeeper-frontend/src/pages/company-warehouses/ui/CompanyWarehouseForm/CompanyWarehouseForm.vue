<script setup>
    import CompanyWarehouseNomenclatureTable from './ui/CompanyWarehouseNomenclatureTable/CompanyWarehouseNomenclatureTable.vue';
    const props = defineProps({
        form: {
            type: Object,
            required: true,
        },
        onFinish: {
            type: Function,
            required: false,
        },
        loading: {
            type: Boolean,
            required: true,
        },
        title: {
            type: String,
            required: true,
        },
        formType: {
            type: String,
            required: true,
            validator: (value) => ["create", "edit", "view"].includes(value),
            default: "create",
        },
        submitButtonText: {
            type: String,
            required: false,
            default: "Создать склад",
        },
        nomenclatures: {
            type: Array,
            required: false,
            default: () => [],
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
</script>


<template>
    <a-card :title="props.title">
      <a-form 
        :model="props.form"
        name="company-warehouse-create-form"
        @finish="props.onFinish"
        :loading="props.loading"
      >
        <a-form-item
          label="Название склада"
          name="name"
          :rules="[{ required: true, message: 'Введите название склада!' }]"
        >
          <a-input
             v-model:value="props.form.name" 
             placeholder="Введите название склада"
             :readonly="props.formType === 'view'"
          />
        </a-form-item>


        <a-form-item 
          v-if="props.formType === 'view' || props.formType === 'edit'" 
        >
            <a-typography-title :level="5" class="flex justify-start">На складе</a-typography-title>
            <CompanyWarehouseNomenclatureTable
                :nomenclatures="props.nomenclatures"
                :onDeleteNomenclature="props.onDeleteNomenclature"
                :onUpdateNomenclatureQuantity="props.onUpdateNomenclatureQuantity"
                :formType="props.formType"
            />
        </a-form-item>

        <a-form-item v-if="props.formType === 'create' || props.formType === 'edit'" class="[&_.ant-form-item-control-input-content]:flex [&_.ant-form-item-control-input-content]:justify-end">
          <a-button type="primary" html-type="submit" :loading="props.loading">{{ props.submitButtonText }}</a-button>
        </a-form-item>
      </a-form>
    </a-card>
</template>