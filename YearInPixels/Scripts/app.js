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
        year: new Date().getFullYear(),
        calendarId: '',
        defaultColor: '#f4f4f4',
        selectedYear: new Date().getFullYear(),
        showCustomizePopup: false,
        showNewCalendarPopup: false,
        showLoginPopup: false,
        showJoinPopup: false,
        showCalendarListPopup: false,
        loggedIn: false,
        loginForm: {
            email: '',
            password: ''
        },
        joinForm: {
            email: '',
            name: '',
            password: '',
            passwordConfirm: ''
        },
        joinValidation: {},
        myCalendarList: [],
        authorities: {
            view: true,
        }
    },
    methods: {
        login(e) {
            e.preventDefault();

            let form = new FormData;
            form.append('email', this.loginForm.email);
            form.append('password', this.loginForm.password);

            fetch('/auth/login',
                    {
                        method: 'POST',
                        body: form
                    })
                .then(res => res.json())
                .then(data => {
                    if (data.error) {
                        alert(data.message);
                        return;
                    }

                    this.loggedIn = true;
                    this.showLoginPopup = false;
                    this.loginForm.email = '';
                    this.loginForm.password = '';

                    fetch(`/api/calendars/${this.calendarId}/claimOwnership`)
                        .then(res => res.json())
                        .then(data => {
                            location.reload();
                            if (data.error)
                                return;
                        });
                });
        },
        join(e) {
            e.preventDefault();

            let valid = true;

            if (this.joinForm.name.replace(/\s/g, '').length === 0) {
                valid = false;
                this.$set(this.joinValidation, 'name', '이름을 입력하세요');
            } else {
                this.$set(this.joinValidation, 'name', null);
            }

            if (this.joinForm.email.replace(/\s/g, '').length === 0) {
                valid = false;
                this.$set(this.joinValidation, 'email', '이메일을 입력하세요');
            } else if (!/^.+@.+\..+$/.test(this.joinForm.email)) {
                valid = false;
                this.$set(this.joinValidation, 'email', '올바른 메일을 입력하세요');
            } else {
                this.$set(this.joinValidation, 'email', null);
            }

            if (this.joinForm.password.replace(/\s/g, '').length < 6) {
                valid = false;
                this.$set(this.joinValidation, 'password', '6자 이상 입력하세요');
            } else {
                this.$set(this.joinValidation, 'password', null);
            }

            if (this.joinForm.passwordConfirm !== this.joinForm.password) {
                valid = false;
                this.$set(this.joinValidation, 'passwordConfirm', '비밀번호와 같게 입력하세요');
            } else {
                this.$set(this.joinValidation, 'passwordConfirm', null);
            }


            if (!valid) {
                return;
            } else {
                this.joinValidation = {};
            }

            let form = new FormData;
            form.append('email', this.joinForm.email);
            form.append('name', this.joinForm.name);
            form.append('password', this.joinForm.password);

            fetch('/auth/join',
                    {
                        method: 'POST',
                        body: form
                    })
                .then(res => res.json())
                .then(data => {
                    if (data.error) {
                        alert(data.message);
                        return;
                    }

                    this.loggedIn = true;
                    this.showJoinPopup = false;

                    this.joinForm.name = '';
                    this.joinForm.email = '';
                    this.joinForm.password = '';
                    this.joinForm.passwordConfirm = '';

                    fetch(`/api/calendars/${calendarId}/claimOwnership`)
                        .then(res => res.json())
                        .then(data => {
                            location.reload();
                            if (data.error)
                                return;
                        });
                });
        },
        logout() {
            fetch('/auth/logout',
                    {
                        method: 'POST'
                    })
                .then(res => res.json())
                .then(data => {
                    location.reload();

                    if (data.error)
                        return;

                    this.loggedIn = false;
                });
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
        },
        showMyCalendarList() {
            this.showCalendarListPopup = true;

            fetch('/api/my/calendars')
                .then(res => res.json())
                .then(data => {
                    if (data.error)
                        return;

                    this.myCalendarList = data;
                });
        },
        deleteCalendar(e, calendar) {
            e.stopPropagation();
            e.preventDefault();

            if (!confirm('달력은 복구할 수 없습니다.\n정말 삭제하시겠습니까?')) {
                return;
            }

            let idx = this.myCalendarList.indexOf(calendar);
            this.myCalendarList.splice(idx, 1);

            fetch(`/api/calendars/${calendar.id}/delete`,
                    {
                        method: 'POST'
                    })
                .then(res => res.json())
                .then(data => {
                    if (data.error)
                        alert(data.message);

                    fetch('/api/my/calendars')
                        .then(res => res.json())
                        .then(data => {
                            if (data.error)
                                return;

                            this.myCalendarList = data;
                        });
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