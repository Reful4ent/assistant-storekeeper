<script setup>
    import Header from '../../widgets/header/Header.vue';
</script>

<template>
    <a-layout>
        <Header />
        <a-layout-content style="padding: 24px;">
            <router-view />
        </a-layout-content>
    </a-layout>
</template>