<script setup>

const props = defineProps({
    breadcrumbs: {
        type: Array,
        required: true,
    },
});
</script>

<template>
    <a-breadcrumb>
        <a-breadcrumb-item v-for="breadcrumb in breadcrumbs" :key="breadcrumb.route" class="mr-[16px]">
            <RouterLink :to="breadcrumb.route">{{ breadcrumb.name }}</RouterLink>
        </a-breadcrumb-item>
    </a-breadcrumb>
</template>