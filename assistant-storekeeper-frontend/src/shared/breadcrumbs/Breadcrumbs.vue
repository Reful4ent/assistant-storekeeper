<script setup>

const props = defineProps({
    breadcrumbs: {
        type: Array,
        required: true,
    },
});
</script>

<template>
    <a-breadcrumb class="!mb-4">
        <a-breadcrumb-item v-for="breadcrumb in breadcrumbs" :key="breadcrumb.route">
            <RouterLink v-if="breadcrumb.route" :to="breadcrumb.route">{{ breadcrumb.title }}</RouterLink>
            <span v-else>{{ breadcrumb.title }}</span>
        </a-breadcrumb-item>
    </a-breadcrumb>
</template>