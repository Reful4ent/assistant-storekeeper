<script setup>
    import { headerLinks } from './links';
    import { RouterLink } from 'vue-router';
</script>

<template>
    <a-layout-header>
        <a-menu
            v-model:selectedKeys="selectedKeys"
            theme="dark"
            mode="horizontal"
            :style="{ lineHeight: '64px' }"
        >
            <a-menu-item v-for="link in headerLinks" :key="link.path">
                <RouterLink :to="link.path">{{ link.name }}</RouterLink>
            </a-menu-item>
        </a-menu>
    </a-layout-header>
</template>