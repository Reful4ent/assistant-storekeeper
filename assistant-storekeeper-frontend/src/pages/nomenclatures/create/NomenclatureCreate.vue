<script setup>
  import Breadcrumbs from '../../../shared/breadcrumbs/Breadcrumbs.vue';
  import { breadcrumbs } from './ui/breadcrumbs/breadcrumbs';
  import NomenclatureForm from '../ui/NomenclatureFrom/NomenclatureForm.vue';
  import { reactive, ref } from 'vue';
  import axios from 'axios';
  import { useRouter } from 'vue-router';
  import { message } from 'ant-design-vue';

  const apiUrl = import.meta.env.VITE_API_URL;

  const router = useRouter()

  const loading = ref(false);
  const form = reactive({
    name: '',
  });

  const onFinish = async (values) => {
    loading.value = true;
    try {
      const response = await axios.post(apiUrl + '/api/nomenclatures', values);
      if (response.status === 201) {
        message.success('Номенклатура успешно создана');
        router.push('/nomenclatures');
      }
    } catch (error) {
      message.error('Не удалось создать номенклатуру');
    } finally {
      loading.value = false;
    }
  }
</script>

<template>
    <Breadcrumbs :breadcrumbs="breadcrumbs" />
    <NomenclatureForm 
        :form="form"
        :onFinish="onFinish"
        :loading="loading"
        title="Создать номенклатуру"
        submitButtonText="Сохранить номенклатуру"
    />
</template>