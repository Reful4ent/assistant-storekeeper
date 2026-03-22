<script setup>
    import Header from '../../widgets/header/Header.vue';
</script>

<template>
    <div class="main-layout">
        <Header />
        <main>
            <router-view />
        </main>
    </div>
</template>