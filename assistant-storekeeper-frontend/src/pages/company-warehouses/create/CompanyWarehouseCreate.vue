<script setup>
  import Breadcrumbs from '../../../shared/breadcrumbs/Breadcrumbs.vue';
  import { breadcrumbs } from './ui/breadcrumbs/breadcrumbs';
  import { reactive, ref } from 'vue';
  import { message } from 'ant-design-vue';
  import axios from 'axios';
  import { useRouter } from 'vue-router';
  import CompanyWarehouseForm from '../ui/CompanyWarehouseForm/CompanyWarehouseForm.vue';

  const apiUrl = import.meta.env.VITE_API_URL;

  const router = useRouter();

  const loading = ref(false);

  const form = reactive({
    name: '',
  });

  const onFinish = async (values) => {
    loading.value = true;
    try {
      const response = await axios.post(apiUrl + '/api/company-warehouses', values);
      console.log(response);
      if (response.status === 201 ) {
        message.success('Склад создан успешно');
        loading.value = false;
        router.push('/company-warehouses');
      }
    } catch (error) {
      message.error(response.data.message);
    } finally {
      loading.value = false;
    }
  };
</script>

<template>
    <Breadcrumbs :breadcrumbs="breadcrumbs" />
    <CompanyWarehouseForm 
      :form="form" 
      :onFinish="onFinish" 
      :loading="loading" 
      title="Создание склада" 
      formType="create" 
      submitButtonText="Создать склад"
    />
</template>