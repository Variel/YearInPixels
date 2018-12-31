<template>
    <div class="color-picker d-flex flex-wrap justify-content-center">
        <button class="color-item" v-for="color in colors" :style="{'background-color': color}" @click="selectColor(color)"></button>
    </div>
</template>
<script>
    export default {
        props: {},
        data() {
            return {
                colors: [
                    '#ef5777', '#575fcf', '#4bcffa', '#34e7e4', '#0be881',
                    '#f53b57', '#3c40c6', '#0fbcf9', '#00d8d6', '#05c46b',
                    '#ffc048', '#ffdd59', '#ff5e57', '#d2dae2', '#485460',
                    '#ffa801', '#ffd32a', '#ff3f34', '#808e9b', '#1e272e',
                ]
            };
        },
        methods: {
            selectColor(color) {
                this.$emit('onselect', { color: color });
            }
        },
        computed: {
        }
    };
</script>

<style lang="less" scoped>
    .color-picker {
        width: 153px;
        height: 123px;
    }

    .color-item {
        display: block;
        border: none;
        outline: none;
        width: 30px;
        height: 30px;
    }
</style>