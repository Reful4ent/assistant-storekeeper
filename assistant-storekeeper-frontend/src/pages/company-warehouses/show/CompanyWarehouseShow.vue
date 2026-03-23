<script setup>
  import CompanyWarehouseForm from '../ui/CompanyWarehouseForm/CompanyWarehouseForm.vue';
  import Breadcrumbs from '../../../shared/breadcrumbs/Breadcrumbs.vue';
  import { breadcrumbs } from './ui/breadcrumbs.js';
  import { ref, onMounted, reactive } from 'vue';
  import axios from 'axios';
  import { message } from 'ant-design-vue';
  import { useRoute } from 'vue-router';

  const route = useRoute();
  const id = route.params.id;
  const apiUrl = import.meta.env.VITE_API_URL;

  const loading = ref(true);
  const form = reactive({ name: '' });
  const nomenclatures = ref([]);

  const getCompanyWarehouse = async () => {
    try {
      const response = await axios.get(apiUrl + '/api/company-warehouses/' + id);
      if (response.status === 200) {
        form.name = response.data.name;
        nomenclatures.value = response.data.companyWarehouseNomenclatures;
      }
    } catch (error) {
      message.error(response.data.name);
    } finally {
      loading.value = false;
    }
  };

  onMounted(async () => {
    await getCompanyWarehouse();
  });
</script>

<template>
    <Breadcrumbs :breadcrumbs="breadcrumbs" />
    <CompanyWarehouseForm 
      :form="form" 
      :loading="loading" 
      title="Просмотр склада" 
      formType="view"
      :nomenclatures="nomenclatures"
    />
</template>