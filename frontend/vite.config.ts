import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

/**
 * Proxies API traffic to the .NET backend during local development.
 */
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5107',
        changeOrigin: true
      }
    }
  }
});
