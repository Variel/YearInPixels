<template>
    <div class="month">
        <div class="title text-muted py-1">{{month}}</div>
        <day-view v-for="(day, idx) in days" :month="month" :day="idx + 1" :day-object="day"></day-view>
    </div>
</template>

<script>
    import DayView from "./DayView.vue";

    export default {
        props: {
            month: Number,
            days: Array
        },
        data() {
            return {
            };
        },
        methods: {
        },
        mounted() {

            this.$on('dayClicked',
                (e) => {
                    console.log(e);
                });
        },
        components: { DayView }
    };
</script>

<style lang="less" scoped>
    .title {
        font-size: 12px;
        text-align: center;
    }
</style>