<script setup>
    import { headerLinks } from './links';
    import { RouterLink } from 'vue-router';
</script>

<template>
    <header>
        <nav>
            <ul>
                <li v-for="link in headerLinks" :key='link.path'>
                    <RouterLink :to="link.path">{{ link.name }}</RouterLink>
                </li>
            </ul>
        </nav>
    </header>
</template>