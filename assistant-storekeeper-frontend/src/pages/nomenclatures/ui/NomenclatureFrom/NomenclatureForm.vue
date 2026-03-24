<script setup>
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
        submitButtonText: {
            type: String,
            required: false,
            default: "Создать номенклатуру",
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
          label="Название номенклатуры"
          name="name"
          :rules="[{ required: true, message: 'Введите название номенклатуры!' }]"
        >
          <a-input
             v-model:value="props.form.name" 
             placeholder="Введите название номенклатуры"
          />
        </a-form-item>


        <a-form-item class="[&_.ant-form-item-control-input-content]:flex [&_.ant-form-item-control-input-content]:justify-end">
          <a-button type="primary" html-type="submit" :loading="props.loading">{{ props.submitButtonText }}</a-button>
        </a-form-item>
      </a-form>
    </a-card>
</template>