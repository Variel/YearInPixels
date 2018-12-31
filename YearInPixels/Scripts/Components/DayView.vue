<template>
    <div class="day border" v-on:click.self="clicked" :class="{selected: dayObject.selected}" :style="{'background-color': backgroundColor}">
        <div class="day-popup-wrap" v-if="dayObject.selected" :style="{'left': popupLeft}">
            <div class="day-popup mx-auto bg-white rounded border shadow p-2 mb-5">
                <div class="d-flex">
                    <div class="btn bg-light">{{month}}월 {{day}}일</div>
                    <div class="dropdown ml-auto">
                        <button class="btn btn-light dropdown-toggle" type="button" id="dropdownMenuButton" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                            <span>
                                <span class="color-thumbnail d-inline-block mr-1" style="width: 13px; height: 13px;" :style="{'background-color': backgroundColor}"></span> {{label}}
                            </span>
                        </button>
                        <div class="dropdown-menu dropdown-menu-right" aria-labelledby="dropdownMenuButton">
                            <button class="dropdown-item" @click="selectOption(null)">
                                <span class="color-thumbnail d-inline-block mr-1" style="width: 13px; height: 13px;" :style="{'background-color': '#f4f4f4'}"></span> 선택 없음
                            </button>
                            <div class="dropdown-divider"></div>
                            <button v-for="(option, idx) in options" class="dropdown-item" @click="selectOption(idx)">
                                <span class="color-thumbnail d-inline-block mr-1" style="width: 13px; height: 13px;" :style="{'background-color': option.color}"></span> {{option.label}}
                            </button>
                            <div class="dropdown-divider"></div>
                            <button class="dropdown-item" @click="showCustomizeOptions"><i class="far fa-palette text-success"></i> 사용자정의</button>
                        </div>
                    </div>
                </div>
                <div class="form-group mt-3 mb-1">
                    <label>메모</label>
                    <textarea class="form-control" rows="5" style="resize: none" v-model="dayObject.note" @change="noteChanged"></textarea>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: {
            day: Number,
            month: Number,
            dayObject: Object
        },
        data() {
            return {
            };
        },
        methods: {
            clicked(e) {
                window.eventBus.$emit('dayClicked', {
                    month: this.month,
                    day: this.day
                });
            },
            selectOption(e) {
                this.$set(this.dayObject, 'option', e);

                let data = new FormData;
                data.append('optionId', e);

                fetch(`/api/calendars/${app.calendarId}/${this.month}/${this.day}/option`, {
                    method: 'POST',
                    body: data
                });
            },
            noteChanged(e) {
                let data = new FormData;
                data.append('note', this.dayObject.note);

                fetch(`/api/calendars/${app.calendarId}/${this.month}/${this.day}/note`, {
                    method: 'POST',
                    body: data
                });
            },
            showCustomizeOptions() {
                window.eventBus.$emit('showCustomizeOptions');
            }
        },
        computed: {
            label() {
                let option = this.dayObject.option;
                if (option === 0 || option > 0) {
                    return app.options[option].label;
                }

                return '선택 없음';
            },
            backgroundColor() {
                let option = this.dayObject.option;
                if (option === 0 || option > 0) {
                    return app.options[option].color;
                }

                return app.defaultColor;
            },
            popupLeft() {
                return -24 * (this.month - 1) - 3 + 'px';
            },
            options() {
                return app.options;
            }
        }
    };
</script>

<style lang="less" scoped>
    .day {
        position: relative;
        width: 25px;
        height: 25px;
        margin-left: -1px;
        margin-top: -1px;

        &.selected {
            border-width: 3px!important;
        }
    }

    .day-popup-wrap {
        position: relative;
        top: 23px;
        z-index: 10;
    }

    .day-popup {
        width: 289px;
    }
</style>