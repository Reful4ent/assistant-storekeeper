<script setup>
  import Breadcrumbs from '../../../shared/breadcrumbs/Breadcrumbs.vue';
  import { breadcrumbs } from './ui/breadcrumbs/breadcrumbs';
  import NomenclatureForm from '../ui/NomenclatureFrom/NomenclatureForm.vue';
  import { reactive, ref, onMounted } from 'vue';
  import axios from 'axios';
  import { useRoute, useRouter } from 'vue-router';
  import { message } from 'ant-design-vue';

  const apiUrl = import.meta.env.VITE_API_URL;

  const route = useRoute();
  const router = useRouter();
  const id = route.params.id;

  const loading = ref(true);
  const form = reactive({
    name: '',
  });

  const getNomenclature = async () => {
    try {
      const response = await axios.get(apiUrl + '/api/nomenclatures/' + id);
      if (response.status === 200) {
        form.name = response.data.name;
      }
    } catch (error) {
      message.error('Не удалось получить номенклатуру');
    } finally {
      loading.value = false;
    }
  }


  const onFinish = async (values) => {
    loading.value = true;
    try {
      const response = await axios.put(apiUrl + '/api/nomenclatures/' + id, values);
      if (response.status === 200) {
        message.success('Номенклатура успешно обновлена');
        router.push('/nomenclatures');
      }
    } catch (error) {
      message.error('Не удалось обновить номенклатуру');
    } finally {
      loading.value = false;
    }
  }

  onMounted(async () => {
    await getNomenclature();
  });
</script>

<template>
    <Breadcrumbs :breadcrumbs="breadcrumbs" />
    <NomenclatureForm 
        :form="form"
        :onFinish="onFinish"
        :loading="loading"
        title="Редактировать номенклатуру"
        submitButtonText="Сохранить номенклатуру"
    />
</template>