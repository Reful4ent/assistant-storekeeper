<script setup>
  import Breadcrumbs from '../../../shared/breadcrumbs/Breadcrumbs.vue';
  import { breadcrumbs } from './ui/breadcrumbs/breadcrumbs';
  import CompanyWarehouseForm from '../ui/CompanyWarehouseForm/CompanyWarehouseForm.vue';
  import { reactive, ref, onMounted } from 'vue';
  import { message } from 'ant-design-vue';
  import axios from 'axios';
  import { useRoute } from 'vue-router';

  const apiUrl = import.meta.env.VITE_API_URL;

  const route = useRoute();
  const id = route.params.id;

  const loading = ref(true);
  const form = reactive({ 
    name: '' 
  });
  const nomenclatures = ref([]);
  let loadedNomenclatures = [];

  const onFinish = async (values) => {
    loading.value = true;
    try {
      const toDelete = loadedNomenclatures.filter((n) => !nomenclatures.value.some((n2) => n2.id === n.id));
      const toUpdate = nomenclatures.value.filter((n) => loadedNomenclatures.some((n2) => n2.id === n.id && n2.quantity !== n.quantity));
      const response = await axios.put(apiUrl + '/api/company-warehouses/' + id, {
        name: values.name,
      });
      await Promise.all([
        ...toDelete.map((n) =>
            axios.delete(apiUrl + '/api/company-warehouse-nomenclatures/' + n.id)
        ),
        ...toUpdate.map((n) =>
            axios.put(apiUrl + '/api/company-warehouse-nomenclatures/' + n.id, {
            quantity: n.quantity,
            nomenclatureId: n.nomenclatureId,
            companyWarehouseId: Number(id),
            id: n.id,
            })
        ),
      ]);
      if (response.status === 200) {
        message.success('Склад успешно обновлен');
      }
    } catch (error) {
      message.error(error);
    } finally {
      loading.value = false;
    }
  };


  const onDeleteNomenclature = (id) => {
    nomenclatures.value = nomenclatures.value.filter((n) => n.id !== id);
  };

  const onUpdateNomenclatureQuantity = ({ id, quantity }) => {
    const item = nomenclatures.value.find((n) => n.id === id);
    if (item) {
      item.quantity = quantity;
    }
  };

  const getCompanyWarehouse = async () => {
    try {
      const response = await axios.get(apiUrl + '/api/company-warehouses/' + id);
      if (response.status === 200) {
        form.name = response.data.name;
        nomenclatures.value = response.data.companyWarehouseNomenclatures;
        loadedNomenclatures = structuredClone(response.data.companyWarehouseNomenclatures);
      }
    } catch (error) {
      message.error(error);
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
      :onFinish="onFinish"
      :loading="loading"
      title="Редактирование склада"
      formType="edit"
      submitButtonText="Сохранить склад"
      :nomenclatures="nomenclatures"
      :onDeleteNomenclature="onDeleteNomenclature"
      :onUpdateNomenclatureQuantity="onUpdateNomenclatureQuantity"
    />
</template>