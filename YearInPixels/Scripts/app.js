import MonthView from "./Components/MonthView.vue";
import Popup from "./Components/Popup.vue";
import ColorPicker from "./Components/ColorPicker.vue";

window.eventBus = new Vue({});
window.app = new Vue({
    el: '#app',
    data: {
        title: '',
        options: [],
        months: [],
        year: new Date().getYear(),
        calendarId: '',
        defaultColor: '#f4f4f4',
        showCustomizePopup: false
    },
    methods: {
        login() {

        },
        join() {

        },
        logout() {

        },
        setCalendarId(id) {
            this.calendarId = id;

            fetch(`/api/calendars/${id}`)
                .then(res => res.json())
                .then(data => {
                    // init calendar
                    let yearStart = moment(`${data.year}-01-01`).startOf('year');

                    for (let i = 1; i <= 12; i++) {
                        let monthStart = yearStart.clone().add(i - 1, 'months').startOf('month');
                        let monthEnd = monthStart.clone().endOf('month');

                        let days = new Array();
                        let endDate = monthEnd.date();

                        for (let j = 1; j <= endDate; j++) {
                            days.push({
                                number: j
                            });
                        }

                        this.months.push({
                            days: days
                        });
                    }

                    // init calendar name and year
                    this.title = data.title;
                    this.year = data.year;

                    // init day logs
                    for (let i in data.dayLogs) {
                        if (!data.dayLogs.hasOwnProperty(i))
                            continue;

                        let log = data.dayLogs[i];

                        this.$set(this.months[log.month - 1].days, log.day - 1, log);
                    }

                    // init options
                    if (data.options) {
                        this.options = data.options;
                    } else {
                        this.options = [
                            {
                                label: '아주 좋은 날',
                                color: '#C3D545'
                            },
                            {
                                label: '좋은 날',
                                color: '#5FB260'
                            },
                            {
                                label: '보통 날',
                                color: '#EDBCAE'
                            },
                            {
                                label: '나쁜 날',
                                color: '#B981BE'
                            },
                            {
                                label: '아주 나쁜 날',
                                color: '#FF404F'
                            }
                        ];
                    }
                });
        },
        removeOption(option) {
            let index = this.options.indexOf(option);
            this.options.splice(index, 1);

            this.updateOptions();
        },
        addOption() {
            function getRandomColor() {
                var letters = '0123456789ABCDEF';
                var color = '#';
                for (var i = 0; i < 6; i++) {
                    color += letters[Math.floor(Math.random() * 16)];
                }
                return color;
            }

            this.options.push({
                color: getRandomColor(),
                label: '',
            });

            this.updateOptions();
        },
        selectColor(option, e) {
            option.color = e.color;

            this.updateOptions();
        },
        updateOptions() {
            let form = new FormData;
            form.append('optionsString', JSON.stringify(this.options));

            fetch(`/api/calendars/${this.calendarId}/options`,
                {
                    method: 'POST',
                    body: form
                });
        },
        updateTitle() {
            let form = new FormData;
            form.append('title', this.title);

            fetch(`/api/calendars/${this.calendarId}/title`,
                {
                    method: 'POST',
                    body: form
                });
        }
    },
    computed: {
        selectedOption() {
            if (!this.selectedDate) {
                return null;
            }

            let dateObject = this.months[this.selectedDate.month - 1].days[this.selectedDate.day - 1];
            let options = this.options;

            return options[dateObject.option];
        },
        minOptionCount() {
            let max = 0;

            for (let i = 0; i < this.months.length; i++) {
                let month = this.months[i];

                for (let j = 0; j < month.days.length; j++) {
                    let day = month.days[j];

                    if (day.option > max)
                        max = day.option;
                }
            }

            return max;
        }
    },
    mounted() {
        eventBus.$on('dayClicked',
            (e) => {
                let selected = this.selectedDate;
                if (selected) {
                    this.$set(this.months[selected.month - 1].days[selected.day - 1], 'selected', false);
                    this.selectedDate = null;

                    if (selected.month !== e.month || selected.day !== e.day) {
                        this.$set(this.months[e.month - 1].days[e.day - 1], 'selected', true);
                        this.selectedDate = { month: e.month, day: e.day };
                    }
                } else {
                    this.$set(this.months[e.month - 1].days[e.day - 1], 'selected', true);
                    this.selectedDate = { month: e.month, day: e.day };
                }
            });

        eventBus.$on('showCustomizeOptions',
            (e) => {
                this.showCustomizePopup = true;
            });
    },
    components: { MonthView, Popup, ColorPicker }
});