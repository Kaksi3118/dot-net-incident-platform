/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{html,ts}"],
  theme: {
    extend: {
      colors: {
        "sla-green": "#10b981",
        "sla-amber": "#f59e0b",
        "sla-red": "#ef4444"
      }
    }
  },
  plugins: []
};
