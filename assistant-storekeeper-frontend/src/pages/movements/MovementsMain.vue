<script setup>
  import { ref, watch } from 'vue';
  import { useRouter, useRoute } from 'vue-router';
  import { message } from 'ant-design-vue';
  import { columns } from './ui/columns/columns';
  import axios from 'axios';
  import { statuses } from './ui/columns/statuses';

  const router = useRouter();
  const route = useRoute();

  const movements = ref([]);
  const loading = ref(true);

  const search = ref(route.query.search || '');
  const sortField = ref(route.query.sortBy || 'Date');
  const sortOrder = ref(route.query.isAscending === 'true' ? 'ascend' : 'descend');
  const pagination = ref({
    current: parseInt(route.query.page) || 1,
    pageSize: parseInt(route.query.pageSize) || 10,
    total: 0
  })

  const updateUrl = () => {
    router.replace({
      path: route.path,
      query: {
        ...(search.value && { search: search.value }),
        isAscending: sortOrder.value === 'ascend',
        sortBy: sortField.value,
        page: pagination.value.current,
        pageSize: pagination.value.pageSize,
      }
    })
  }

  const apiUrl = import.meta.env.VITE_API_URL;

  const getMovements = async () => {
    try {
      const params = {
        page: pagination.value.current,
        pageSize: pagination.value.pageSize,
        ...(search.value && { search: search.value }),
        sortBy: sortField.value,
        isAscending: sortOrder.value === 'ascend',
      }
      const response = await axios.get(apiUrl + '/api/movements', { params });
      movements.value = response.data.data;
      pagination.value.total = response.data.total;
    } catch (error) {
      message.error('Не удалось получить перемещения');
    } finally {
      loading.value = false;
    }
  }

  const onSearch = () => {
    pagination.value.current = 1;
    updateUrl();
  };

  const onTableChange = (paginationTable, filters, sorter) => {
    pagination.value.current = paginationTable.current;
    pagination.value.pageSize = paginationTable.pageSize;
    if (sorter.field) {
      if (typeof sorter.order === 'undefined') {
        sortField.value = 'Date';
        sortOrder.value = 'ascend';
      } else {
        sortField.value = sorter.field;
        sortOrder.value = sorter.order === 'ascend' ? 'ascend' : 'descend';
      }
    }
    updateUrl();
  }

  const onShowClick = (id) => {
    console.log(id);
    router.push(`/movements/${id}`);
  }

  const onAddClick = () => {
    router.push('/movements/create');
  }

  const onDeleteClick = async (id) => {
    try {
        console.log(id);
      const response = await axios.delete(apiUrl + `/api/movements/${id}`);
      if (response.status === 204) {
        message.success('Перемещение успешно удалено');
        await getMovements();
      }
    } catch (error) {
      message.error('Не удалось удалить перемещение');
    }
  }

  watch(() => route.query, (newQuery) => {
    pagination.value.current = parseInt(newQuery.page, 10) || 1;
    pagination.value.pageSize = parseInt(newQuery.pageSize, 10) || 10;
    search.value = newQuery.search || '';
    sortField.value = newQuery.sortBy || 'Date';
    sortOrder.value = newQuery.isAscending === 'true' ? 'ascend' : 'descend';
    getMovements();
  }, { immediate: true })
</script>

<template>
  <div class="flex justify-end mb-4 gap-2">
    <a-input-search
      v-model:value="search"
      placeholder="Поиск по ID"
      enter-button
      allow-clear
      @search="onSearch"
    />
    <a-button type="primary"@click="onAddClick">
      Добавить перемещение
    </a-button>
  </div>
  <a-table 
    bordered 
    :columns="columns" 
    :data-source="movements" 
    :loading="loading" 
    :pagination="{
      current: pagination.page,
      pageSize: pagination.pageSize,
      total: pagination.total,
      showSizeChanger: true,
      showTotal: (total) => `Всего: ${total}`,
    }"
    @change="onTableChange"
  >
    <template 
        #customFilterDropdown="{ setSelectedKeys, selectedKeys, confirm, clearFilters, column }"
    >
      <div style="padding: 8px">
        <a-input
          ref="searchInput"
          :placeholder="`Поиск по ID`"
          :value="selectedKeys[0]"
          style="width: 188px; margin-bottom: 8px; display: block"
          @change="e => setSelectedKeys(e.target.value ? [e.target.value] : [])"
          @pressEnter="handleSearch(selectedKeys, confirm, column.dataIndex)"
        />
        <a-button
          type="primary"
          size="small"
          style="width: 90px; margin-right: 8px"
          @click="handleSearch(selectedKeys, confirm, column.dataIndex)"
        >
          <template #icon><SearchOutlined /></template>
          Поиск
        </a-button>
        <a-button size="small" style="width: 90px" @click="handleReset(clearFilters)">
          Сбросить
        </a-button>
      </div>
    </template>
    <template #customFilterIcon="{ filtered }">
      <search-outlined :style="{ color: filtered ? '#108ee9' : undefined }" />
    </template>
    <template #bodyCell="{ column, record }">
      <template v-if="column.dataIndex === 'actions'">
        <div class="flex gap-2">
          <a-button 
            type="primary" 
            @click="onShowClick(record.id)"
          >
            Посмотреть
          </a-button>
          <a-popconfirm 
            title="Вы уверены, что хотите удалить это перемещение?" 
            @confirm="onDeleteClick(record.id)"
          >
            <a-button type="primary" danger>
              Удалить
            </a-button>
          </a-popconfirm>
        </div>
      </template>
      <template 
        v-if="(column.dataIndex === 'companyWarehouseFromName' || column.dataIndex === 'companyWarehouseToName') && record[column.dataIndex] == null"
      >
        <span>-</span>
      </template>
      <template 
        v-if="(column.dataIndex === 'date')"
      >
        <span>{{ new Date(record[column.dataIndex]).toLocaleDateString('ru-RU') }}</span>
      </template>
      <template v-if="column.dataIndex === 'status'">
        <a-tag 
            :color="statuses.find(status => status.value == record[column.dataIndex]).color"
        >
            {{ statuses.find(status => status.value == record[column.dataIndex]).label }}
        </a-tag>
      </template>
    </template>
  </a-table>
</template>