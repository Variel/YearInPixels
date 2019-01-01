<template>
    <div class="popup p-2 d-flex justify-content-center align-items-center" v-if="visibility" @click.self="closePopup">
        <div class="popup-body bg-white shadow border rounded px-3 py-2">
            <button class="btn btn-close" @click="closePopup"><i class="fa fa-times"></i></button>
            <slot></slot>
        </div>
    </div>
</template>
<script>
    export default {
        props: {
            visibility: Boolean
        },
        data() {
            return {
            };
        },
        methods: {
            closePopup(e) {
                this.$emit('onclose');
            }
        },
        computed: {
        }
    };
</script>

<style lang="less" scoped>
    .popup {
        position: fixed;
        top: 0;
        right: 0;
        left: 0;
        bottom: 0;

        background-color: rgba(0,0,0,.7);

        z-index: 2000;
    }

    .popup-body {
        max-height: 80vh;
        position: relative;
    }

    .btn-close {
        position: absolute;
        top: 0;
        right: 0;
        z-index: 2010;
    }
</style>