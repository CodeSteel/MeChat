/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './**/*.{razor,html,cshtml,css}',
        './wwwroot/**/*.js',
    ],
    theme: {
        extend: {
            colors: {
                brand: {
                    '50': '#f2f9fd',
                    '100': '#e4f1fa',
                    '200': '#c3e2f4',
                    '300': '#8fcbea',
                    '400': '#53b0dd',
                    '500': '#369fd3',
                    '600': '#1d78ac',
                    '700': '#19618b',
                    '800': '#185274',
                    '900': '#1a4560',
                    '950': '#112c40',
                },
                "true-gray": {
                    50: "#FAFAFA",
                    100: "#F5F5F5",
                    200: "#E5E5E5",
                    300: "#D4D4D4",
                    400: "#A3A3A3",
                    500: "#737373",
                    600: "#525252",
                    700: "#404040",
                    800: "#262626",
                    900: "#171717",
                },
            },
        },
    },
    plugins: [
        require('@tailwindcss/forms'),
        require('@tailwindcss/aspect-ratio'),
        require('@tailwindcss/typography'),
    ]
}