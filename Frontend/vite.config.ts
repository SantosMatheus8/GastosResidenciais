import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 3000,
    // Proxy para o backend em desenvolvimento
    proxy: {
      '/v1': {
        target: 'http://localhost:5285',
        changeOrigin: true,
      },
    },
  },
})
